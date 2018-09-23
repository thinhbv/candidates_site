namespace CMSSolutions.Websites.Controllers
{
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
    
    
    [Authorize()]
    [Themed(IsDashboard=true)]
	public class EmployeesController : BaseAdminController
    {
		public EmployeesController(IWorkContextAccessor workContextAccessor) : 
                base(workContextAccessor)
        {
			this.TableName = "tblEmployees";
        }

		[Url("admin/employees")]
		public ActionResult Index()
		{
			WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Employees"), Url = "#" });

			ViewBag.Title = T("Employees");

			return View();
		}
    }
}
