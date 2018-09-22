using System.Collections.Generic;
using System.Web.Mvc;

namespace CMSSolutions.Web.Mvc.Filters
{
    public class FilterResolvingActionInvoker : ControllerActionInvoker
    {
        private readonly IEnumerable<IFilterProvider> filterProviders;

        public FilterResolvingActionInvoker(IEnumerable<IFilterProvider> filterProviders)
        {
            this.filterProviders = filterProviders;
        }

        protected override FilterInfo GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            var filters = base.GetFilters(controllerContext, actionDescriptor);
            foreach (var provider in filterProviders)
            {
                provider.AddFilters(filters);
            }
            return filters;
        }
    }
}