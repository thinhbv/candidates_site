using System.Web;

namespace CMSSolutions.Localization
{
    public interface ICultureManager : IDependency
    {
        string GetCurrentCulture(HttpContextBase requestContext);

        bool IsValidCulture(string cultureName);
    }
}