using System.Web.Routing;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization.Services;
using CMSSolutions.Web.Themes;

namespace CMSSolutions.Localization
{
    [Feature(Constants.Areas.Localization)]
    public class LanguageThemeSelector : IThemeSelector
    {
        private readonly IWorkContextAccessor workContextAccessor;

        public LanguageThemeSelector(IWorkContextAccessor workContextAccessor)
        {
            this.workContextAccessor = workContextAccessor;
        }

        #region Implementation of IThemeSelector

        public ThemeSelectorResult GetTheme(RequestContext context)
        {
            var workContext = workContextAccessor.GetContext(context.HttpContext);
            var culture = workContext.CurrentCulture;

            if (!string.IsNullOrEmpty(culture))
            {
                var languageService = workContext.Resolve<ILanguageService>();
                var language = languageService.GetLanguage(culture);
                if (language != null && !string.IsNullOrEmpty(language.Theme))
                {
                    ThemedAttribute attribute;
                    if (ThemeFilter.IsApplied(context, out attribute) && attribute.IsDashboard)
                    {
                        return new ThemeSelectorResult { IsDashboard = attribute.IsDashboard, Priority = attribute.Priority, Name = language.Theme };
                    }

                    return null;
                }
            }

            return null;
        }

        #endregion Implementation of IThemeSelector
    }
}