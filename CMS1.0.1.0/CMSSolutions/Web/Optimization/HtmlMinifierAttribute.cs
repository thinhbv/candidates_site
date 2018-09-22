using System.Web;
using System.Web.Mvc;

namespace CMSSolutions.Web.Optimization
{
    public class HtmlMinifierAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpResponseBase response = filterContext.HttpContext.Response;
            response.Filter = new HtmlMinifierFilter(response.Filter);
        }
    }
}