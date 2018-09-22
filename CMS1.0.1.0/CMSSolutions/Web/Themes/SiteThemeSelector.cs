using System.Web.Routing;

namespace CMSSolutions.Web.Themes
{
    public class SiteThemeSelector : IThemeSelector
    {
        private readonly SiteSettings siteSettings;

        public SiteThemeSelector(SiteSettings siteSettings)
        {
            this.siteSettings = siteSettings;
        }

        #region IThemeSelector Members

        public ThemeSelectorResult GetTheme(RequestContext context)
        {
            ThemedAttribute attribute;
            if (ThemeFilter.IsApplied(context, out attribute) && !attribute.IsDashboard)
            {
                return new ThemeSelectorResult { IsDashboard = attribute.IsDashboard, Priority = attribute.Priority, Name = siteSettings.Theme ?? "Default" };
            }

            return null;
        }

        #endregion IThemeSelector Members
    }
}