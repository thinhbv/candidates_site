using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.Configuration
{
    [Feature(Constants.Areas.Core)]
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageSiteSettings = new Permission { Name = "ManageSiteSettings", Category = "System", Description = "Manage site settings" };

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageSiteSettings
            };
        }
    }
}
