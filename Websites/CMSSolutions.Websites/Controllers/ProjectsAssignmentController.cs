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
	public class ProjectsAssignmentController : BaseAdminController
    {
		public ProjectsAssignmentController(IWorkContextAccessor workContextAccessor) : 
                base(workContextAccessor)
        {
			this.TableName = "tblProjectsAssignment";
        }

		[Url("admin/project-assignment")]
		public ActionResult Index()
		{
			WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Projects Assignment"), Url = "#" });

			ViewBag.Title = T("Projects Assignment");

			return View("Index");
		}
    }
}
