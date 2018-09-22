namespace CMSSolutions.Websites.Permissions
{
    using System.Collections.Generic;
    using CMSSolutions.Web.Security.Permissions;
    
    
    public class QuestionsPermissions : IPermissionProvider
    {
        
        public static readonly Permission ManagerQuestions = new Permission
        {
            Name = "ManagerQuestions", 
            Category = "Management", 
            Description = "Manager Questions", 
        };

        
        public virtual IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerQuestions;
        }
    }
}
