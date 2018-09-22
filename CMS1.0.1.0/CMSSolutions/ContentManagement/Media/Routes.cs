using System.Collections.Generic;
using CMSSolutions.ContentManagement.Media.Controllers;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.ContentManagement.Media
{
    [Feature(Constants.Areas.Media)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(MediaController));
            MapAttributeRoutes(routes, typeof(UploadFilesController));
        }
    }

    public class FeatureProvider : IFeatureProvider
    {
        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
            {
                Id = Constants.Areas.Media,
                Name = "Media",
                Category = "Content"
            };
        }
    }
}