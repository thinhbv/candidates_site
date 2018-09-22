using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.Indexing
{
    [Feature(Constants.Areas.Indexing)]
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
                menu => menu.Add(T("Search Index"), "5", item => item
                    .Action("Index", "Indexing", new { area = Constants.Areas.Indexing })
                    .IconCssClass("cx-icon cx-icon-search")
                    .Permission(StandardPermissions.FullAccess)));
        }
    }
}