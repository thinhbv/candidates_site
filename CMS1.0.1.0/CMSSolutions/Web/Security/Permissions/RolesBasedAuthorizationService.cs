using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using CMSSolutions.Collections;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.Security.Services;

namespace CMSSolutions.Web.Security.Permissions
{
    [Feature(Constants.Areas.Security)]
    public class RolesBasedAuthorizationService : IAuthorizationService
    {
        private readonly IMembershipService roleService;
        private readonly IAuthorizationServiceEventHandler authorizationServiceEventHandler;
        private static readonly string[] anonymousRole = new[] { "Anonymous" };
        private static readonly string[] authenticatedRole = new[] { "Authenticated" };

        public RolesBasedAuthorizationService(IMembershipService roleService, IAuthorizationServiceEventHandler authorizationServiceEventHandler)
        {
            this.roleService = roleService;
            this.authorizationServiceEventHandler = authorizationServiceEventHandler;
        }

        public Localizer T { get; set; }

        public void CheckAccess(Permission permission, IUserInfo user)
        {
            if (!TryCheckAccess(permission, user))
            {
                throw new SecurityException(T("A security exception occurred in the system."));
            }
        }

        public bool TryCheckAccess(Permission permission, IUserInfo user)
        {
            var context = new CheckAccessContext { Permission = permission, User = user };
            authorizationServiceEventHandler.Checking(context);

            for (var adjustmentLimiter = 0; adjustmentLimiter != 3; ++adjustmentLimiter)
            {
                if (!context.Granted && context.User != null && context.User.SuperUser)
                {
                    context.Granted = true;
                }

                if (!context.Granted)
                {
                    // determine which set of permissions would satisfy the access check
                    var grantingNames = PermissionNames(context.Permission, Enumerable.Empty<string>()).Distinct().ToArray();

                    // determine what set of roles should be examined by the access check
                    IEnumerable<string> rolesToExamine;
                    if (context.User == null)
                    {
                        rolesToExamine = anonymousRole;
                    }
                    else
                    {
                        rolesToExamine = roleService.GetRolesForUser(context.User.Id).Select(x => x.Name).ToList();
                        if (!rolesToExamine.Contains(anonymousRole[0]))
                        {
                            rolesToExamine = rolesToExamine.Concat(authenticatedRole);
                        }
                    }

                    foreach (var role in rolesToExamine)
                    {
                        foreach (var rolePermission in roleService.GetPermissionsForRole(role))
                        {
                            string possessedName = rolePermission.Name;
                            if (grantingNames.Any(grantingName => String.Equals(possessedName, grantingName, StringComparison.OrdinalIgnoreCase)))
                            {
                                context.Granted = true;
                            }

                            if (context.Granted)
                                break;
                        }

                        if (context.Granted)
                            break;
                    }
                }

                context.Adjusted = false;
                authorizationServiceEventHandler.Adjust(context);
                if (!context.Adjusted)
                    break;
            }

            authorizationServiceEventHandler.Complete(context);

            return context.Granted;
        }

        private static IEnumerable<string> PermissionNames(Permission permission, IEnumerable<string> stack)
        {
            // the given name is tested
            yield return permission.Name;

            // iterate implied permissions to grant, it present
            if (!permission.ImpliedBy.IsNullOrEmpty())
            {
                foreach (var impliedBy in permission.ImpliedBy)
                {
                    // avoid potential recursion
                    if (stack.Contains(impliedBy.Name))
                        continue;

                    // otherwise accumulate the implied permission names recursively
                    foreach (var impliedName in PermissionNames(impliedBy, stack.Concat(new[] { permission.Name })))
                    {
                        yield return impliedName;
                    }
                }
            }

            yield return StandardPermissions.FullAccess.Name;
        }
    }
}