namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class InterviewPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerInterview = new Permission
        {
            Name = "ManagerInterview", 
            Category = "Management", 
            Description = "Manager Interview", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerInterview;
        }
    }
}
