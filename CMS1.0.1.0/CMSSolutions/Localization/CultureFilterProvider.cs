using System.Globalization;
using System.Threading;
using System.Web.Mvc;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc.Filters;

namespace CMSSolutions.Localization
{
    [Feature(Constants.Areas.Localization)]
    public class CultureFilterProvider : FilterProvider, IActionFilter
    {
        private readonly IWorkContextAccessor workContextAccessor;

        public CultureFilterProvider(IWorkContextAccessor workContextAccessor)
        {
            this.workContextAccessor = workContextAccessor;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var workContext = workContextAccessor.GetContext();
            var cultureCode = workContext.CurrentCulture;

            if (!string.IsNullOrEmpty(cultureCode))
            {
                var culture = CultureInfo.GetCultureInfo(cultureCode);
                Thread.CurrentThread.CurrentCulture = culture;
            }
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
        }
    }
}
