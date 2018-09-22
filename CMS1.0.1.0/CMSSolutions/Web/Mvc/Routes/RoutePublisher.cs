using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.Web.Mvc.Routes
{
    public class RoutePublisher : IRoutePublisher
    {
        private readonly RouteCollection routeCollection;
        private readonly ShellSettings shellSettings;
        private readonly IWorkContextAccessor workContextAccessor;
        private readonly IRunningShellTable runningShellTable;
        private readonly IExtensionManager extensionManager;

        public RoutePublisher(
            RouteCollection routeCollection,
            ShellSettings shellSettings,
            IWorkContextAccessor workContextAccessor,
            IRunningShellTable runningShellTable,
            IExtensionManager extensionManager)
        {
            this.routeCollection = routeCollection;
            this.shellSettings = shellSettings;
            this.workContextAccessor = workContextAccessor;
            this.runningShellTable = runningShellTable;
            this.extensionManager = extensionManager;
        }

        public void Publish(IEnumerable<RouteDescriptor> routes)
        {
            var routesArray = routes
                .OrderByDescending(r => r.Priority)
                .ToArray();

            // this is not called often, but is intended to surface problems before
            // the actual collection is modified
            var preloading = new RouteCollection();
            foreach (var routeDescriptor in routesArray)
            {
                // extract the WebApi route implementation
                var httpRouteDescriptor = routeDescriptor as HttpRouteDescriptor;
                if (httpRouteDescriptor != null)
                {
                    var httpRouteCollection = new RouteCollection();
                    httpRouteCollection.MapHttpRoute(httpRouteDescriptor.Name, httpRouteDescriptor.RouteTemplate, httpRouteDescriptor.Defaults);
                    routeDescriptor.Route = httpRouteCollection.First();    
                }

                preloading.Add(routeDescriptor.Name, routeDescriptor.Route);
            }

            using (routeCollection.GetWriteLock())
            {
                // existing routes are removed while the collection is briefly inaccessable
                var cropArray = routeCollection
                    .OfType<ShellRoute>()
                    .Where(sr => sr.ShellSettingsName == shellSettings.Name)
                    .ToArray();

                foreach (var crop in cropArray)
                {
                    routeCollection.Remove(crop);
                }

                // new routes are added
                foreach (var routeDescriptor in routesArray)
                {
                    // Loading session state information.
                    var defaultSessionState = SessionStateBehavior.Default;

                    ExtensionDescriptor extensionDescriptor = null;
                    if (routeDescriptor.Route is Route)
                    {
                        object extensionId;
                        var route = routeDescriptor.Route as Route;
                        if (route.DataTokens != null && route.DataTokens.TryGetValue("area", out extensionId) ||
                           route.Defaults != null && route.Defaults.TryGetValue("area", out extensionId))
                        {
                            extensionDescriptor = extensionManager.GetExtension(extensionId.ToString());
                        }
                    }
                    else if (routeDescriptor.Route is IRouteWithArea)
                    {
                        var route = routeDescriptor.Route as IRouteWithArea;
                        extensionDescriptor = extensionManager.GetExtension(route.Area);
                    }

                    if (extensionDescriptor != null)
                    {
                        // if session state is not define explicitly, use the one define for the extension
                        if (routeDescriptor.SessionState == SessionStateBehavior.Default)
                        {
                            Enum.TryParse(extensionDescriptor.SessionState, true /*ignoreCase*/, out defaultSessionState);
                        }
                    }

                    // Route-level setting overrides module-level setting (from manifest).
                    var sessionStateBehavior = routeDescriptor.SessionState == SessionStateBehavior.Default ? defaultSessionState : routeDescriptor.SessionState;

                    var shellRoute = new ShellRoute(routeDescriptor.Route, shellSettings, workContextAccessor, runningShellTable)
                    {
                        IsHttpRoute = routeDescriptor is HttpRouteDescriptor,
                        SessionState = sessionStateBehavior
                    };
                    routeCollection.Add(routeDescriptor.Name, shellRoute);
                }
            }
        }
    }
}