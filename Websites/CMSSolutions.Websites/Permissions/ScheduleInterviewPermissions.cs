namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class ScheduleInterviewPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerScheduleInterview = new Permission
        {
            Name = "ManagerScheduleInterview", 
            Category = "Management", 
            Description = "Manager ScheduleInterview", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerScheduleInterview;
        }
    }
}
