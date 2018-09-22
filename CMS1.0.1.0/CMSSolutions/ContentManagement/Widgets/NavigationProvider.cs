using System.Web.Mvc;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.ContentManagement.Widgets
{
    [Feature(Constants.Areas.Widgets)]
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
                menu => menu.Add(T("Widgets"), "5", item => item
                    .Action("Index", "Widget", new { area = Constants.Areas.Widgets, pageId = UrlParameter.Optional })
                    .IconCssClass("cx-icon cx-icon-widgets")
                    .Permission(WidgetPermissions.ManageWidgets)));
        }
    }
}