using System.Collections.Generic;

namespace CMSSolutions.Web.Mvc.Routes
{
    public interface IRouteProvider : IDependency
    {
        void GetRoutes(ICollection<RouteDescriptor> routes);
    }
}