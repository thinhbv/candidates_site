using System.Web.Routing;
using System.Web.SessionState;

namespace CMSSolutions.Web.Mvc.Routes
{
    public class RouteDescriptor
    {
        public string Name { get; set; }

        public int Priority { get; set; }

        public RouteBase Route { get; set; }

        public SessionStateBehavior SessionState { get; set; }

        public override string ToString()
        {
            var route = Route as Route;
            if (route != null)
            {
                return route.Url;
            }
            return Route.ToString();
        }
    }

    public class HttpRouteDescriptor : RouteDescriptor
    {
        public string RouteTemplate { get; set; }

        public object Defaults { get; set; }

        public object Constraints { get; set; }
    }
}