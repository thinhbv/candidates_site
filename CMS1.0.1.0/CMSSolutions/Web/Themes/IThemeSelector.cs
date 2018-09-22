using System.Web.Routing;

namespace CMSSolutions.Web.Themes
{
    public interface IThemeSelector : IDependency
    {
        ThemeSelectorResult GetTheme(RequestContext context);
    }
}