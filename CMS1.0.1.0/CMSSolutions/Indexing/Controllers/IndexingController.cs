using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Indexing.Services;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.Notify;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Indexing.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Indexing)]
    public class IndexingController : BaseController
    {
        private const string DefaultIndexName = "Search";
        private readonly IIndexingService indexingService;
        private readonly INotifier notifier;

        public IndexingController(IWorkContextAccessor workContextAccessor, IIndexingService indexingService, INotifier notifier)
            : base(workContextAccessor)
        {
            this.indexingService = indexingService;
            this.notifier = notifier;
        }

        [Url("{DashboardBaseUrl}/indexing")]
        public ActionResult Index()
        {
            if (!CheckPermission(StandardPermissions.FullAccess, T("Not allowed to manage the search index.")))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Indexing"));

            IndexEntry indexEntry;

            try
            {
                indexEntry = indexingService.GetIndexEntry(DefaultIndexName);

                if (indexEntry == null)
                {
                    notifier.Information(T("There is no search index to manage for this site."));
                }
            }
            catch (Exception e)
            {
                indexEntry = null;
                Logger.ErrorFormat(e, "Search index couldn't be read.");

                notifier.Information(T("The index might be corrupted. If you can't recover click on Rebuild."));
            }

            if (indexEntry == null)
            {
                notifier.Information(T("There is currently no search index."));
            }
            else if (indexEntry.LastUpdateUtc == DateTime.MinValue)
            {
                notifier.Information(T("The search index has not been built yet."));
            }
            else
            {
                if (indexEntry.DocumentCount == 0)
                {
                    notifier.Information(T("The search index does not contain any document."));
                }
                else
                {
                    notifier.Information(T("The search index contains {0} document(s).", indexEntry.DocumentCount));
                }

                if (indexEntry.Fields.Any())
                {
                    notifier.Information(T("The search index contains the following fields: {0}.", string.Join(", ", indexEntry.Fields)));
                }
                else
                {
                    notifier.Information(T("The search index does not contain any field."));
                }

                notifier.Information(T("The search index was last updated {0}.", indexEntry.LastUpdateUtc));

                switch (indexEntry.IndexingStatus)
                {
                    case IndexingStatus.Rebuilding:
                        notifier.Information(T("The indexing process is currently being rebuilt."));
                        break;

                    case IndexingStatus.Updating:
                        notifier.Information(T("The indexing process is currently being updated."));
                        break;
                }
            }

            var sb = new StringBuilder();

            if (indexEntry != null)
            {
                sb.AppendFormat("<form method=\"post\" action=\"{0}\">", Url.Action("Rebuild"));
                sb.AppendFormat("<div class=\"form-group\"><label>{0}</label><br />", T("Rebuild the search index for a fresh start:"));
                sb.AppendFormat("<button class=\"btn btn-primary\" type=\"submit\"><i class=\"cx-icon cx-icon-refresh\"></i>&nbsp;{0}</button></div>", T("Rebuild"));
                sb.Append("</form>");
            }

            var result = new ControlContentResult(sb.ToString())
            {
                Title = T("Search Index")
            };

            return result;
        }

        [HttpPost, Url("{DashboardBaseUrl}/indexing/rebuild")]
        public ActionResult Rebuild()
        {
            if (!CheckPermission(StandardPermissions.FullAccess, T("Not allowed to manage the search index.")))
            {
                return new HttpUnauthorizedResult();
            }

            indexingService.RebuildIndex(DefaultIndexName);

            return RedirectToAction("Index");
        }
    }
}