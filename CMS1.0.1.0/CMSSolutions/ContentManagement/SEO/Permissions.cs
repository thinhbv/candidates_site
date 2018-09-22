using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.ContentManagement.SEO
{
    [Feature(Constants.Areas.SEO)]
    public class Permissions : IPermissionProvider
    {
        // ReSharper disable InconsistentNaming
        public static readonly Permission ManageSEO = new Permission { Name = "ManageSEO", Category = "Content Management", Description = "Manage SEO" };
        // ReSharper restore InconsistentNaming

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageSEO
            };
        }
    }
}
