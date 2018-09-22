using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.ContentManagement.Newsletters
{
    [Feature(Constants.Areas.Newsletters)]
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
                menu => menu.Add(T("Newsletters"), "5", item => item
                    .Action("Index", "Newsletter", new { area = Constants.Areas.Newsletters })
                    .IconCssClass("cx-icon cx-icon-newsletters")
                    .Permission(NewsletterPermissions.ManageNewsletters)));
        }
    }
}