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
	public class CandidatesController : BaseAdminController
    {
        
        private readonly ICandidatesService service;
        
        public CandidatesController(IWorkContextAccessor workContextAccessor, ICandidatesService service) : 
                base(workContextAccessor)
        {
            this.service = service;
            this.TableName = "tblCandidates";
        }
        
        [Url("admin/candidates")]
        public ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Candidates"), Url = "#" });
            var result = new ControlGridFormResult<Candidates>();
            var siteSettings = WorkContext.Resolve<SiteSettings>();

			result.Title = this.T("Candidate List");
            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = this.GetModule_Candidates;
            result.GridWrapperStartHtml = CMSSolutions.Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = CMSSolutions.Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;

            result.AddColumn(x => x.Id, T("ID")).HasWidth(60);
			result.AddColumn(x => x.full_name, T("Full Name"));
			result.AddColumn(x => x.mail_address, T("Email"));
			result.AddColumn(x => x.phone_number, T("Phone Number"));
			result.AddColumn(x => x.is_employee)
				.HasHeaderText(T("Is Employed"))
				.AlignCenter()
				.HasWidth(100)
				.RenderAsStatusImage();

            result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Edit", new { id = 0 })).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(CMSSolutions.Constants.RowLeft).HasRow(true);
			result.AddRowAction().HasText(this.T("View CV")).HasUrl(x => Url.Action("CandidateProfile", new { id = x.Id })).HasButtonStyle(ButtonStyle.Info).HasButtonSize(ButtonSize.ExtraSmall);
			result.AddRowAction().HasText(this.T("Send Mail")).HasUrl(x => Url.Action("SendMail", new { id = x.Id })).HasButtonStyle(ButtonStyle.Success).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall);
			result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(CMSSolutions.Constants.Messages.ConfirmDeleteRecord));
            
			result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");

			return result;
        }
        
        private ControlGridAjaxData<Candidates> GetModule_Candidates(ControlGridFormRequest options)
        {
            int totals;
			var items = this.service.GetRecords(options, out totals, x => x.status == (int)CandidateStatus.Actived);
            var result = new ControlGridAjaxData<Candidates>(items, totals);
            return result;
        }
        
        [Url("admin/candidates/edit/{id}")]
        public ActionResult Edit(int id)
        {
			var text = T("Create Candidate");
            var model = new CandidatesModel();
            if (id > 0)
            {
				text = T("Edit Candidate");
				 model = this.service.GetById(id);
			}

			WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Candidates"), Url = Url.Action("Index") });
			WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = text, Url = "#" });

            var result = new ControlFormResult<CandidatesModel>(model);
			result.Title = text;
            result.FormMethod = FormMethod.Post;
            result.UpdateActionName = "Update";
            result.ShowCancelButton = false;
            result.ShowBoxHeader = false;
            result.FormWrapperStartHtml = CMSSolutions.Constants.Form.FormWrapperStartHtml;
            result.FormWrapperEndHtml = CMSSolutions.Constants.Form.FormWrapperEndHtml;
            result.AddAction().HasText(this.T("Cancel")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);

			result.RegisterFileUploadOptions("cv_path.FileName", new ControlFileUploadOptions
			{
				AllowedExtensions = "doc,docx"
			});
			result.RegisterExternalDataSource(x => x.hr_user_id, y => BindHRUsers(model.hr_user_id));

			if (id != 0)
			{
				var result2 = new ControlGridFormResult<LevelCandidates>();
				result2.Title = this.T("Skills Of Candidate");
				result2.CssClass = "table table-bordered table-striped";
				result2.UpdateActionName = "Update";
				result2.IsAjaxSupported = true;
				result2.DefaultPageSize = WorkContext.DefaultPageSize;
				result2.EnablePaginate = true;
				result2.FetchAjaxSource = this.GetModule_LevelCandidates;
				result2.GridWrapperStartHtml = CMSSolutions.Constants.Grid.GridWrapperStartHtml;
				result2.GridWrapperEndHtml = CMSSolutions.Constants.Grid.GridWrapperEndHtml;
				result2.ClientId = TableName;
				result2.ActionsColumnWidth = 130;

				result2.AddColumn(x => x.language_name, T("Skill"));
				result2.AddColumn(x => x.level_name, T("Level"));
				result2.AddColumn(x => x.month, T("Experience(month)"));
				result2.AddColumn(x => x.is_main)
					.HasHeaderText(T("Main Skill"))
					.AlignCenter()
					.HasWidth(100)
					.RenderAsStatusImage();

				result2.AddAction().HasText(this.T("Create"))
					.HasUrl(this.Url.Action("Edit", "LevelCandidates", new { id = 0, candidateId = id }))
					.HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false)
					.HasCssClass(CMSSolutions.Constants.RowLeft).HasRow(true).ShowModalDialog(600, 600);

				result2.AddRowAction().HasText(this.T("Edit"))
					.HasUrl(x => Url.Action("Edit", "LevelCandidates", new { id = x.Id, candidateId = id }))
					.HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall).ShowModalDialog(600, 600);

				result2.AddRowAction(true)
					.HasText(T("Delete"))
					.HasName("DeleteLevel")
					.HasValue(x => x.Id)
					.HasButtonStyle(ButtonStyle.Danger)
					.HasButtonSize(ButtonSize.ExtraSmall)
					.HasConfirmMessage(T(CMSSolutions.Constants.Messages.ConfirmDeleteRecord).Text);

				result2.AddCustomVar(Extensions.Constants.CandidateId, id);
				result2.AddReloadEvent("DELETE_ENTITY_COMPLETE");

				return new ControlFormsResult(result, result2);
			}

			return result;
        }

		[Url("admin/candidates/view-profile/{id}")]
		public ActionResult CandidateProfile(int id)
		{
			WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Candidates"), Url = Url.Action("Index") });
			WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("View Profile"), Url = "#" });

			//var text = T("Create Candidate");
			//var model = new CandidatesModel();
			//if (id > 0)
			//{
			//	text = T("Edit Candidate");
			//	model = this.service.GetById(id);
			//}

			//WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Candidates"), Url = Url.Action("Index") });
			//WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = text, Url = "#" });

			//var result = new ControlFormResult<CandidatesModel>(model);
			//result.Title = text;
			//result.FormMethod = FormMethod.Post;
			//result.UpdateActionName = "Update";
			//result.ShowCancelButton = false;
			//result.ShowBoxHeader = false;
			//result.FormWrapperStartHtml = CMSSolutions.Constants.Form.FormWrapperStartHtml;
			//result.FormWrapperEndHtml = CMSSolutions.Constants.Form.FormWrapperEndHtml;
			//result.AddAction().HasText(this.T("Cancel")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);

			return View("ViewProfile");
		}

		[Themed(false)]
		[Url("admin/candidates/send-mail/{id}")]
		public ActionResult SendMail(int id)
		{
			var model = new MailModel();
			var result = new ControlFormResult<MailModel>(model)
			{
				Title = T("Send Mail Recruitment"),
				FormMethod = FormMethod.Post,
				UpdateActionName = "Update",
				SubmitButtonText = T("Save"),
				CancelButtonText = T("Close"),
				ShowBoxHeader = false,
				FormWrapperStartHtml = CMSSolutions.Constants.Form.FormWrapperStartHtml,
				FormWrapperEndHtml = CMSSolutions.Constants.Form.FormWrapperEndHtml
			};

			return result;
		}

		private ControlGridAjaxData<LevelCandidates> GetModule_LevelCandidates(ControlGridFormRequest options)
		{
			var candidateId = 0;
			if (Utilities.IsNotNull(Request.Form[Extensions.Constants.CandidateId]))
			{
				candidateId = Convert.ToInt32(Request.Form[Extensions.Constants.CandidateId]);
			}

			int totals;
			var languagesv = WorkContext.Resolve<ILanguagesService>();
			var levelsv = WorkContext.Resolve<ILevelsService>();
			var levelCandidatesv = WorkContext.Resolve<ILevelCandidatesService>();
			var listLanguages = languagesv.GetRecords();
			var listLevels = levelsv.GetRecords();

			var items = levelCandidatesv.GetRecords(options, out totals, x => x.candidate_id == candidateId);

			var result = new ControlGridAjaxData<LevelCandidates>(items, totals);
			if (result.Count > 0)
			{
				if (listLanguages.Count > 0)
				{
					foreach (var lg in listLanguages)
					{
						foreach (var item in result)
						{
							if (item.language_id == lg.Id)
							{
								item.language_name = lg.name;
								break;
							}
						}
					}
				}

				if (listLevels.Count > 0)
				{
					foreach (var vl in listLevels)
					{
						foreach (var item in result)
						{
							if (item.level_dev == vl.Id)
							{
								item.level_name = vl.name;
								break;
							}
						}
					}
				}
			}
			
			return result;
		}

		private IEnumerable<SelectListItem> BindHRUsers(int hr_id)
		{
			var HRRoleId = Convert.ToInt32(CMSSolutions.Websites.Extensions.Constants.HRRoleId);
			var service = WorkContext.Resolve<IMembershipService>();
			var items = service.GetUsersByRole(HRRoleId);
			var result = new List<SelectListItem>();
			if (items != null)
			{
				result.AddRange(items.Select(item => new SelectListItem
				{
					Text = item.FullName,
					Value = item.Id.ToString(),
					Selected = item.Id == hr_id
				}));
			}

			return result;
		}

        [HttpPost()]
        [FormButton("Save")]
        [ValidateInput(false)]
        [Url("admin/candidates/update")]
        public ActionResult Update(CandidatesModel model)
        {
            if (!ModelState.IsValid)
            {
				return new AjaxResult().Alert(T(CMSSolutions.Constants.Messages.InvalidModel));
            }
			var text = "Create success.";
            Candidates item;
			var status = false;
            if (model.Id == 0)
            {
                item = new Candidates();
				item.created_user_id = WorkContext.CurrentUser.Id;
				item.created_date = DateTime.Now;
            }
            else
            {
                item = service.GetById(model.Id);
				item.updated_user_id = WorkContext.CurrentUser.Id;
				item.updated_date = DateTime.Now;
				text = "Update success.";
				status = true;
            }
            item.full_name = model.full_name;
			if (!string.IsNullOrEmpty(model.birthday))
			{
				item.birthday = DateTime.ParseExact(model.birthday, Extensions.Constants.DateTimeFomat, CultureInfo.InvariantCulture);
			}
            item.mail_address = model.mail_address;
            item.phone_number = model.phone_number;
            item.address = model.address;
			if (!string.IsNullOrEmpty(model.start_working_date))
			{
				item.start_working_date = DateTime.ParseExact(model.start_working_date, Extensions.Constants.DateTimeFomat, CultureInfo.InvariantCulture);
			}

            item.hr_user_id = model.hr_user_id;
            item.cv_path = model.cv_path;
			
            service.Save(item);
			if (!status)
			{
				return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(text).Redirect(this.Url.Action("Edit", new { id = item.Id }));
			}
			else 
			{
				return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(text);
			}
        }
        
        [ActionName("Update")]
        [FormButton("Delete")]
        public ActionResult Delete(int id)
        {
			var text = string.Empty;
            var model = service.GetById(id);
			if (model.status == (int)CandidateStatus.Actived)
			{
				model.status = (int)CandidateStatus.Blocked;
				text = T("Block success.");
			}
			else
			{
				model.status = (int)CandidateStatus.Actived;
				text = T("Active success.");
			}
            service.Update(model);

			return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE").Alert(text);
        }

		[ActionName("Update")]
		[FormButton("DeleteLevel")]
		public ActionResult DeleteLevel(int id)
		{
			var levelsv = WorkContext.Resolve<ILevelCandidatesService>();
			var model = levelsv.GetById(id);
			levelsv.Delete(model);

			return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE").Alert(T("Deleted success."));
		}
    }
}
