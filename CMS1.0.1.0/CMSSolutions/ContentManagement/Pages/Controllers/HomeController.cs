using System.Web.Mvc;
using CMSSolutions.ContentManagement.Pages.Services;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;

namespace CMSSolutions.ContentManagement.Pages.Controllers
{
    [Themed]
    [Feature(Constants.Areas.Pages)]
    public class HomeController : BaseController
    {
        private readonly IPageService pageService;
        private readonly ShellSettings shellSettings;
        private readonly SiteSettings siteSettings;
        private readonly IPageTagService pageTagService;

        public HomeController(IWorkContextAccessor workContextAccessor,
            IPageService pageService,
            ShellSettings shellSettings,
            SiteSettings siteSettings,
            IPageTagService pageTagService)
            : base(workContextAccessor)
        {
            this.pageService = pageService;
            this.shellSettings = shellSettings;
            this.siteSettings = siteSettings;
            this.pageTagService = pageTagService;
        }

        [Url("{*url}", Priority = -999)]
        public ActionResult PageContent()
        {
            // ReSharper disable PossibleNullReferenceException
            var slug = Request.Url.LocalPath.Trim('/');
            // ReSharper restore PossibleNullReferenceException

            if (!string.IsNullOrEmpty(shellSettings.RequestUrlPrefix) && slug.StartsWith(shellSettings.RequestUrlPrefix))
            {
                slug = slug.Substring(shellSettings.RequestUrlPrefix.Length);
            }

            var currentCulture = WorkContext.CurrentCulture;

            var page = pageService.GetPageBySlug(slug, currentCulture);

            if (page == null && siteSettings.DefaultLanguage != currentCulture)
            {
                page = pageService.GetPageBySlug(slug, siteSettings.DefaultLanguage);
            }

            if (page == null)
            {
                page = pageService.GetPageBySlug(slug, null);
            }

            if (page != null && page.IsEnabled)
            {
                WorkContext.SetState("CurrentPage", page);

                WorkContext.Breadcrumbs.Add(page.Title);

                if (!string.IsNullOrEmpty(page.Theme))
                {
                    var themeManager = WorkContext.Resolve<IThemeManager>();
                    var theme = themeManager.GetTheme(page.Theme);
                    if (theme != null)
                    {
                        WorkContext.CurrentTheme = theme;
                    }
                }

                var bodyContent = string.IsNullOrEmpty(page.CssClass)
                    ? string.Format("<article class=\"page-content\"><header><h1>{1}</h1></header><div class=\"article-content\">{0}</div></article>", page.BodyContent, page.Title)
                    : string.Format("<article class=\"page-content {2}\"><header><h1>{1}</h1></header><div class=\"article-content\">{0}</div></article>", page.BodyContent, page.Title, page.CssClass);

                // Replace tags
                var tags = pageTagService.GetRecords();
                if (tags.Count > 0)
                {
                    foreach (var tag in tags)
                    {
                        bodyContent = bodyContent.Replace("[%" + tag.Name + "%]", tag.Content);
                    }
                }

                return new ContentViewResult
                {
                    Title = page.Title,
                    MetaKeywords = page.MetaKeywords,
                    MetaDescription = page.MetaDescription,
                    BodyContent = bodyContent
                };
            }

            return HttpNotFound();
        }
    }
}