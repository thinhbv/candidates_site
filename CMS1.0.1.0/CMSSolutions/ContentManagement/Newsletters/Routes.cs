using System.Collections.Generic;
using CMSSolutions.ContentManagement.Newsletters.Controllers;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.ContentManagement.Newsletters
{
    [Feature(Constants.Areas.Newsletters)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(NewsletterController));
            MapAttributeRoutes(routes, typeof(SubscriptionController));
        }
    }

    public class FeatureProvider : IFeatureProvider
    {
        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
            {
                Id = Constants.Areas.Newsletters,
                Name = "Newsletters",
                Category = "Content",
                Dependencies = new[] { Constants.Areas.Localization }
            };
        }
    }
}