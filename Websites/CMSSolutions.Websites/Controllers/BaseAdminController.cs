using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.Localization.Domain;
using CMSSolutions.Net.Mail;
using CMSSolutions.Web;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Security.Services;
using CMSSolutions.Websites.Entities;
using CMSSolutions.Websites.Extensions;
using CMSSolutions.Websites.Models;
using CMSSolutions.Websites.Services;

namespace CMSSolutions.Websites.Controllers
{
	public class BaseAdminController : BaseController
    {
		public BaseAdminController(IWorkContextAccessor workContextAccessor)
			: base(workContextAccessor)
		{

		}

		public void SendEmail(string subject, string body, string toEmailReceive, string ccEmail)
		{
			var service = WorkContext.Resolve<IEmailSender>();
			var mailMessage = new MailMessage
			{
				Subject = subject,
				SubjectEncoding = Encoding.UTF8,
				Body = body,
				BodyEncoding = Encoding.UTF8,
				IsBodyHtml = true
			};

			mailMessage.Sender = new MailAddress(toEmailReceive);
			mailMessage.To.Add(toEmailReceive);
			if (!string.IsNullOrEmpty(ccEmail))
			{
				mailMessage.CC.Add(ccEmail);
			}

			service.Send(mailMessage);
		}
		public string GetListToBCC()
		{
			var listBCC = string.Empty;
			var memberSV = WorkContext.Resolve<IMembershipService>();
			var listUsers = memberSV.GetUsersByRole(int.Parse(Extensions.Constants.ManagerRoleId));
			listBCC = string.Join(",", listUsers.Select(x => x.Email));
			return listBCC;
		}
    }
}
