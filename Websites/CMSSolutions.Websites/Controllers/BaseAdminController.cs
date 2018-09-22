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

		[HttpPost, ValidateInput(false)]
		[Url("admin/hr/send-job.html")]
		public ActionResult SendJob()
		{
			var email = Request.Form["txtEmailAddress"];
			var fullName = Request.Form["txtFullName"];
			var phoneNumber = Request.Form["txtPhoneNumber"];
			var messages = Request.Form["txtMessages"];

			var result = new DataViewModel();
			var htmlBuilder = new StringBuilder();
			htmlBuilder.Append("<div style=\"float:left;width:100%;\">");
			htmlBuilder.AppendFormat("<div style=\"float:left;\">{0} </div>", T("Họ và Tên:"));
			htmlBuilder.AppendFormat("<div style=\"float:left;margin-left:5px;\">{0}</div>", fullName);
			htmlBuilder.Append("</div><br>");

			htmlBuilder.Append("<div style=\"float:left;width:100%;\">");
			htmlBuilder.AppendFormat("<div style=\"float:left;\">{0} </div>", T("Số điện thoại:"));
			htmlBuilder.AppendFormat("<div style=\"float:left;margin-left:5px;\">{0}</div>", phoneNumber);
			htmlBuilder.Append("</div><br>");

			htmlBuilder.Append("<div style=\"float:left;width:100%;\">");
			htmlBuilder.AppendFormat("<div style=\"float:left;\">{0} </div>", T("Địa chỉ email:"));
			htmlBuilder.AppendFormat("<div style=\"float:left;margin-left:5px;\">{0}</div>", email);
			htmlBuilder.Append("</div><br>");

			htmlBuilder.Append("<div style=\"float:left;width:100%;\">");
			htmlBuilder.AppendFormat("<div style=\"float:left;\">{0} </div>", T("Yêu cầu của bạn:"));
			htmlBuilder.AppendFormat("<div style=\"float:left;margin-left:5px;\">{0}</div>", messages);
			htmlBuilder.Append("</div><br>");

			string html = System.IO.File.ReadAllText(Server.MapPath("~/Media/Default/EmailTemplates/JobTemplate.html"));
			try
			{
				SendEmail(T("Thông tin liên hệ"), html, email, "sitetab2016@gmail.com");
				result.Status = true;
				result.Data = T("Send Email Success");
			}
			catch (Exception)
			{
				result.Status = false;
				result.Data = T("Send Email Fail");
			}

			return Json(result);
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
				mailMessage.Bcc.Add("iccare.contact@gmail.com");
			}

			service.Send(mailMessage);
		}
    }
}
