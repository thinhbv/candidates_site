using System.Collections.Generic;
using CMSSolutions.ContentManagement.Pages.Controllers;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.ContentManagement.Pages
{
    [Feature(Constants.Areas.Pages)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(PageController));
            MapAttributeRoutes(routes, typeof(PageTagController));
            MapAttributeRoutes(routes, typeof(HomeController));
        }
    }

    public class FeatureProvider : IFeatureProvider
    {
        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
            {
                Id = Constants.Areas.Pages,
                Name = "Pages",
                Category = "Content",
                Dependencies = new[] { Constants.Areas.Menus, Constants.Areas.Localization }
            };
        }
    }
}