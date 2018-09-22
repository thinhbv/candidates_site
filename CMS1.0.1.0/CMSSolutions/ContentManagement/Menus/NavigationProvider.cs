using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Localization;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.ContentManagement.Menus
{
    [Feature(Constants.Areas.Menus)]
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
                menu => menu.Add(T("Menus"), "5", item => item
                    .Action("Index", "Menu", new { area = Constants.Areas.Menus })
                    .IconCssClass("cx-icon cx-icon-menus")
                    .Permission(MenusPermissions.ManageMenus)));
        }
    }

    public class FeatureProvider : IFeatureProvider
    {
        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
            {
                Id = Constants.Areas.Menus,
                Name = "Menus",
                Category = "Content"
            };
        }
    }
}