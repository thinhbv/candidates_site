using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;
using CMSSolutions.Websites.Entities;
using CMSSolutions.Websites.Models;
using CMSSolutions.Websites.Services;
using CMSSolutions.Web;
using CMSSolutions.Web.UI.Navigation;
using CMSSolutions.Web.Routing;
    

namespace CMSSolutions.Websites.Controllers
{
	[Authorize()]
	[Themed(IsDashboard = true)]
	public class SyncDataController : BaseAdminController
    {
		public SyncDataController(IWorkContextAccessor workContextAccessor) : 
                base(workContextAccessor)
        {
			this.TableName = "tblSyncData";
        }

		[Url("admin/sync-data")]
		public ActionResult Index()
		{
			WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Sync Data"), Url = "#" });

			ViewBag.Title = T("Sync Data");

			return View("Index");
		}
    }
}
