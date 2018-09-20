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
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;

            result.AddColumn(x => x.Id, T("ID"));
			result.AddColumn(x => x.full_name, T("Full Name"));
			result.AddColumn(x => x.mail_address, T("Email"));
			result.AddColumn(x => x.phone_number, T("Phone Number"));
			result.AddColumn(x => x.hr_full_name, T("HR Recipient"));
            result.AddColumn(x => x.created_date, T("Created Date"));
            result.AddColumn(x => x.updated_date, T("Updated Date"));
			result.AddColumn(x => x.is_employee)
				.HasHeaderText(T("Is Employed"))
				.AlignCenter()
				.HasWidth(100)
				.RenderAsStatusImage();

            result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Edit", new { id = 0 })).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(Constants.RowLeft).HasRow(true);
            result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(Constants.Messages.ConfirmDeleteRecord));
            
			result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");
            return result;
        }
        
        private ControlGridAjaxData<Candidates> GetModule_Candidates(ControlGridFormRequest options)
        {
            int totals;
            var items = this.service.GetRecords(options, out totals);
            var result = new ControlGridAjaxData<Candidates>(items, totals);
            return result;
        }
        
        [Url("admin/candidates/edit/{id}")]
        public ActionResult Edit(int id)
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Candidates"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Candidates"), Url = Url.Action("Index") });
            var model = new CandidatesModel();
            if (id > 0)
            {
				 model = this.service.GetById(id);
            }
            var result = new ControlFormResult<CandidatesModel>(model);
            result.Title = this.T("Edit Candidates");
            result.FormMethod = FormMethod.Post;
            result.UpdateActionName = "Update";
            result.ShowCancelButton = false;
            result.ShowBoxHeader = false;
            result.FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml;
            result.FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml;
            result.AddAction().HasText(this.T("Cancel")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);

			result.RegisterExternalDataSource(x => x.hr_user_id, y => BindHRUsers(model.hr_user_id));

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
				return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }
            Candidates item;
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
            }
            item.full_name = model.full_name;
			if (string.IsNullOrEmpty(model.birthday))
			{
				model.birthday = Extensions.Constants.DateTimeMin;
			}
			item.birthday = DateTime.ParseExact(model.birthday, Extensions.Constants.DateTimeFomat, CultureInfo.InvariantCulture);
            item.mail_address = model.mail_address;
            item.phone_number = model.phone_number;
            item.address = model.address;
			if (string.IsNullOrEmpty(model.start_working_date))
			{
				model.start_working_date = Extensions.Constants.DateTimeMin;
			}
			item.start_working_date = DateTime.ParseExact(model.start_working_date, Extensions.Constants.DateTimeFomat, CultureInfo.InvariantCulture);

            item.hr_user_id = model.hr_user_id;
            item.cv_path = model.cv_path;

            service.Save(item);
			return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE");
        }
        
        [ActionName("Update")]
        [FormButton("Delete")]
        public ActionResult Delete(int id)
        {
            var model = service.GetById(id);
            service.Delete(model);
			return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE");
        }
    }
}
