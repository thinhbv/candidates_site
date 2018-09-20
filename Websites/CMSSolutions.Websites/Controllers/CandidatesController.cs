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
    public class CandidatesController : BaseController
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
            result.Title = this.T("Management Candidates");
            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = this.GetModule_Candidates;
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
            result.AddColumn(x => x.Id);
            result.AddColumn(x => x.full_name);
            result.AddColumn(x => x.birthday);
            result.AddColumn(x => x.mail_address);
            result.AddColumn(x => x.phone_number);
            result.AddColumn(x => x.address);
            result.AddColumn(x => x.start_working_date);
            result.AddColumn(x => x.hr_user_id);
            result.AddColumn(x => x.cv_path);
            result.AddColumn(x => x.created_user_id);
            result.AddColumn(x => x.created_date);
            result.AddColumn(x => x.updated_user_id);
            result.AddColumn(x => x.updated_date);
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
            result.AddAction().HasText(this.T("Clear")).HasUrl(this.Url.Action("Edit", RouteData.Values.Merge(new { id = 0 }))).HasButtonStyle(ButtonStyle.Success);
            result.AddAction().HasText(this.T("Back")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);
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
            }
            else
            {
                item = service.GetById(model.Id);
            }
            item.full_name = model.full_name;
            item.birthday = model.birthday;
            item.mail_address = model.mail_address;
            item.phone_number = model.phone_number;
            item.address = model.address;
            item.start_working_date = model.start_working_date;
            item.hr_user_id = model.hr_user_id;
            item.cv_path = model.cv_path;
            item.created_user_id = model.created_user_id;
            item.created_date = model.created_date;
            item.updated_user_id = model.updated_user_id;
            item.updated_date = model.updated_date;
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
