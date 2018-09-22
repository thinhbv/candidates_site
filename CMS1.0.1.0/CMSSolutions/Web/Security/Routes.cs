using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Web.Mvc.Routes;
using CMSSolutions.Web.Security.Controllers;

namespace CMSSolutions.Web.Security
{
    [Feature(Constants.Areas.Security)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(UserController));
            MapAttributeRoutes(routes, typeof(RoleController));
        }
    }

    public class FeatureProvider : IFeatureProvider
    {
        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
            {
                Id = Constants.Areas.Security,
                Name = "Security",
                Category = "Core"
            };
        }
    }
}