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
			builder.Add(T("Interview Schedules"), "1", b => b
				.Action("Index", "ScheduleInterview", new { area = "" })
				.IconCssClass("fa-calendar")
				.Permission(ScheduleInterviewPermissions.ManagerScheduleInterview));
			builder.Add(T("Administration"), "2", BuildMaster);
			builder.Add(T("Recruitment"), "3", BuildRecruitment);
			builder.Add(T("Interview"), "4", BuildInterview);
			builder.Add(T("Employee"), "5", BuildEmployee);
			builder.Add(T("Interview Reports"), "5", b => b
				.Action("Index", "Report", new { area = "" })
				.IconCssClass("fa-bar-chart-o")
				.Permission(AdminPermissions.ManagerReports));
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

			builder.Add(T("Recruitment Notice"), "0", b => b
				.Action("Index", "Positions", new { area = "" })
				.Permission(PositionsPermissions.ManagerPositions));


			builder.Add(T("Candidate List"), "1", b => b
				.Action("Index", "Candidates", new { area = "" })
				.Permission(CandidatesPermissions.ManagerCandidates));
		}

		private void BuildInterview(NavigationItemBuilder builder)
		{
			builder.IconCssClass("fa-desktop");

			builder.Add(T("Interview List"), "1", b => b
				.Action("Index", "Interview", new { area = "" })
				.Permission(InterviewPermissions.ManagerInterview));

			builder.Add(T("Questions"), "2", b => b
				.Action("Index", "Questions", new { area = "" })
				.Permission(QuestionsPermissions.ManagerQuestions));
		}

		private void BuildEmployee(NavigationItemBuilder builder)
		{
			builder.IconCssClass("fa-group");

			builder.Add(T("Employee List"), "2", b => b
				.Action("Index", "EmployeeController", new { area = "" })
				.Permission(AdminPermissions.ManagerEmployees));
		}
    }
}