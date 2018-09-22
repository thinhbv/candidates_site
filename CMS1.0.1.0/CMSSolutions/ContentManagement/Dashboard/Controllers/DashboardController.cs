using System.Web.Mvc;
using CMSSolutions.Caching;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Dashboard.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Dashboard)]
    public class DashboardController : BaseController
    {
        private readonly ICacheManager cacheManager;
        private readonly IStaticCacheManager staticCacheManager;

        public DashboardController(
            IWorkContextAccessor workContextAccessor,
            ICacheManager cacheManager,
            IStaticCacheManager staticCacheManager)
            : base(workContextAccessor)
        {
            this.cacheManager = cacheManager;
            this.staticCacheManager = staticCacheManager;
        }

        [Url("{DashboardBaseUrl}")]
        public ActionResult Index()
        {
            if (!CheckPermission(StandardPermissions.DashboardAccess, T("Can't access the dashboard panel.")))
            {
                return new HttpUnauthorizedResult();
            }

            ViewBag.Title = T("Dashboard");
            return new ControlContentResult("");
        }

        [Url("{DashboardBaseUrl}/reset-cache")]
        public virtual ActionResult ResetCache()
        {
            if (cacheManager != null)
            {
                cacheManager.Reset();
            }
            if (staticCacheManager != null)
            {
                staticCacheManager.Reset();
            }

            return RedirectToAction("Index");
        }
    }
}