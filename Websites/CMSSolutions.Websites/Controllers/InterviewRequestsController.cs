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
	public class InterviewRequestsController : BaseAdminController
	{
		public InterviewRequestsController(IWorkContextAccessor workContextAccessor) :
			base(workContextAccessor)
		{
			this.TableName = "tblInterviewRequests";
		}

		[Url("admin/interview-requests")]
		public ActionResult Index()
		{
			WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Interview Requests"), Url = "#" });

			ViewBag.Title = T("Interview Requests");

			return View("Index");
		}
	}
}
