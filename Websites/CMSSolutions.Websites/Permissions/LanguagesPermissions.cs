namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class LanguagesPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerLanguages = new Permission
        {
            Name = "ManagerLanguages", 
            Category = "Management", 
            Description = "Manager Languages", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerLanguages;
        }
    }
}
