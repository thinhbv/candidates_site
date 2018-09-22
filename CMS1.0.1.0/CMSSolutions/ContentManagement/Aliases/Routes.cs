using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Aliases.Controllers;
using CMSSolutions.ContentManagement.Aliases.Implementation;
using CMSSolutions.ContentManagement.Aliases.Implementation.Holder;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Environment.ShellBuilders.Models;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.ContentManagement.Aliases
{
    [Feature(Constants.Areas.Aliases)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        private readonly ShellBlueprint blueprint;
        private readonly IAliasHolder aliasHolder;

        public Routes(ShellBlueprint blueprint, IAliasHolder aliasHolder)
        {
            this.blueprint = blueprint;
            this.aliasHolder = aliasHolder;
        }

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(AliasController));

            foreach (var routeDescriptor in GetRoutes())
            {
                routes.Add(routeDescriptor);
            }
        }

        public IEnumerable<RouteDescriptor> GetRoutes()
        {
            var distinctAreaNames = blueprint.Controllers
                .Select(controllerBlueprint => controllerBlueprint.AreaName)
                .Distinct();

            return distinctAreaNames.Select(areaName =>
                new RouteDescriptor
                {
                    Priority = 80,
                    Route = new AliasRoute(aliasHolder, areaName, new MvcRouteHandler())
                }).ToList();
        }
    }

    public class FeatureProvider : IFeatureProvider
    {
        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
                             {
                                 Id = Constants.Areas.Aliases,
                                 Name = "Alias",
                                 Category = "Content"
                             };
        }
    }
}