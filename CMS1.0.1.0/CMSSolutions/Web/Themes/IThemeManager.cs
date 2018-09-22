using System.Web.Routing;
using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.Web.Themes
{
    public interface IThemeManager : IDependency
    {
        string[] GetInstalledThemes();

        ExtensionDescriptor GetRequestTheme(RequestContext requestContext);

        ExtensionDescriptor GetTheme(string theme);
    }
}