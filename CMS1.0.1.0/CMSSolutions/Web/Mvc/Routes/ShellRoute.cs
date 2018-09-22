using System;
using System.Diagnostics;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Configuration;
using CMSSolutions.FileSystems.AppData;
using CMSSolutions.Web.Routing;

namespace CMSSolutions.Web.Mvc.Routes
{
    public class ShellRoute : RouteBase, IRouteWithArea
    {
        private readonly RouteBase route;
        private readonly ShellSettings shellSettings;
        private readonly IWorkContextAccessor workContextAccessor;
        private readonly IRunningShellTable runningShellTable;
        private readonly UrlPrefix urlPrefix;

        public ShellRoute(RouteBase route, ShellSettings shellSettings, IWorkContextAccessor workContextAccessor, IRunningShellTable runningShellTable)
        {
            this.route = route;
            this.shellSettings = shellSettings;
            this.runningShellTable = runningShellTable;
            this.workContextAccessor = workContextAccessor;
            if (!string.IsNullOrEmpty(this.shellSettings.RequestUrlPrefix))
                urlPrefix = new UrlPrefix(this.shellSettings.RequestUrlPrefix);

            Area = route.GetAreaName();
        }

        public SessionStateBehavior SessionState { get; set; }

        public string ShellSettingsName { get { return shellSettings.Name; } }

        public string Area { get; private set; }

        public bool IsHttpRoute { get; set; }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            // locate appropriate shell settings for request
            var settings = runningShellTable.Match(httpContext);

            // only proceed if there was a match, and it was for this client
            if (settings == null || settings.Name != shellSettings.Name)
                return null;

            var effectiveHttpContext = httpContext;
            if (urlPrefix != null)
                effectiveHttpContext = new UrlPrefixAdjustedHttpContext(httpContext, urlPrefix);

            var routeData = route.GetRouteData(effectiveHttpContext);
            if (routeData == null)
                return null;

            // push provided session state behavior to underlying MvcHandler
            effectiveHttpContext.SetSessionStateBehavior(SessionState);

            // otherwise wrap handler and return it
            routeData.RouteHandler = new RouteHandler(workContextAccessor, routeData.RouteHandler, SessionState);
            routeData.DataTokens["IWorkContextAccessor"] = workContextAccessor;

            if (IsHttpRoute)
            {
                routeData.Values["IWorkContextAccessor"] = workContextAccessor; // for WebApi
            }

            return routeData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            // locate appropriate shell settings for request
            var settings = runningShellTable.Match(requestContext.HttpContext);

            // only proceed if there was a match, and it was for this client
            if (settings == null || settings.Name != shellSettings.Name)
                return null;

            var effectiveRequestContext = requestContext;
            if (urlPrefix != null)
                effectiveRequestContext = new RequestContext(new UrlPrefixAdjustedHttpContext(requestContext.HttpContext, urlPrefix), requestContext.RouteData);

            var virtualPath = route.GetVirtualPath(effectiveRequestContext, values);
            if (virtualPath == null)
                return null;

            if (urlPrefix != null)
                virtualPath.VirtualPath = urlPrefix.PrependLeadingSegments(virtualPath.VirtualPath);

            return virtualPath;
        }

        private class RouteHandler : IRouteHandler
        {
            private readonly IWorkContextAccessor workContextAccessor;
            private readonly IRouteHandler routeHandler;
            private readonly SessionStateBehavior sessionStateBehavior;

            public RouteHandler(IWorkContextAccessor workContextAccessor, IRouteHandler routeHandler, SessionStateBehavior sessionStateBehavior)
            {
                this.workContextAccessor = workContextAccessor;
                this.routeHandler = routeHandler;
                this.sessionStateBehavior = sessionStateBehavior;
            }

            public IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                var httpHandler = routeHandler.GetHttpHandler(requestContext);
                requestContext.HttpContext.SetSessionStateBehavior(sessionStateBehavior);

                if (httpHandler is IHttpAsyncHandler)
                {
                    return new HttpAsyncHandler(workContextAccessor, (IHttpAsyncHandler)httpHandler);
                }
                return new HttpHandler(workContextAccessor, httpHandler);
            }
        }

        private class HttpHandler : IHttpHandler, IRequiresSessionState, IHasRequestContext
        {
            private readonly IWorkContextAccessor workContextAccessor;

            private readonly IHttpHandler httpHandler;

            public HttpHandler(IWorkContextAccessor workContextAccessor, IHttpHandler httpHandler)
            {
                this.workContextAccessor = workContextAccessor;
                this.httpHandler = httpHandler;
            }

            public bool IsReusable
            {
                get { return false; }
            }

            public void ProcessRequest(HttpContext context)
            {
                using (workContextAccessor.CreateWorkContextScope(new HttpContextWrapper(context)))
                {
                    httpHandler.ProcessRequest(context);
                }
            }

            public RequestContext RequestContext
            {
                get
                {
                    var mvcHandler = httpHandler as MvcHandler;
                    return mvcHandler == null ? null : mvcHandler.RequestContext;
                }
            }
        }

        private class HttpAsyncHandler : HttpHandler, IHttpAsyncHandler
        {
            private readonly IWorkContextAccessor workContextAccessor;
            private readonly IHttpAsyncHandler httpAsyncHandler;
            private IWorkContextScope scope;

            public HttpAsyncHandler(IWorkContextAccessor containerProvider, IHttpAsyncHandler httpAsyncHandler)
                : base(containerProvider, httpAsyncHandler)
            {
                workContextAccessor = containerProvider;
                this.httpAsyncHandler = httpAsyncHandler;
            }

            public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
            {
                scope = workContextAccessor.CreateWorkContextScope(new HttpContextWrapper(context));
                var shellSettings = scope.Resolve<ShellSettings>();

                if (shellSettings.State == TenantState.Offline)
                {
                    var appDataFolder = scope.Resolve<IAppDataFolder>();
                    var path = appDataFolder.Combine("Sites", shellSettings.Name, "App_Offline.htm");
                    if (appDataFolder.FileExists(path))
                    {
                        context.Response.WriteFile(appDataFolder.GetVirtualPath(path));
                    }
                    else
                    {
                        if (appDataFolder.FileExists("App_Offline.htm"))
                        {
                            context.Response.WriteFile(appDataFolder.GetVirtualPath("App_Offline.htm"));
                        }
                        else
                        {
                            context.Response.Write("Sorry, our site is undergoing routine maintenance. Please check back with us soon.");
                        }
                    }

                    var asyncResult = new OfflineAsyncResult();
                    return asyncResult;
                }

                try
                {
                    return httpAsyncHandler.BeginProcessRequest(context, cb, extraData);
                }
                catch
                {
                    scope.Dispose();
                    throw;
                }
            }

            [DebuggerStepThrough]
            public void EndProcessRequest(IAsyncResult result)
            {
                if (result is OfflineAsyncResult)
                {
                    scope.Dispose();
                    return;
                }

                try
                {
                    httpAsyncHandler.EndProcessRequest(result);
                }
                finally
                {
                    scope.Dispose();
                }
            }

            private class OfflineAsyncResult : IAsyncResult
            {
                private static readonly WaitHandle waitHandle = new ManualResetEvent(true/*initialState*/);

                public bool IsCompleted { get { return true; } }

                public WaitHandle AsyncWaitHandle { get { return waitHandle; } }

                public object AsyncState { get { return null; } }

                public bool CompletedSynchronously { get { return true; } }
            }
        }
    }
}