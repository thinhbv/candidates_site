using System.Web;

namespace CMSSolutions.Environment
{
    public interface IHttpContextAccessor : ISingletonDependency
    {
        HttpContextBase Current();
    }

    public class HttpContextAccessor : IHttpContextAccessor
    {
        public HttpContextBase Current()
        {
            var httpContext = GetStaticProperty();
            if (httpContext == null)
                return null;
            return new HttpContextWrapper(httpContext);
        }

        private static HttpContext GetStaticProperty()
        {
            return HttpContext.Current;
        }
    }
}