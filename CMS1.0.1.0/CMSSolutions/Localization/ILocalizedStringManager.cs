using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Localization
{
    public interface ILocalizedStringManager : IDependency
    {
        string GetLocalizedString(string text, string cultureCode);
    }

    [Feature(Constants.Areas.Core)]
    public class LocalizedStringManager : ILocalizedStringManager
    {
        public string GetLocalizedString(string text, string cultureCode)
        {
            return text;
        }
    }
}