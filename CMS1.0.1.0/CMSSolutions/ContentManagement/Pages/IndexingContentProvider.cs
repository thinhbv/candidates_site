using System;
using System.Collections.Generic;
using System.Globalization;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Pages.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Indexing;
using CMSSolutions.Indexing.Services;

namespace CMSSolutions.ContentManagement.Pages
{
    [Feature(Constants.Areas.Pages)]
    public class IndexingContentProvider : IIndexingContentProvider
    {
        private readonly IPageService pageService;
        private readonly UrlHelper urlHelper;
        private readonly IWorkContextAccessor workContextAccessor;

        public IndexingContentProvider(IPageService pageService, UrlHelper urlHelper, IWorkContextAccessor workContextAccessor)
        {
            this.pageService = pageService;
            this.urlHelper = urlHelper;
            this.workContextAccessor = workContextAccessor;
        }

        public IEnumerable<IDocumentIndex> GetDocuments(Func<string, IDocumentIndex> factory)
        {
            var pages = pageService.GetRecords();
            CultureInfo defaultCultureInfo = null;
            var workContext = workContextAccessor.GetContext();
            if (!string.IsNullOrEmpty(workContext.CurrentCulture))
            {
                try
                {
                    defaultCultureInfo = new CultureInfo(workContext.CurrentCulture);
                }
                catch (Exception)
                {
                    defaultCultureInfo = null;
                }
            }

            foreach (var page in pages)
            {
                var document = factory(page.Id.ToString());
                document.Add("title", page.Title).Analyze().Store();
                document.Add("meta_keywords", page.MetaKeywords).Analyze();
                document.Add("meta_description", page.MetaDescription).Analyze();
                document.Add("body", page.BodyContent).Analyze().Store();
                document.Add("description", page.BodyContent).Analyze().Store();
                document.Add("url", urlHelper.Action("PageContent", "Home", new { area = Constants.Areas.Pages, url = page.Slug })).Store();

                if (!string.IsNullOrEmpty(page.CultureCode))
                {
                    var cultureInfo = new CultureInfo(page.CultureCode);
                    document.Add("culture", cultureInfo.LCID).Store();
                }
                else
                {
                    if (defaultCultureInfo != null)
                    {
                        document.Add("culture", defaultCultureInfo.LCID).Store();
                    }
                }

                yield return document;
            }
        }
    }
}