namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class LevelCandidatesPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerLevelCandidates = new Permission
        {
            Name = "ManagerLevelCandidates", 
            Category = "Management", 
            Description = "Manager LevelCandidates", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerLevelCandidates;
        }
    }
}
