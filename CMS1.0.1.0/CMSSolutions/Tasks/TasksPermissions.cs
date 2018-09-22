using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.Tasks
{
    [Feature(Constants.Areas.ScheduledTasks)]
    public class TasksPermissions : IPermissionProvider
    {
        public static readonly Permission ManageScheduleTasks = new Permission { Name = "ManageScheduleTasks", Category = "System", Description = "Manage schedule tasks" };

        public IEnumerable<Permission> GetPermissions()
        {
            yield return ManageScheduleTasks;
        }
    }
}