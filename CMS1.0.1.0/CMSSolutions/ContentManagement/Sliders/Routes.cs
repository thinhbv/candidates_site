using System.Collections.Generic;
using CMSSolutions.ContentManagement.Sliders.Controllers;
using CMSSolutions.ContentManagement.Widgets;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Localization;
using CMSSolutions.Web.Mvc.Routes;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.ContentManagement.Sliders
{
    [Feature(Constants.Areas.Sliders)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(SliderController));
            MapAttributeRoutes(routes, typeof(SlideController));
        }
    }

    [Feature(Constants.Areas.Sliders)]
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
                menu => menu.Add(T("Slideshows"), "5", item => item
                    .Action("Index", "Slider", new { area = Constants.Areas.Sliders })
                    .IconCssClass("cx-icon cx-icon-slideshows")
                    .Permission(WidgetPermissions.ManageWidgets)));
        }
    }

    public class FeatureProvider : IFeatureProvider
    {
        #region IFeatureProvider Members

        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
            {
                Id = Constants.Areas.Sliders,
                Name = "Sliders",
                Category = "Content"
            };
        }

        #endregion IFeatureProvider Members
    }
}