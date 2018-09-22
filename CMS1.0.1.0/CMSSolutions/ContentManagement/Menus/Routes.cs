using System.Collections.Generic;
using CMSSolutions.ContentManagement.Menus.Controllers;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.ContentManagement.Menus
{
    [Feature(Constants.Areas.Menus)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(MenuController));
        }
    }
}