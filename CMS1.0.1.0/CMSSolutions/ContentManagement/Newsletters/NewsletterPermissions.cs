using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.ContentManagement.Newsletters
{
    [Feature(Constants.Areas.Newsletters)]
    public class NewsletterPermissions : IPermissionProvider
    {
        public static readonly Permission ManageNewsletters = new Permission { Name = "ManageNewsletters", Category = "Content Management", Description = "Manage Newsletters" };

        public IEnumerable<Permission> GetPermissions()
        {
            return new[]
            {
                ManageNewsletters
            };
        }
    }
}