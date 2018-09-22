using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.ContentManagement.Aliases
{
    [Feature(Constants.Areas.Aliases)]
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
                menu => menu.Add(T("Alias"), "5", item => item
                    .Action("Index", "Alias", new { area = Constants.Areas.Aliases })
                    .IconCssClass("cx-icon cx-icon-alias")
                    .Permission(StandardPermissions.FullAccess)));
        }
    }
}