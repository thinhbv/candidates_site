using System;
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
	using CMSSolutions.Extensions;
	using CMSSolutions.Websites.Extensions;

namespace CMSSolutions.Websites.Controllers
{
	[Authorize()]
	[Themed(IsDashboard = true)]
	public class EmployeeController : BaseAdminController
    {
		public EmployeeController(IWorkContextAccessor workContextAccessor) : 
                base(workContextAccessor)
        {
            this.TableName = "tblEmployee";
        }

		[Url("admin/employee")]
		public ActionResult Index()
        {
			WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Employees"), Url = "#" });

			ViewBag.Title = T("Employee List");

            return View("Index");
        }
    }
}
