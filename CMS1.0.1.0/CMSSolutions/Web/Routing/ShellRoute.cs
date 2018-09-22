using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;

namespace VortexSoft.MvcCornerstone.Web.Routing
{
    internal class ShellRoute : RouteBase, IRouteWithArea
    {
        private readonly RouteBase baseRoute;
        private readonly IWorkContextAccessor workContextAccessor;
        private readonly IEnumerable<string> namespaces;
        private readonly string url;

        public ShellRoute(RouteBase baseRoute, IWorkContextAccessor workContextAccessor)
        {
            this.baseRoute = baseRoute;
            this.workContextAccessor = workContextAccessor;
            Area = baseRoute.GetAreaName();

            var route = baseRoute as Route;
            if (route != null)
            {
                url = route.Url;
                if (route.DataTokens != null && route.DataTokens.ContainsKey("namespaces"))
                {
                    namespaces = (IEnumerable<string>)route.DataTokens["namespaces"];
                }
            }
        }

        public float Priority { get; set; }

        public override RouteData GetRouteData(HttpContextBase httpContext)
        {
            var routeData = baseRoute.GetRouteData(httpContext);
            if (routeData == null)
            {
                return null;
            }

            routeData.RouteHandler = new RouteHandler(workContextAccessor, routeData.RouteHandler);
            routeData.DataTokens["IWorkContextAccessor"] = workContextAccessor;

            if (namespaces != null)
            {
                routeData.DataTokens["namespaces"] = namespaces;
                routeData.Values["namespaces"] = namespaces;
            }

            return routeData;
        }

        public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
        {
            if (values.ContainsKey("namespaces"))
            {
                var ns = (IEnumerable<string>)values["namespaces"];
                var routeValues = new RouteValueDictionary(values);
                routeValues.Remove("namespaces");

                var virtualPathData = baseRoute.GetVirtualPath(requestContext, routeValues);
                if (virtualPathData != null)
                {
                    var namespacesOfVirtualPathData = virtualPathData.DataTokens["namespaces"] as IEnumerable<string>;
                    if (namespacesOfVirtualPathData == null)
                    {
                        return null;
                    }

                    return namespacesOfVirtualPathData.ToList().SequenceEqual(ns) ? virtualPathData : null;
                }

                return null;
            }

            return baseRoute.GetVirtualPath(requestContext, values);
        }

        private class RouteHandler : IRouteHandler
        {
            private readonly IWorkContextAccessor workContextAccessor;
            private readonly IRouteHandler routeHandler;

            public RouteHandler(IWorkContextAccessor workContextAccessor, IRouteHandler routeHandler)
            {
                this.workContextAccessor = workContextAccessor;
                this.routeHandler = routeHandler;
            }

            public IHttpHandler GetHttpHandler(RequestContext requestContext)
            {
                var httpHandler = routeHandler.GetHttpHandler(requestContext);
                var httpAsyncHandler = httpHandler as IHttpAsyncHandler;
                if (httpAsyncHandler != null)
                {
                    return new HttpAsyncHandler(workContextAccessor, httpAsyncHandler);
                }
                return new HttpHandler(workContextAccessor, httpHandler);
            }
        }

        private class HttpHandler : IHttpHandler, IRequiresSessionState, IHasRequestContext
        {
            protected readonly IWorkContextAccessor WorkContextAccessor;
            private readonly IHttpHandler httpHandler;

            public HttpHandler(IWorkContextAccessor workContextAccessor, IHttpHandler httpHandler)
            {
                WorkContextAccessor = workContextAccessor;
                this.httpHandler = httpHandler;
            }

            public bool IsReusable
            {
                get { return false; }
            }

            public void ProcessRequest(HttpContext context)
            {
                using (WorkContextAccessor.CreateWorkContextScope(new HttpContextWrapper(context)))
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
            private readonly IHttpAsyncHandler httpAsyncHandler;
            private IDisposable scope;

            public HttpAsyncHandler(IWorkContextAccessor containerProvider, IHttpAsyncHandler httpAsyncHandler)
                : base(containerProvider, httpAsyncHandler)
            {
                this.httpAsyncHandler = httpAsyncHandler;
            }

            public IAsyncResult BeginProcessRequest(HttpContext context, AsyncCallback cb, object extraData)
            {
                scope = WorkContextAccessor.CreateWorkContextScope(new HttpContextWrapper(context));
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
                try
                {
                    httpAsyncHandler.EndProcessRequest(result);
                }
                finally
                {
                    scope.Dispose();
                }
            }
        }

        public string Area { get; private set; }

        public override string ToString()
        {
            return url;
        }
    }
}