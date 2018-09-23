using CMSSolutions.Localization;
using CMSSolutions.Web.UI.Navigation;
using CMSSolutions.Websites.Permissions;

namespace CMSSolutions.Websites.Menus
{
    public class NavigationProvider : INavigationProvider
    {
        public Localizer T { get; set; }

        public NavigationProvider()
        {
            T = NullLocalizer.Instance;
        }

        public void GetNavigation(NavigationBuilder builder)
        {
			builder.Add(T("Dashboard"), "0", BuildHomeMenu);
			builder.Add(T("Administration"), "1", BuildMaster);
			builder.Add(T("Recruitment Management"), "2", BuildRecruitment);
			builder.Add(T("Interview Management"), "3", BuildInterview);
			builder.Add(T("Employee Management"), "4", BuildEmployee);
        }

        private void BuildHomeMenu(NavigationItemBuilder builder)
        {
			builder.IconCssClass("fa-home")
				.Action("Index", "Admin", new { area = "" })
				.Permission(AdminPermissions.ManagerAdmin);
        }

		private void BuildMaster(NavigationItemBuilder builder)
		{
			builder.IconCssClass("fa-th");

			builder.Add(T("Languages"), "0", b => b
				.Action("Index", "Languages", new { area = "" })
				.Permission(LanguagesPermissions.ManagerLanguages));

			builder.Add(T("Levels"), "2", b => b
				.Action("Index", "Levels", new { area = "" })
				.Permission(LevelsPermissions.ManagerLevels));

			builder.Add(T("Stakeholder"), "3", b => b
				.Action("Index", "Stakeholder", new { area = "" })
				.Permission(StakeholderPermissions.ManagerStakeholder));

			builder.Add(T("Email Templates"), "4", b => b
				.Action("Index", "MailTemplates", new { area = "" })
				.Permission(MailTemplatesPermissions.ManagerMailTemplates));
		}
		private void BuildRecruitment(NavigationItemBuilder builder)
		{
			builder.IconCssClass("fa-building");

			builder.Add(T("Dashboard"), "0", b => b
				.Action("Index", "DashboardCandidate", new { area = "" })
				.Permission(AdminPermissions.ManagerDashboardCandidate));

			builder.Add(T("Recruitment Requests"), "1", b => b
				.Action("Index", "Positions", new { area = "" })
				.Permission(PositionsPermissions.ManagerPositions));

			builder.Add(T("Candidate Management"), "2", b => b
				.Action("Index", "Candidates", new { area = "" })
				.Permission(CandidatesPermissions.ManagerCandidates));
		}

		private void BuildInterview(NavigationItemBuilder builder)
		{
			builder.IconCssClass("fa-desktop");

			builder.Add(T("Dashboard"), "0", b => b
				.Action("Index", "DashboardInterview", new { area = "" })
				.Permission(AdminPermissions.ManagerDashboardInterview));

			builder.Add(T("Interview Schedules"), "2", b => b
				.Action("Index", "ScheduleInterview", new { area = "" })
				.Permission(ScheduleInterviewPermissions.ManagerScheduleInterview));

			builder.Add(T("Interview Request List"), "3", b => b
				.Action("Index", "Interview", new { area = "" })
				.Permission(InterviewPermissions.ManagerInterview));

			builder.Add(T("Candidate Questions"), "4", b => b
				.Action("Index", "Questions", new { area = "" })
				.Permission(QuestionsPermissions.ManagerQuestions));

			builder.Add(T("Interviewer Questions"), "5", b => b
				.Action("Index", "Report", new { area = "" })
				.Permission(AdminPermissions.ManagerReports));
		}

		private void BuildEmployee(NavigationItemBuilder builder)
		{
			builder.IconCssClass("fa-group");

			builder.Add(T("Dashboard"), "0", b => b
				.Action("Index", "DashboardEmployee", new { area = "" })
				.Permission(AdminPermissions.ManagerDashboardEmployee));

			builder.Add(T("Sync Data From Portal"), "1", b => b
				.Action("Index", "SyncData", new { area = "" })
				.Permission(AdminPermissions.ManagerSyncData));

			builder.Add(T("Employee's Info"), "2", b => b
				.Action("Index", "Employees", new { area = "" })
				.Permission(EmployeePermissions.ManagerEmployees));

			builder.Add(T("Projects Assignment"), "3", b => b
				.Action("Index", "ProjectsAssignment", new { area = "" })
				.Permission(AdminPermissions.ManagerProjectsAssignment));
		}
    }
}