using System.Web.Mvc;
using CMSSolutions.Configuration;

namespace CMSSolutions.Web.Security
{
    public interface IAuthenticationClientProvider : ISettings
    {
        string ProviderName { get; }

        bool IsValid();

        string GetLoginUrl(UrlHelper urlHelper, string returnUrl);
    }
}
