using System.Collections.Generic;
using CMSSolutions.ContentManagement.Homepage.Controllers;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.ContentManagement.Homepage
{
    [Feature(Constants.Areas.Core)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(HomepageController));
        }
    }
}
