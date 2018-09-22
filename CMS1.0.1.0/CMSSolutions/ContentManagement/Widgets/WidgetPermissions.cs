using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.ContentManagement.Widgets
{
    [Feature(Constants.Areas.Widgets)]
    public class WidgetPermissions : IPermissionProvider
    {
        public static readonly Permission ManageWidgets = new Permission { Name = "ManageWidgets", Description = "Manage widgets", Category = "Content Management" };

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageWidgets
            };
        }
    }
}