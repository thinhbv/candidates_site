using System;
using System.Web;
using System.Web.Routing;

namespace CMSSolutions.Web
{
    public class TransferHttpHandler : IHttpHandler
    {
        private readonly string targetUrl;

        public TransferHttpHandler(string targetUrl)
        {
            this.targetUrl = targetUrl;
        }

        public void ProcessRequest(HttpContext context)
        {
            string baseUrl = context.Request.Url.GetLeftPart(UriPartial.Authority);
            var httpContext = new InternalHttpContext(new Uri(new Uri(baseUrl), targetUrl), context.Request.ApplicationPath);
            var routeData = RouteTable.Routes.GetRouteData(httpContext);
        }

        public bool IsReusable
        {
            get { return false; }
        }

        private class InternalHttpContext : HttpContextBase
        {
            private readonly HttpRequestBase request;

            public InternalHttpContext(Uri uri, string applicationPath)
            {
                request = new InternalHttpRequest(uri, applicationPath);
            }

            public override HttpRequestBase Request { get { return request; } }
        }

        private class InternalHttpRequest : HttpRequestBase
        {
            private readonly string appRelativePath;
            private readonly string pathInfo;

            public InternalHttpRequest(Uri uri, string applicationPath)
            {
                if (string.IsNullOrEmpty(applicationPath) || !uri.AbsolutePath.StartsWith(applicationPath, StringComparison.OrdinalIgnoreCase))
                    appRelativePath = uri.AbsolutePath.Substring(applicationPath.Length);
                else
                    appRelativePath = uri.AbsolutePath;
            }

            public override string AppRelativeCurrentExecutionFilePath { get { return string.Concat("~", appRelativePath); } }

            public override string PathInfo { get { return pathInfo; } }
        }
    }
}