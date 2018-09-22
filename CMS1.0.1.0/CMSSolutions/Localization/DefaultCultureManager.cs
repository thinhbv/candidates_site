using System.Text.RegularExpressions;
using System.Web;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization.Services;
using CMSSolutions.Web;

namespace CMSSolutions.Localization
{
    [Feature(Constants.Areas.Localization)]
    public class DefaultCultureManager : ICultureManager
    {
        private readonly IWorkContextAccessor workContextAccessor;

        public DefaultCultureManager(IWorkContextAccessor workContextAccessor)
        {
            this.workContextAccessor = workContextAccessor;
        }

        #region ICultureManager Members

        public virtual string GetCurrentCulture(HttpContextBase requestContext)
        {
            if (requestContext.Items.Contains("CachedCurrentCulture"))
            {
                return requestContext.Items["CachedCurrentCulture"].ToString();
            }

            var workContext = workContextAccessor.GetContext(requestContext);
            var languageService = workContext.Resolve<ILanguageService>();
            string cultureCode = "en-US";

            var cookie = requestContext.Request.Cookies["CurrentCulture"];
            if (cookie != null && !string.IsNullOrEmpty(cookie.Value))
            {
                var language = languageService.GetLanguage(cookie.Value);
                if (language != null)
                {
                    cultureCode = language.CultureCode;
                    requestContext.Items["CachedCurrentCulture"] = cultureCode;
                    return cultureCode;
                }
            }

            var siteSettings = workContext.Resolve<SiteSettings>();
            if (!string.IsNullOrEmpty(siteSettings.DefaultLanguage))
            {
                var language = languageService.GetLanguage(siteSettings.DefaultLanguage);
                if (language != null)
                {
                    cultureCode = language.CultureCode;
                }
            }

            requestContext.Items["CachedCurrentCulture"] = cultureCode;
            return cultureCode;
        }

        public bool IsValidCulture(string cultureName)
        {
            var cultureRegex = new Regex(@"\w{2}(-\w{2,})*");
            if (cultureRegex.IsMatch(cultureName))
            {
                return true;
            }
            return false;
        }

        #endregion ICultureManager Members
    }
}