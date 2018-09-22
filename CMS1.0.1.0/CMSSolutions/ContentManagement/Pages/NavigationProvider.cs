using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.ContentManagement.Pages
{
    [Feature(Constants.Areas.Pages)]
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
                menu => menu.Add(T("Pages"), "5", item => item
                    .Action("Index", "Page", new { area = Constants.Areas.Pages })
                    .IconCssClass("cx-icon cx-icon-pages")
                    .Permission(PagesPermissions.ManagePages)));
        }
    }
}