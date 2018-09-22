using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.ContentManagement.Backups
{
    [Feature(Constants.Areas.Backups)]
    public class BackupsPermissions : IPermissionProvider
    {
        public static readonly Permission ManageBackups = new Permission { Name = "ManageBackups", Category = "Backup/Restore", Description = "Manage database backups" };

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageBackups
            };
        }
    }
}