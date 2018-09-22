namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class MailTemplatesPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerMailTemplates = new Permission
        {
            Name = "ManagerMailTemplates", 
            Category = "Management", 
            Description = "Manager MailTemplates", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerMailTemplates;
        }
    }
}
