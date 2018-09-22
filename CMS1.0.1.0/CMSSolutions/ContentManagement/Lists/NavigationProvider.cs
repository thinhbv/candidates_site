using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.ContentManagement.Lists
{
    [Feature(Constants.Areas.Lists)]
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
                menu => menu.Add(T("Lists"), "5", item => item
                    .Action("Index", "List", new { area = Constants.Areas.Lists })
                    .IconCssClass("cx-icon cx-icon-lists")
                    .Permission(ListsPermissions.ManageLists)));
        }
    }
}