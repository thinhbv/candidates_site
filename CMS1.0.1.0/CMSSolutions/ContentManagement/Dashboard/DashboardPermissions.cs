using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.ContentManagement.Dashboard
{
    [Feature(Constants.Areas.Dashboard)]
    public class DashboardPermissions : IPermissionProvider
    {
        public static readonly Permission ManageModules = new Permission { Name = "ManageModules", Category = "System", Description = "Manage site modules" };
        public static readonly Permission ManageThemes = new Permission { Name = "ManageThemes", Category = "System", Description = "Manage site themes" };
        public static readonly Permission ManageModuleSettings = new Permission { Name = "ManageModuleSettings", Category = "System", Description = "Manage mudule settings" };

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageThemes,
                ManageModules,
                ManageModuleSettings
            };
        }
    }
}