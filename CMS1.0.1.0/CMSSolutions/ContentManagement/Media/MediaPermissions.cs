using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.ContentManagement.Media
{
    [Feature(Constants.Areas.Media)]
    public class MediaPermissions : IPermissionProvider
    {
        public static readonly Permission ManageMedia = new Permission { Name = "ManageMedia", Category = "Content Management", Description = "Manage media" };

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageMedia
            };
        }
    }
}