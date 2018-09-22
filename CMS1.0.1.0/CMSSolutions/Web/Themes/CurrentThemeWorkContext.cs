using System;
using CMSSolutions.Environment;

namespace CMSSolutions.Web.Themes
{
    public class CurrentThemeWorkContext : IWorkContextStateProvider
    {
        private readonly IThemeManager themeManager;
        private readonly IHttpContextAccessor httpContextAccessor;

        public CurrentThemeWorkContext(IThemeManager themeManager, IHttpContextAccessor httpContextAccessor)
        {
            this.themeManager = themeManager;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Func<WorkContext, T> Get<T>(string name)
        {
            if (name == "CurrentTheme")
            {
                var currentTheme = themeManager.GetRequestTheme(httpContextAccessor.Current().Request.RequestContext);
                return ctx => (T)(object)currentTheme;
            }
            return null;
        }
    }
}