using System.Text;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.SEO.Models;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;

namespace CMSSolutions.ContentManagement.SEO.Controllers
{
    [Authorize]
    [Feature(Constants.Areas.SEO)]
    public class ControltsController : Controller
    {
        private readonly ControltsSettings controltsSettings;

        public ControltsController(ControltsSettings controltsSettings)
        {
            this.controltsSettings = controltsSettings;
        }

        [Url("controlts.txt")]
        public ActionResult Index()
        {
            return Content(controltsSettings.Content, "text/plain", Encoding.UTF8);
        }
    }
}
