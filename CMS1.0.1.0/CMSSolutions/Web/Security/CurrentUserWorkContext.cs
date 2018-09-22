using System;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Security;
using CMSSolutions.Web.Security.Services;

namespace CMSSolutions.Web.Security
{
    [Feature(Constants.Areas.Security)]
    public class CurrentUserWorkContext : IWorkContextStateProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IMembershipService membershipService;

        public CurrentUserWorkContext(IMembershipService membershipService, IHttpContextAccessor httpContextAccessor)
        {
            this.membershipService = membershipService;
            this.httpContextAccessor = httpContextAccessor;
        }

        public Func<WorkContext, T> Get<T>(string name)
        {
            if (name == "CurrentUser")
            {
                var httpContext = httpContextAccessor.Current();
                if (httpContext.User.Identity.IsAuthenticated)
                {
                    var user = membershipService.GetUser(httpContext.User.Identity.Name);
                    if (user == null)
                    {
                        throw new NotAuthorizedException();
                    }
                    return ctx => (T)(object)user;
                }
            }
            return null;
        }
    }
}