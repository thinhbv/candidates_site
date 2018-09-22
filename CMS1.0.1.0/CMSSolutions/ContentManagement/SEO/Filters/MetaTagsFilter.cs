using System.Web.Mvc;
using CMSSolutions.ContentManagement.SEO.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc.Filters;
using CMSSolutions.Web.UI;

namespace CMSSolutions.ContentManagement.SEO.Filters
{
    [Feature(Constants.Areas.SEO)]
    public class MetaTagsFilter : FilterProvider, IResultFilter
    {
        private readonly IResourcesManager resourcesManager;
        private readonly IMetaTagService metaTagService;

        public MetaTagsFilter(IResourcesManager resourcesManager, IMetaTagService metaTagService)
        {
            this.resourcesManager = resourcesManager;
            this.metaTagService = metaTagService;
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult))
            {
                return;
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                return;
            }

            var metaTags = metaTagService.GetRecords();

            foreach (var metaTag in metaTags)
            {
                resourcesManager.AppendMeta(new MetaEntry
                                                {
                                                    Name = metaTag.Name,
                                                    Content = metaTag.Content,
                                                    Charset = metaTag.Charset
                                                }, ", ");
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}
