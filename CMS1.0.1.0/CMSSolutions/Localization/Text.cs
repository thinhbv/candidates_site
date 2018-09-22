using System;
using System.Globalization;
using System.Linq;
using System.Web;

namespace CMSSolutions.Localization
{
    public class Text : IText
    {
        private readonly ILocalizedStringManager localizedStringManager;
        private readonly IWorkContextAccessor workContextAccessor;

        public Text(IWorkContextAccessor workContextAccessor,
                    ILocalizedStringManager localizedStringManager)
        {
            this.workContextAccessor = workContextAccessor;
            this.localizedStringManager = localizedStringManager;
        }

        #region IText Members

        public LocalizedString Get(string textHint, params object[] args)
        {
            var workContext = workContextAccessor.GetContext();
            var currentCulture = workContext.CurrentCulture;
            var localizedFormat = localizedStringManager.GetLocalizedString(textHint, currentCulture);

            if (string.IsNullOrEmpty(localizedFormat))
            {
                return new LocalizedString(textHint);
            }

            return args.Length == 0
                       ? new LocalizedString(localizedFormat, textHint, args)
                       : new LocalizedString(
                             string.Format(GetFormatProvider(currentCulture), localizedFormat,
                                           args.Select(Encode).ToArray()), textHint, args);
        }

        #endregion IText Members

        private static IFormatProvider GetFormatProvider(string currentCulture)
        {
            try
            {
                return CultureInfo.GetCultureInfoByIetfLanguageTag(currentCulture);
            }
            catch
            {
                return null;
            }
        }

        private static object Encode(object arg)
        {
            if (arg is IFormattable || arg is IHtmlString)
            {
                return arg;
            }
            return HttpUtility.HtmlEncode(arg);
        }
    }
}