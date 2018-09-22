using System;
using System.Web;

namespace CMSSolutions.Web
{
    public class StaticFileModule : IHttpModule
    {
        public void Init(HttpApplication context)
        {
            context.EndRequest += ContextEndRequest;
        }

        private static void ContextEndRequest(object sender, EventArgs e)
        {
            var application = (HttpApplication)sender;
            var context = application.Context;
            var url = context.Request.Url.LocalPath.ToLowerInvariant();

            if (url.Contains("/scripts/") || url.Contains("/styles/"))
            {
                var cache = context.Response.Cache;
                cache.SetCacheability(HttpCacheability.Public);
                cache.SetMaxAge(new TimeSpan(7, 0, 0, 0));
                cache.SetValidUntilExpires(true);
            }
        }

        public void Dispose()
        {
        }
    }
}