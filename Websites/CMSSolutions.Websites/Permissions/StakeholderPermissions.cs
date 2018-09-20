namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class StakeholderPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerStakeholder = new Permission
        {
            Name = "ManagerStakeholder", 
            Category = "Management", 
            Description = "Manager Stakeholder", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerStakeholder;
        }
    }
}
