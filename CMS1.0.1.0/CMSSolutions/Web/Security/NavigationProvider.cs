using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.Web.Security
{
    [Feature(Constants.Areas.Security)]
    public class NavigationProvider : INavigationProvider
    {
        public NavigationProvider()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Membership"), "5", BuildMembershipMenu);
        }

        private void BuildMembershipMenu(NavigationItemBuilder builder)
        {
            builder.IconCssClass("fa-user");
            builder.Permission(StandardPermissions.FullAccess);

            builder.Add(T("Users"), "1", item => item.Action("Index", "User", new { area = Constants.Areas.Security }).IconCssClass("fa-user-md ").Permission(StandardPermissions.FullAccess));
            builder.Add(T("Roles"), "2", item => item.Action("Index", "Role", new { area = Constants.Areas.Security }).IconCssClass("fa-male").Permission(StandardPermissions.FullAccess));
        }
    }

    [Feature(Constants.Areas.OAuth)]
    public class OAuthNavigationProvider : INavigationProvider
    {
        public OAuthNavigationProvider()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Membership"),
                menu => menu.Add(T("OAuth Providers"), "5", item => item
                    .Action("Index", "OAuthProvider", new { area = Constants.Areas.OAuth })));
        }
    }
}