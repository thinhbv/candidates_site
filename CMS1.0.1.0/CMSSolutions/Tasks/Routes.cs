using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Tasks.Controllers;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.Tasks
{
    [Feature(Constants.Areas.ScheduledTasks)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(ScheduleTaskController));
        }
    }

    public class FeatureProvider : IFeatureProvider
    {
        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
            {
                Id = Constants.Areas.ScheduledTasks,
                Name = "Schedule Tasks",
                Category = "Content"
            };
        }
    }
}