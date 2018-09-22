using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.ContentManagement.Lists
{
    [Feature(Constants.Areas.Lists)]
    public class ListsPermissions : IPermissionProvider
    {
        public static readonly Permission ManageLists = new Permission { Name = "ManageLists", Category = "Content Management", Description = "Manage lists" };

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageLists
            };
        }
    }
}