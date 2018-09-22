using System;
using System.Web;
using System.Web.Caching;
using System.Web.Hosting;
using System.Web.Mvc;

namespace CMSSolutions.Web.Mvc.ViewEngines
{
    public class ThemeViewLocationCache : IViewLocationCache
    {
        private readonly string requestTheme;

        public ThemeViewLocationCache(string requestTheme)
        {
            this.requestTheme = requestTheme;
        }

        private string AlterKey(string key)
        {
            return key + ":" + requestTheme;
        }

        public string GetViewLocation(HttpContextBase httpContext, string key)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            return (string)httpContext.Cache[AlterKey(key)];
        }

        public void InsertViewLocation(HttpContextBase httpContext, string key, string virtualPath)
        {
            if (httpContext == null)
            {
                throw new ArgumentNullException("httpContext");
            }

            httpContext.Cache.Insert(AlterKey(key), virtualPath, new CacheDependency(HostingEnvironment.MapPath("~/Themes")));
        }
    }
}