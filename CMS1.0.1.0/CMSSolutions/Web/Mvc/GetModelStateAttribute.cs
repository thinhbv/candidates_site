using System.Web.Mvc;

namespace CMSSolutions.Web.Mvc
{
    public class GetModelStateAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var modelState = filterContext.Controller.TempData[PassModelStateAttribute.TempDataTransferKey] as ModelStateDictionary;

            if (modelState != null)
            {
                filterContext.Controller.ViewData.ModelState.Merge(modelState);
            }

            base.OnActionExecuting(filterContext);
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var modelState = filterContext.Controller.TempData[PassModelStateAttribute.TempDataTransferKey] as ModelStateDictionary;

            if (modelState != null)
            {
                if (!(filterContext.Result is ViewResult))
                {
                    //Otherwise remove it.
                    filterContext.Controller.TempData.Remove(PassModelStateAttribute.TempDataTransferKey);
                }
            }

            base.OnActionExecuted(filterContext);
        }
    }
}