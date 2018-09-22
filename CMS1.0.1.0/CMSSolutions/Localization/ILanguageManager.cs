using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Localization
{
    public interface ILanguageManager : IDependency
    {
        IList<Language> GetActiveLanguages(string themeLanguages, bool all);
    }

    [Feature(Constants.Areas.Core)]
    public class DefaultLanguageManager : ILanguageManager
    {
        #region ILanguageManager Members

        public IList<Language> GetActiveLanguages(string themeLanguages, bool all)
        {
            return new List<Language>
            {
                new Language
                {
                    Name = "English",
                    CultureCode = "en-US"
                }
            };
        }

        #endregion
    }
}