using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Pages.Services;
using CMSSolutions.ContentManagement.SEO.SiteMaps;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;

namespace CMSSolutions.ContentManagement.Pages
{
    [Feature(Constants.Areas.Pages)]
    public class SitemapProvider : ISitemapProvider
    {
        private readonly IPageService pageService;

        public SitemapProvider(IPageService pageService)
        {
            this.pageService = pageService;
        }

        public IEnumerable<SitemapUrl> GetSitemapUrls(UrlHelper urlHelper)
        {
            var pages = pageService.GetRecords(x => x.RefId == null && x.IsEnabled);
            return pages.Select(page => new SitemapUrl(page.Title, urlHelper.AbsoluteContent("~/" + page.Slug))
            {
                ChangeFrequency = ChangeFrequency.Weekly,
                Priority = 0.8f
            });
        }
    }
}