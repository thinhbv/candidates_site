using System;
using System.Linq;
using System.Security.Principal;
using System.Web;
using System.Web.Security;

namespace CMSSolutions.Web.Security
{
    public class LazyGenericPrincipal : IPrincipal
    {
        private readonly HttpRequest httpRequest;
        private readonly IPrincipal principal;
        private string[] roles;

        public LazyGenericPrincipal(IPrincipal principal, HttpRequest httpRequest)
        {
            this.principal = principal;
            this.httpRequest = httpRequest;
        }

        #region IPrincipal Members

        public bool IsInRole(string role)
        {
            EnsureRoles();

            return roles.Any(name => name.Equals(role, StringComparison.OrdinalIgnoreCase));
        }

        public IIdentity Identity
        {
            get { return principal.Identity; }
        }

        #endregion IPrincipal Members

        private void EnsureRoles()
        {
            if (roles != null) return;

            HttpCookie authCookie = httpRequest.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie == null || authCookie.Value == "")
            {
                roles = new string[0];
                return;
            }

            FormsAuthenticationTicket authTicket;
            try
            {
                authTicket = FormsAuthentication.Decrypt(authCookie.Value);
            }
            catch
            {
                roles = new string[0];
                return;
            }

            if (string.IsNullOrEmpty(authTicket.UserData))
            {
                roles = new string[0];
                return;
            }

            roles = authTicket.UserData.Split(';');
        }
    }
}