using System.Web.Mvc;

namespace CMSSolutions.Web.Mvc
{
    public class PassModelStateAttribute : ActionFilterAttribute
    {
        public const string TempDataTransferKey = "__CMSSolutions.Web.Mvc.TempDataTransferKey";

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            //Only export when ModelState is not valid
            if (!filterContext.Controller.ViewData.ModelState.IsValid)
            {
                //Export if we are redirecting
                if ((filterContext.Result is RedirectResult) || (filterContext.Result is RedirectToRouteResult))
                {
                    filterContext.Controller.TempData[TempDataTransferKey] = filterContext.Controller.ViewData.ModelState;
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}