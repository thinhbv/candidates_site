using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Routing;
using CMSSolutions.Collections;
using CMSSolutions.Configuration;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Web.Mvc.Routes
{
    public abstract class RouteProviderBase
    {
        public RouteDescriptor GetRouteDescriptor(string url, RouteValueDictionary defaults, RouteValueDictionary dataTokens, RouteValueDictionary constraints, int order)
        {
            return new RouteDescriptor
            {
                Priority = order,
                Route = new Route(url, defaults, constraints, dataTokens, new MvcRouteHandler())
            };
        }

        public void MapAttributeRoutes(ICollection<RouteDescriptor> routes, Assembly assembly, string defaultArea = null)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException("assembly");
            }

            // Find all non abstract classes of type Controller and whose names end with "Controller"
            assembly.GetTypes()
                .Where(x => x.IsClass && !x.IsAbstract && x.IsSubclassOf(typeof(Controller)) && x.Name.EndsWith("Controller"))
                .ForEach(x => MapAttributeRoutes(routes, x, defaultArea));
        }

        public void MapAttributeRoutes(ICollection<RouteDescriptor> routes, Type type, string defaultArea = null)
        {
            var dashboardBaseUrl = CMSConfigurationSection.Instance.Routing.DashboardBaseUrl;
            var baseUrl = type.GetCustomAttribute<UrlAttribute>();
            var feature = type.GetCustomAttribute<FeatureAttribute>();

            // Find all public methods from those controller class
            type.GetMethods().Select(x => new { Controller = type.Name, Method = x, type.Namespace })
                .SelectMany(x => x.Method.GetCustomAttributes(typeof(UrlAttribute), false),
                            (x, y) => new { Controller = x.Controller.Substring(0, x.Controller.Length - 10), Action = x.Method.Name, x.Method, x.Namespace, Route = (UrlAttribute)y })

                // Order selected entries by rank number and iterate through each of them
                .OrderBy(x => x.Route.Priority).ToList().ForEach(x =>
                {
                    if (x.Route.Url.Contains("{BaseUrl}"))
                    {
                        if (baseUrl == null || string.IsNullOrEmpty(baseUrl.BaseUrl))
                        {
                            throw new ArgumentException(string.Format("Cannot register route for {0}.{1}Controller type, must be set value for BaseUrl in UrlAttribute.", x.Namespace, x.Controller));
                        }
                        x.Route.Url = x.Route.Url.Replace("{BaseUrl}", baseUrl.BaseUrl);
                    }

                    // Replace DashboardBaseUrl
                    x.Route.Url = x.Route.Url.Replace("{DashboardBaseUrl}", dashboardBaseUrl).TrimStart('/');

                    var area = feature != null ? feature.Name : defaultArea ?? "";

                    var split = x.Namespace.Split('.').ToList();
                    var indexOfControllers = split.IndexOf("Controllers");
                    if (indexOfControllers < split.Count - 1)
                    {
                        area = (area + "." + string.Join(".", split.ToArray(), indexOfControllers + 1, split.Count - indexOfControllers - 1)).Trim('.');
                    }

                    // Set Defautls
                    var defaults = ParseRouteValues(x.Route.Defaults);
                    defaults.Add("area", area);
                    defaults.Add("controller", x.Controller);

                    var attribute = (ActionNameAttribute)x.Method.GetCustomAttributes(typeof(ActionNameAttribute), true).FirstOrDefault();
                    defaults.Add("action", attribute != null ? attribute.Name : x.Action);

                    // Set Optional Parameters and remove '?' mark from the url
                    Match m;

                    // ReSharper disable ConditionIsAlwaysTrueOrFalse
                    while ((m = Regex.Match(x.Route.Url, @"\{([^\}]+?)\?\}")) != null && m.Success)
                    // ReSharper restore ConditionIsAlwaysTrueOrFalse
                    {
                        var p = m.Groups[1].Value;
                        defaults.Add(p, UrlParameter.Optional);
                        x.Route.Url = x.Route.Url.Replace("{" + p + "?}", "{" + p + "}");
                    }

                    // Set Defautls
                    var constraints = ParseRouteValues(x.Route.Constraints);

                    // Set Data Tokens
                    var dataTokens = new RouteValueDictionary
                                         {
                                             {"area", area}
                                         };

                    routes.Add(GetRouteDescriptor(x.Route.Url, defaults, dataTokens, constraints, x.Route.Priority));
                });
        }

        /// <summary>
        /// Parse route values string
        /// </summary>
        /// <param name="values">Route values string. Ex.: id=[0-9]+;name=[a-z]+;category=books</param>
        /// <returns><see cref="System.Web.Routing.RouteValueDictionary"/></returns>
        protected static RouteValueDictionary ParseRouteValues(string values)
        {
            var routeValues = new RouteValueDictionary();

            if (String.IsNullOrEmpty(values))
            {
                return routeValues;
            }

            foreach (var value in values.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries))
            {
                var args = value.Split('=');
                if (args.Length == 2)
                {
                    routeValues.Add(args[0], args[1]);
                }
            }

            return routeValues;
        }
    }
}