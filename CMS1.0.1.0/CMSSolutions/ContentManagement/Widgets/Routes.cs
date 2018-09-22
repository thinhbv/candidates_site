using System.Collections.Generic;
using CMSSolutions.ContentManagement.Widgets.Controllers;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.ContentManagement.Widgets
{
    [Feature(Constants.Areas.Widgets)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(WidgetController));
            MapAttributeRoutes(routes, typeof(ZoneController));
            MapAttributeRoutes(routes, typeof(FormWidgetController));
        }
    }

    public class FeatureProvider : IFeatureProvider
    {
        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
            {
                Id = Constants.Areas.Widgets,
                Name = "Widgets",
                Category = "Content"
            };
        }
    }
}