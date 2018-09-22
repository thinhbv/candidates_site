using System.Collections.Generic;

namespace CMSSolutions.Web.Mvc.Routes
{
    public interface IRoutePublisher : IDependency
    {
        void Publish(IEnumerable<RouteDescriptor> routes);
    }
}