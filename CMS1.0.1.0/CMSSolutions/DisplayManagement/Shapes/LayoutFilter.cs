using System.Web.Mvc;
using CMSSolutions.Web.Mvc.Filters;
using CMSSolutions.Web.Themes;

namespace CMSSolutions.DisplayManagement.Shapes
{
    public class LayoutFilter : FilterProvider, IResultFilter
    {
        private readonly IWorkContextAccessor workContextAccessor;

        public LayoutFilter(IWorkContextAccessor workContextAccessor)
        {
            this.workContextAccessor = workContextAccessor;
        }

        #region Implementation of IResultFilter

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var workContext = workContextAccessor.GetContext();

            ThemedAttribute themedAttribute;
            if (ThemeFilter.IsApplied(filterContext.RequestContext, out themedAttribute))
            {
                if (themedAttribute.Minimal)
                {
                    workContext.Layout.Metadata.Alternates.Add("Layout__Minimal");
                }
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        #endregion Implementation of IResultFilter
    }
}