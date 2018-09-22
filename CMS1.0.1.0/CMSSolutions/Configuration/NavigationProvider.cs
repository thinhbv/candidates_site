using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.Configuration
{
    [Feature(Constants.Areas.Core)]
    public class NavigationProvider : INavigationProvider
    {
        public NavigationProvider()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Configuration"),
                menu => menu.Add(T("Settings"), "5", item => item
                    .Action("Index", "Settings", new { area = Constants.Areas.Core })
                    .IconCssClass("cx-icon cx-icon-settings")
                    .Permission(Permissions.ManageSiteSettings)));
        }
    }
}
