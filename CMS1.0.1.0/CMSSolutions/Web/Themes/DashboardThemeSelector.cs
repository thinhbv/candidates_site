using System.Web.Routing;

namespace CMSSolutions.Web.Themes
{
    public class DashboardThemeSelector : IThemeSelector
    {
        public ThemeSelectorResult GetTheme(RequestContext context)
        {
            ThemedAttribute attribute;
            if (ThemeFilter.IsApplied(context, out attribute) && attribute.IsDashboard)
            {
                return new ThemeSelectorResult { IsDashboard = attribute.IsDashboard, Priority = attribute.Priority, Name = "Dashboard" };
            }
            return null;
        }
    }
}