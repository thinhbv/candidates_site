using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.ContentManagement.Menus
{
    [Feature(Constants.Areas.Menus)]
    public class MenusPermissions : IPermissionProvider
    {
        public static readonly Permission ManageMenus = new Permission { Name = "ManageMenus", Category = "Content Management", Description = "Manage menus" };

        public IEnumerable<Permission> GetPermissions()
        {
            yield return ManageMenus;
        }
    }
}