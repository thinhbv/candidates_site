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
	public class DashboardEmployeeController : BaseAdminController
    {
		public DashboardEmployeeController(IWorkContextAccessor workContextAccessor) : 
                base(workContextAccessor)
        {
			this.TableName = "tblDashboard";
        }

		[Url("admin/employee-dashboard")]
		public ActionResult Index()
		{
			WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Dashboard"), Url = "#" });

			ViewBag.Title = T("Dashboard");

			return View("Index");
		}
    }
}
