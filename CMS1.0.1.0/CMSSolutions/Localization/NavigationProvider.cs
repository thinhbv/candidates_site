using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.Localization
{
    [Feature(Constants.Areas.Localization)]
    public class NavigationProvider : INavigationProvider
    {
        public NavigationProvider()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("CMS"),
                menu => menu.Add(T("Localization"), "5", item => item
                    .Action("Index", "Language", new { area = Constants.Areas.Localization })
                    .IconCssClass("cx-icon cx-icon-localization")
                    .Permission(StandardPermissions.FullAccess)));
        }
    }
}