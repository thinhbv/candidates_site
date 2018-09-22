using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.ContentManagement.Pages
{
    [Feature(Constants.Areas.Pages)]
    public class PagesPermissions : IPermissionProvider
    {
        public static readonly Permission ManagePages = new Permission { Name = "ManagePages", Category = "Content Management", Description = "Manage pages" };

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManagePages
            };
        }
    }
}