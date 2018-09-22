using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.Extensions;
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

		public virtual string BuildLanguages()
		{
			var service = WorkContext.Resolve<ILanguagesService>();
			var items = service.GetRecords();
			var result = new List<SelectListItem>();
			result.AddRange(items.Select(item => new SelectListItem
			{
				Text = item.name,
				Value = item.Id.ToString()
			}));
			result.Insert(0, new SelectListItem { Text = T("---Choose Skill---"), Value = "0" });

			var sb = new StringBuilder();
			sb.AppendFormat(T("Skill") + " <select id=\"" + Extensions.Constants.LanguageId + "\" name=\"" + Extensions.Constants.LanguageId + "\" autocomplete=\"off\" class=\"uniform form-control col-md-3\" onchange=\"$('#" + TableName + "').jqGrid().trigger('reloadGrid');\">");
			foreach (var item in result)
			{
				sb.AppendFormat("<option value=\"{1}\">{0}</option>", item.Text, item.Value);
			}

			sb.Append("</select>");
			return sb.ToString();
		}

		public virtual string BuildLevels()
		{
			var service = WorkContext.Resolve<ILevelsService>();
			var items = service.GetRecords();
			var result = new List<SelectListItem>();
			result.AddRange(items.Select(item => new SelectListItem
			{
				Text = item.name,
				Value = item.Id.ToString()
			}));
			result.Insert(0, new SelectListItem { Text = T("---Choose Position---"), Value = "0" });

			var sb = new StringBuilder();
			sb.AppendFormat(T("Position") + " <select id=\"" + Extensions.Constants.LevelId + "\" name=\"" + Extensions.Constants.LevelId + "\" autocomplete=\"off\" class=\"uniform form-control col-md-3\" onchange=\"$('#" + TableName + "').jqGrid().trigger('reloadGrid');\">");
			foreach (var item in result)
			{
				sb.AppendFormat("<option value=\"{1}\">{0}</option>", item.Text, item.Value);
			}

			sb.Append("</select>");
			return sb.ToString();
		}

		public virtual string BuildCandiateStatus()
		{
			var result = EnumExtensions.GetListItems<CandidateStatus>();
			var sb = new StringBuilder();
			sb.AppendFormat(T("Status") + " <select id=\"" + Extensions.Constants.Status + "\" name=\"" + Extensions.Constants.Status + "\" autocomplete=\"off\" class=\"uniform form-control col-md-3\" onchange=\"$('#" + TableName + "').jqGrid().trigger('reloadGrid');\">");
			foreach (var item in result)
			{
				sb.AppendFormat("<option value=\"{1}\">{0}</option>", item.Text, item.Value);
			}

			sb.Append("</select>");
			return sb.ToString();
		}

		public virtual string BuildSearchText()
		{
			var sb = new StringBuilder();

			var keyword = GetQueryString(11);
			sb.AppendFormat(T("Keyword") + " <input value=\"" + keyword + "\" placeholder=\"" + T("Full Name/Email/Phone Number.") + "\" id=\"" + Extensions.Constants.Keyword + "\" name=\"" + Extensions.Constants.Keyword + "\" class=\"form-control\" onkeypress = \"return InputEnterEvent(event, '" + TableName + "');\" onblur=\"$('#" + TableName + "').jqGrid().trigger('reloadGrid');\"></input>");

			return sb.ToString();
		}

		public virtual string BuildFromDate(bool showDefaultDate = true)
		{
			var sb = new StringBuilder();
			var date = "";
			if (showDefaultDate)
			{
				date = DateTime.Now.AddMonths(-1).ToString("MM/dd/yyyy");
			}

			var fromDateSelected = GetQueryString(9);
			if (!string.IsNullOrEmpty(fromDateSelected))
			{
				date = fromDateSelected;
			}

			sb.AppendFormat(T("From Date") + " <input id=\"" + Extensions.Constants.FromDate + "\" name=\"" + Extensions.Constants.FromDate + "\" value=\"" + date + "\" class=\"form-control datepicker\"></input>");
			sb.Append("<script>$(document).ready(function () { " +
					  "$('.datepicker').datepicker({ " +
					  "dateFormat: 'mm/dd/yy', " +
					  "changeMonth: true, " +
					  "changeYear: true, " +
					  "onSelect: function (dateText) { " +
					  "$('#" + TableName + "').jqGrid().trigger('reloadGrid'); " +
					  "}}); });</script>");

			return sb.ToString();
		}

		public virtual string BuildToDate(bool showDefaultDate = true)
		{
			var sb = new StringBuilder();
			var date = "";
			if (showDefaultDate)
			{
				date = DateTime.Now.ToString("MM/dd/yyyy");
			}

			var toDateSelected = GetQueryString(10);
			if (!string.IsNullOrEmpty(toDateSelected))
			{
				date = toDateSelected;
			}

			sb.AppendFormat(T("To Date") + " <input id=\"" + Extensions.Constants.ToDate + "\" name=\"" + Extensions.Constants.ToDate + "\" value=\"" + date + "\" class=\"form-control datepicker\"></input>");
			sb.Append("<script>$(document).ready(function () { " +
					  "$('.datepicker').datepicker({ " +
					  "dateFormat: 'mm/dd/yy', " +
					  "changeMonth: true, " +
					  "changeYear: true, " +
					  "onSelect: function (dateText) { " +
					  "$('#" + TableName + "').jqGrid().trigger('reloadGrid'); " +
					  "}}); });</script>");

			return sb.ToString();
		}

		public string GetQueryString(int index)
		{
			var returnUrl = Request.QueryString[Extensions.Constants.ReturnUrl];
			if (!string.IsNullOrEmpty(returnUrl))
			{
				var data = Encoding.UTF8.GetString(Convert.FromBase64String(returnUrl));
				if (!string.IsNullOrEmpty(data))
				{
					var list = data.Split(',');
					if (list.Length > index)
					{
						var result = list[index];
						if (result != "null")
						{
							return result;
						}
					}
				}
			}

			return string.Empty;
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
