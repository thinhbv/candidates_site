using System.Web.Mvc;
using CMSSolutions.ContentManagement.Dashboard.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Homepage.Controllers
{
    [Themed]
    [Feature(Constants.Areas.Core)]
    public class HomepageController : Controller
    {
        private readonly SiteSettings siteSettings;

        public HomepageController(SiteSettings siteSettings)
        {
            this.siteSettings = siteSettings;
        }

        [Url(""), AllowAnonymous]
        public ActionResult DefaultHomePage()
        {
            ViewBag.Title = siteSettings.HomepageTitle;
            return new ControlContentResult("");
        }
    }
}
