using System;
using CMSSolutions.Environment;

namespace CMSSolutions.Web
{
    public class HttpContextWorkContext : IWorkContextStateProvider
    {
        private readonly IHttpContextAccessor httpContextAccessor;

        public HttpContextWorkContext(IHttpContextAccessor httpContextAccessor)
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        #region IWorkContextStateProvider Members

        public Func<WorkContext, T> Get<T>(string name)
        {
            if (name == "HttpContext")
            {
                var result = (T)(object)httpContextAccessor.Current();
                return ctx => result;
            }
            return null;
        }

        #endregion IWorkContextStateProvider Members
    }
}