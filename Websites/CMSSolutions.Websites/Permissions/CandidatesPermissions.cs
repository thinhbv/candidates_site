namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class CandidatesPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerCandidates = new Permission
        {
            Name = "ManagerCandidates", 
            Category = "Management", 
            Description = "Manager Candidates", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerCandidates;
        }
    }
}
