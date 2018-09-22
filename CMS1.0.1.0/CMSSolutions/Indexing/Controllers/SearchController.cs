using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using CMSSolutions.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Indexing.Models;
using CMSSolutions.Indexing.Services;
using CMSSolutions.Logging;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.Navigation;
using CMSSolutions.Web.UI.Notify;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Indexing.Controllers
{
    [Authorize]
    [Themed]
    [Feature(Constants.Areas.Indexing)]
    public class SearchController : BaseController
    {
        private readonly INotifier notifier;
        private readonly ISearchService searchService;
        private readonly SearchSettings searchSettings;

        public SearchController(IWorkContextAccessor workContextAccessor, INotifier notifier, ISearchService searchService, SearchSettings searchSettings)
            : base(workContextAccessor)
        {
            this.notifier = notifier;
            this.searchService = searchService;
            this.searchSettings = searchSettings;
        }

        [Url("search")]
        public ActionResult Search(PagerParameters pagerParameters, string q = "")
        {
            q = q.Trim();

            if (string.IsNullOrEmpty(q))
            {
                return Redirect(Request.UrlReferrer != null ? Request.UrlReferrer.ToString() : Url.Content("~/"));
            }

            PagedList<ISearchHit> searchHits;

            try
            {
                searchHits = searchService.Query(q, 0, 20, searchSettings.SearchedFields, WorkContext.CurrentCulture, searchHit => searchHit);
            }
            catch (Exception exception)
            {
                Logger.Error(T("Invalid search query: {0}", exception.Message).Text);
                notifier.Error(T("Invalid search query: {0}", exception.Message));
                searchHits = new PagedList<ISearchHit>(new ISearchHit[] { });
            }

            var sb = new StringBuilder();
            if (searchHits.ItemCount == 0)
            {
                notifier.Information(T("Your search - {0} - did not match any documents.", new HtmlString("<strong>" + q + "</strong>")));
            }
            else
            {
                notifier.Information(T("Your search - {0} - resulted in {1} documents.", new HtmlString("<strong>" + q + "</strong>"), searchHits.ItemCount));

                sb.Append("<ul class=\"thumbnails search-results\">");
                foreach (var searchHit in searchHits)
                {
                    sb.Append("<li>");

                    sb.AppendFormat("<a href=\"{1}\">{0}</a>", searchHit.GetString("title"), searchHit.GetString("url"));
                    sb.AppendFormat("<div class=\"description\">{0}</div>", searchHit.GetString("description") ?? searchHit.GetString("body"));

                    sb.Append("</li>");
                }
                sb.Append("</ul>");
            }

            var result = new ControlContentResult(sb.ToString())
            {
                Title = T("Search")
            };

            return result;
        }
    }
}