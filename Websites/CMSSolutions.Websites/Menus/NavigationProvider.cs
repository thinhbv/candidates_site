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
            builder.Add(T("Home"), "0", BuildHomeMenu);
			builder.Add(T("Administration"), "1", BuildMaster);
			builder.Add(T("Interview"), "2", BuildRecruitment);
			builder.Add(T("Reports"), "3", BuildReport);
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

			builder.Add(T("Positions"), "1", b => b
				.Action("Index", "Positions", new { area = "" })
				.Permission(PositionsPermissions.ManagerPositions));

			builder.Add(T("Levels"), "2", b => b
				.Action("Index", "Levels", new { area = "" })
				.Permission(LevelsPermissions.ManagerLevels));

			builder.Add(T("Stakeholder"), "3", b => b
				.Action("Index", "Stakeholder", new { area = "" })
				.Permission(StakeholderPermissions.ManagerStakeholder));
		}

		private void BuildRecruitment(NavigationItemBuilder builder)
		{
			builder.IconCssClass("fa-building");

			builder.Add(T("Candidate List"), "0", b => b
				.Action("Index", "Candidates", new { area = "" })
				.Permission(CandidatesPermissions.ManagerCandidates));

			builder.Add(T("Interview List"), "1", b => b
				.Action("Index", "Interview", new { area = "" })
				.Permission(InterviewPermissions.ManagerInterview));
		}

		private void BuildReport(NavigationItemBuilder builder)
		{
			builder.IconCssClass("fa-bar-chart-o");

			builder.Add(T("Interview Result"), "0", b => b
				.Action("Index", "Candidates", new { area = "" })
				.Permission(CandidatesPermissions.ManagerCandidates));

			builder.Add(T("Summary Report"), "0", b => b
				.Action("Index", "Candidates", new { area = "" })
				.Permission(CandidatesPermissions.ManagerCandidates));
		}
    }
}