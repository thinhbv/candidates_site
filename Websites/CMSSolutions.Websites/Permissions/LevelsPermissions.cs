namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class LevelsPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerLevels = new Permission
        {
            Name = "ManagerLevels", 
            Category = "Management", 
            Description = "Manager Levels", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerLevels;
        }
    }
}
