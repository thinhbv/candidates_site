using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CMSSolutions.Web.Security.Permissions;

namespace CMSSolutions.Websites.Permissions
{
	public class EmployeePermissions : IPermissionProvider
	{
		public static readonly Permission ManagerEmployees = new Permission
		{
			Name = "ManagerEmployees",
			Category = "Management",
			Description = "Manager Employees",
		};

		public IEnumerable<Permission> GetPermissions()
		{
			yield return ManagerEmployees;
		}
	}
}