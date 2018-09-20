namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class PositionsPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerPositions = new Permission
        {
            Name = "ManagerPositions", 
            Category = "Management", 
            Description = "Manager Positions", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerPositions;
        }
    }
}
