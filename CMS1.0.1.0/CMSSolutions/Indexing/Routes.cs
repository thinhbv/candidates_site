using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Indexing.Controllers;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.Indexing
{
    [Feature(Constants.Areas.Indexing)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(IndexingController));
            MapAttributeRoutes(routes, typeof(SearchController));
        }
    }

    public class FeatureProvider : IFeatureProvider
    {
        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
            {
                Id = Constants.Areas.Indexing,
                Name = "Indexing",
                Category = "Search"
            };
        }
    }
}