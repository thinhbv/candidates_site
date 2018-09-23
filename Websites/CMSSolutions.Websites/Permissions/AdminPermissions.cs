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

		public static readonly Permission ManagerDashboardCandidate = new Permission
        {
			Name = "ManagerDashboardCandidate",
            Category = "Management",
			Description = "Candidate Dashboard"
        };

		public static readonly Permission ManagerDashboardInterview = new Permission
        {
			Name = "ManagerDashboardInterview",
            Category = "Management",
			Description = "Dashboard Interview"
        };

		public static readonly Permission ManagerDashboardEmployee = new Permission
		{
			Name = "ManagerDashboardEmployee",
			Category = "Management",
			Description = "Dashboard Employee"
		};

		public static readonly Permission ManagerSyncData = new Permission
		{
			Name = "ManagerSyncData",
			Category = "Management",
			Description = "Sync Data"
		};

		public static readonly Permission ManagerProjectsAssignment = new Permission
		{
			Name = "ManagerProjectsAssignment",
			Category = "Management",
			Description = "ProjectsAssignment"
		};

        public IEnumerable<Permission> GetPermissions()
        {
            yield return ManagerAdmin;
			yield return ManagerReports;
			yield return ManagerDashboardCandidate;
			yield return ManagerDashboardInterview;
			yield return ManagerDashboardEmployee;
			yield return ManagerSyncData;
			yield return ManagerProjectsAssignment;
        }
    }
}