using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.ContentManagement.SEO
{
    [Feature(Constants.Areas.SEO)]
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
                menu => menu.Add(T("Meta Tags"), "5", item => item
                    .Action("Index", "MetaTag", new { area = Constants.Areas.SEO })
                    .IconCssClass("cx-icon cx-icon-tags")
                    .Permission(Permissions.ManageSEO)));
        }
    }
}
