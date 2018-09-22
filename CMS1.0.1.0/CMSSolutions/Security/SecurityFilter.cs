using System.Web.Mvc;
using CMSSolutions.Web.Mvc.Filters;

namespace CMSSolutions.Security
{
    public class SecurityFilter : FilterProvider, IExceptionFilter
    {
        public void OnException(ExceptionContext filterContext)
        {
            if (!(filterContext.Exception is NotAuthorizedException))
                return;

            filterContext.Result = new HttpUnauthorizedResult();
            filterContext.ExceptionHandled = true;
        }
    }
}
