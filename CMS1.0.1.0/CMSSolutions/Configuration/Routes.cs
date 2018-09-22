using System.Collections.Generic;
using CMSSolutions.Configuration.Controllers;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.Configuration
{
    [Feature(Constants.Areas.Core)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(SettingsController));
        }
    }
}
