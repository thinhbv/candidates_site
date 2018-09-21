using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.WebPages.Html;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Security.Services;

namespace CMSSolutions.Websites.Controllers
{
	public class BaseAdminController : BaseController
    {
		public BaseAdminController(IWorkContextAccessor workContextAccessor)
			: base(workContextAccessor)
		{

		}
    }
}
