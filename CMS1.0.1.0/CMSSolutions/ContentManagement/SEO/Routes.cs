using System.Collections.Generic;
using CMSSolutions.ContentManagement.SEO.Controllers;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.ContentManagement.SEO
{
    [Feature(Constants.Areas.SEO)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(ControltsController));
            MapAttributeRoutes(routes, typeof(MetaTagController));
            MapAttributeRoutes(routes, typeof(SiteMapController));
        }
    }

    public class FeatureProvider : IFeatureProvider
    {
        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
            {
                Id = "MvcCMS.SEO",
                Name = "SEO",
                Category = "Content"
            };
        }
    }
}
