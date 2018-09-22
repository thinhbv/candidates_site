using System.Collections.Generic;
using CMSSolutions.ContentManagement.Messages.Controllers;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.ContentManagement.Messages
{
    [Feature(Constants.Areas.Messages)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(QueuedEmailController));
            MapAttributeRoutes(routes, typeof(QueuedSmsController));
            MapAttributeRoutes(routes, typeof(MessageTemplateController));
        }
    }

    public class FeatureProvider : IFeatureProvider
    {
        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
            {
                Id = Constants.Areas.Messages,
                Name = "Messages",
                Category = "Messages"
            };
        }
    }
}