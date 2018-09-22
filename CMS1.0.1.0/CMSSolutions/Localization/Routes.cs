using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Localization.Controllers;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.Localization
{
    [Feature(Constants.Areas.Localization)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(LocalizationController));
            MapAttributeRoutes(routes, typeof(LanguageController));
        }

        public class FeatureProvider : IFeatureProvider
        {
            public IEnumerable<FeatureDescriptor> AvailableFeatures()
            {
                yield return new FeatureDescriptor
                {
                    Id = Constants.Areas.Localization,
                    Name = "Localization",
                    Category = "Content"
                };
            }
        }
    }
}