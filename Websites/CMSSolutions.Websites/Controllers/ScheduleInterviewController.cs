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
    public class ScheduleInterviewController : BaseController
    {
        
        private readonly IScheduleInterviewService service;
        
        public ScheduleInterviewController(IWorkContextAccessor workContextAccessor, IScheduleInterviewService service) : 
                base(workContextAccessor)
        {
            this.service = service;
            this.TableName = "tblScheduleInterview";
        }
        
        [Url("admin/scheduleinterviews")]
        public ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Schedule Interviews"), Url = "#" });
            var result = new ControlGridFormResult<ScheduleInterview>();
            var siteSettings = WorkContext.Resolve<SiteSettings>();
            result.Title = this.T("Management ScheduleInterview");
            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = this.GetModule_ScheduleInterview;
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
            result.AddColumn(x => x.Id);
            result.AddColumn(x => x.pos_id);
            result.AddColumn(x => x.candidate_id);
            result.AddColumn(x => x.interview_date);
            result.AddColumn(x => x.created_date);
            result.AddColumn(x => x.updated_date);
            result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Edit", new { id = 0 })).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(Constants.RowLeft).HasRow(true);
            result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(Constants.Messages.ConfirmDeleteRecord));
            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");
            return result;
        }
        
        private ControlGridAjaxData<ScheduleInterview> GetModule_ScheduleInterview(ControlGridFormRequest options)
        {
            int totals;
            var items = this.service.GetRecords(options, out totals);
            var result = new ControlGridAjaxData<ScheduleInterview>(items, totals);
            return result;
        }
        
        [Url("admin/scheduleinterviews/edit/{id}")]
        public ActionResult Edit(int id)
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("ScheduleInterview"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Schedule Interviews"), Url = Url.Action("Index") });
            var model = new ScheduleInterviewModel();
            if (id > 0)
            {
				 model = this.service.GetById(id);
            }
            var result = new ControlFormResult<ScheduleInterviewModel>(model);
            result.Title = this.T("Edit ScheduleInterview");
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
        [Url("admin/scheduleinterviews/update")]
        public ActionResult Update(ScheduleInterviewModel model)
        {
            if (!ModelState.IsValid)
            {
				return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }
            ScheduleInterview item;
            if (model.Id == 0)
            {
                item = new ScheduleInterview();
            }
            else
            {
                item = service.GetById(model.Id);
            }
            item.pos_id = model.pos_id;
            item.candidate_id = model.candidate_id;
            item.interview_date = model.interview_date;
            item.created_date = model.created_date;
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
