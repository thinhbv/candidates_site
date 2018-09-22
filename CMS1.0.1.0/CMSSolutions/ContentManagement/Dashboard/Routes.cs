using System.Collections.Generic;
using CMSSolutions.ContentManagement.Dashboard.Controllers;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.ContentManagement.Dashboard
{
    [Feature(Constants.Areas.Dashboard)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            //MapAttributeRoutes(routes, typeof(DashboardController));
            MapAttributeRoutes(routes, typeof(ModuleController));
            MapAttributeRoutes(routes, typeof(ThemeController));
            MapAttributeRoutes(routes, typeof(ModuleSettingsController));
        }
    }
}