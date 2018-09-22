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
	using CMSSolutions.Web.Security.Services;
	using CMSSolutions.Websites.Extensions;
    
    
    [Authorize()]
    [Themed(IsDashboard=true)]
    public class MailProcessController : BaseAdminController
    {
		public MailProcessController(IWorkContextAccessor workContextAccessor) : 
                base(workContextAccessor)
        {
			this.TableName = "tblMailProcess";
        }

		[Themed(false)]
		[Url("admin/candidates/send-mail/{id}")]
		public ActionResult SendMail(int id)
		{
			var service = WorkContext.Resolve<ICandidatesService>();
			var item = service.GetById(id);

			var model = new MailModel();
			model.mail_to = item.mail_address;
			var result = new ControlFormResult<MailModel>(model)
			{
				Title = T("Send Mail Recruitment"),
				FormMethod = FormMethod.Post,
				UpdateActionName = "Update",
				SubmitButtonText = T("Send"),
				CancelButtonText = T("Close"),
				ShowBoxHeader = false,
				FormWrapperStartHtml = CMSSolutions.Constants.Form.FormWrapperStartHtml,
				FormWrapperEndHtml = CMSSolutions.Constants.Form.FormWrapperEndHtml
			};

			result.RegisterExternalDataSource(x => x.mail_cc, y => BindUserRelated());
			result.RegisterExternalDataSource(x => x.template, y => BindTemplates());
			result.RegisterCascadingDropDownDataSource(x => x.mail_body, Url.Action("GetRecruitment"));

			return result;
		}

		private IEnumerable<SelectListItem> BindUserRelated()
		{
			var InterviewRoleId = Convert.ToInt32(CMSSolutions.Websites.Extensions.Constants.InterviewRoleId);
			var service = WorkContext.Resolve<IMembershipService>();
			var items = service.GetUsersByRole(InterviewRoleId);
			var result = new List<SelectListItem>();
			if (items != null)
			{
				result.AddRange(items.Select(item => new SelectListItem
				{
					Text = item.FullName,
					Value = item.Id.ToString(),
					Selected = false
				}));
			}

			return result;
		}

		private IEnumerable<SelectListItem> BindTemplates()
		{
			var service = WorkContext.Resolve<IMailTemplatesService>();
			var items = service.GetRecords();
			var result = new List<SelectListItem>();
			if (items != null)
			{
				result.AddRange(items.Select(item => new SelectListItem
				{
					Text = item.name,
					Value = item.Id.ToString(),
					Selected = false
				}));
			}
			result.Insert(0, new SelectListItem
			{
				Text = "---Choose Template---",
				Value = "0",
				Selected = true
			});
			return result;
		}

		[Url("admin/candidates/get-recruitment")]
		public ActionResult GetRecruitment()
		{
			var templateId = 0;
			var result = new List<SelectListItem>();

			if (Utilities.IsNotNull(Request.Form[Extensions.Constants.Template]))
			{
				templateId = Convert.ToInt32(Request.Form[Extensions.Constants.Template]);
			}

			if (templateId > 0)
			{
				var sv = WorkContext.Resolve<IPositionsService>();
				var items = sv.GetRecords();
				result.AddRange(items.Select(item => new SelectListItem
				{
					Text = item.pos_name,
					Value = item.Id.ToString()
				}));
			}
			
			return Json(result);
		}

        [HttpPost()]
        [FormButton("Save")]
        [ValidateInput(false)]
		[Url("admin/send-mail/update")]
		public ActionResult Update(MailModel model)
        {
            if (!ModelState.IsValid)
            {
				return new AjaxResult().Alert(T(CMSSolutions.Constants.Messages.InvalidModel));
            }
			
			var listBCC = string.Empty;
			if (model.mail_cc != null && model.mail_cc.Length > 0)
			{
				var memberSV = WorkContext.Resolve<IMembershipService>();
				var listUsers = memberSV.GetUsers(model.mail_cc);
				listBCC = string.Join(", ", listUsers.Select(x => x.Email));
			}

			var templateSV = WorkContext.Resolve<IMailTemplatesService>();
			var templateInfo = templateSV.GetById(model.template);
			var body = System.IO.File.ReadAllText(Server.MapPath(string.Format("~{0}", templateInfo.url_template)));

			SendEmail(model.subject, body, model.mail_to, listBCC);

			return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").CloseModalDialog();
        }
    }
}
