using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Routing;
using CMSSolutions.ContentManagement.Lists.Controllers;
using CMSSolutions.ContentManagement.Lists.Routing;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Web.Mvc.Routes;

namespace CMSSolutions.ContentManagement.Lists
{
    [Feature(Constants.Areas.Lists)]
    public class Routes : RouteProviderBase, IRouteProvider
    {
        private readonly IListPathConstraint pathConstraint;

        public Routes(IListPathConstraint pathConstraint)
        {
            this.pathConstraint = pathConstraint;
        }

        #region IRouteProvider Members

        public void GetRoutes(ICollection<RouteDescriptor> routes)
        {
            MapAttributeRoutes(routes, typeof(ListController));
            MapAttributeRoutes(routes, typeof(CategoryController));
            MapAttributeRoutes(routes, typeof(FieldController));
            MapAttributeRoutes(routes, typeof(ListItemController));
            MapAttributeRoutes(routes, typeof(CommentController));

            routes.Add(new RouteDescriptor
            {
                Route = new Route("{listSlug}/api/get-events",
                    new RouteValueDictionary
                    {
                        {"area", Constants.Areas.Lists},
                        {"controller", "Home"},
                        {"action", "GetEvents"}
                    },
                    new RouteValueDictionary
                    {
                        {"listSlug", pathConstraint},
                    },
                    new RouteValueDictionary
                    {
                        {"area", Constants.Areas.Lists}
                    },
                    new MvcRouteHandler())
            });

            routes.Add(new RouteDescriptor
            {
                Route = new Route("{listSlug}/load-more",
                    new RouteValueDictionary
                    {
                        {"area", Constants.Areas.Lists},
                        {"controller", "Home"},
                        {"action", "LoadMore"}
                    },
                    new RouteValueDictionary
                    {
                        {"listSlug", pathConstraint}
                    },
                    new RouteValueDictionary
                    {
                        {"area", Constants.Areas.Lists}
                    },
                    new MvcRouteHandler())
            });

            routes.Add(new RouteDescriptor
            {
                Route = new Route("{listSlug}/{pageIndex}",
                    new RouteValueDictionary
                    {
                        {"area", Constants.Areas.Lists},
                        {"controller", "Home"},
                        {"action", "Index"},
                        {"pageIndex", 1}
                    },
                    new RouteValueDictionary
                    {
                        {"listSlug", pathConstraint},
                        {"pageIndex", @"\d+"}
                    },
                    new RouteValueDictionary
                    {
                        {"area", Constants.Areas.Lists}
                    },
                    new MvcRouteHandler())
            });

            routes.Add(new RouteDescriptor
            {
                Route = new Route("{listSlug}/add-comment",
                    new RouteValueDictionary
                    {
                        {"area", Constants.Areas.Lists},
                        {"controller", "Home"},
                        {"action", "AddComment"}
                    },
                    new RouteValueDictionary
                    {
                        {"listSlug", pathConstraint},
                    },
                    new RouteValueDictionary
                    {
                        {"area", Constants.Areas.Lists}
                    },
                    new MvcRouteHandler())
            });

            routes.Add(new RouteDescriptor
            {
                Route = new Route("{listSlug}/{*pathInfo}",
                    new RouteValueDictionary
                    {
                        {"area", Constants.Areas.Lists},
                        {"controller", "Home"},
                        {"action", "Category"}
                    },
                    new RouteValueDictionary
                    {
                        {"listSlug", pathConstraint},
                    },
                    new RouteValueDictionary
                    {
                        {"area", Constants.Areas.Lists}
                    },
                    new MvcRouteHandler())
            });
        }

        #endregion IRouteProvider Members
    }

    public class FeatureProvider : IFeatureProvider
    {
        #region IFeatureProvider Members

        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
            {
                Id = Constants.Areas.Lists,
                Name = "Lists",
                Category = "Content"
            };
        }

        #endregion IFeatureProvider Members
    }
}