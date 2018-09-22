using System;
using System.Collections.Generic;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.Websites.Permissions
{
    public class AdminPermissions : IPermissionProvider
    {
        public static readonly Permission ManagerAdmin = new Permission
        {
            Name = "ManagerAdmin",
            Category = "Management",
			Description = "Dashboard"
        };

		public static readonly Permission ManagerReports = new Permission
		{
			Name = "ManagerReports",
			Category = "Management",
			Description = "Manager Reports",
		};

        public IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerAdmin;
        }
    }
}