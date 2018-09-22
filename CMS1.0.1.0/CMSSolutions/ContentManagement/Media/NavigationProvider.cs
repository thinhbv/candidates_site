using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.ContentManagement.Media
{
    [Feature(Constants.Areas.Media)]
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
                menu => menu.Add(T("Media"), "5", item => item
                    .Action("Index", "Media", new { area = Constants.Areas.Media })
                    .IconCssClass("cx-icon cx-icon-media")
                    .Permission(MediaPermissions.ManageMedia)));
        }
    }
}