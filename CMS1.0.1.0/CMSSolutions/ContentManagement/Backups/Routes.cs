using System.Collections.Generic;
using CMSSolutions.ContentManagement.Backups.Controllers;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.ContentManagement.Backups
{
    [Feature(Constants.Areas.Backups)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(BackupController));
        }
    }
}