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
    public class InterviewController : BaseController
    {
        
        private readonly IInterviewService service;
        
        public InterviewController(IWorkContextAccessor workContextAccessor, IInterviewService service) : 
                base(workContextAccessor)
        {
            this.service = service;
            this.TableName = "tblInterview";
        }
        
        [Url("admin/interviews")]
        public ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Interviews"), Url = "#" });
            var result = new ControlGridFormResult<Interview>();
            var siteSettings = WorkContext.Resolve<SiteSettings>();
            result.Title = this.T("Management Interview");
            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = this.GetModule_Interview;
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
            result.AddColumn(x => x.Id);
            result.AddColumn(x => x.candidate_id);
            result.AddColumn(x => x.round_id);
            result.AddColumn(x => x.position_id);
            result.AddColumn(x => x.interview_date_plan);
            result.AddColumn(x => x.interview_date);
            result.AddColumn(x => x.interviewer_id);
            result.AddColumn(x => x.evaluation);
            result.AddColumn(x => x.status);
            result.AddColumn(x => x.interview_result);
            result.AddColumn(x => x.created_date);
            result.AddColumn(x => x.created_user_id);
            result.AddColumn(x => x.updated_date);
            result.AddColumn(x => x.updated_user_id);
            result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Edit", new { id = 0 })).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(Constants.RowLeft).HasRow(true);
            result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(Constants.Messages.ConfirmDeleteRecord));
            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");
            return result;
        }
        
        private ControlGridAjaxData<Interview> GetModule_Interview(ControlGridFormRequest options)
        {
            int totals;
            var items = this.service.GetRecords(options, out totals);
            var result = new ControlGridAjaxData<Interview>(items, totals);
            return result;
        }
        
        [Url("admin/interviews/edit/{id}")]
        public ActionResult Edit(int id)
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Interview"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Interviews"), Url = Url.Action("Index") });
            var model = new InterviewModel();
            if (id > 0)
            {
				 model = this.service.GetById(id);
            }
            var result = new ControlFormResult<InterviewModel>(model);
            result.Title = this.T("Edit Interview");
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
        [Url("admin/interviews/update")]
        public ActionResult Update(InterviewModel model)
        {
            if (!ModelState.IsValid)
            {
				return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }
            Interview item;
            if (model.Id == 0)
            {
                item = new Interview();
            }
            else
            {
                item = service.GetById(model.Id);
            }
            item.candidate_id = model.candidate_id;
            item.round_id = model.round_id;
            item.position_id = model.position_id;
            item.interview_date_plan = model.interview_date_plan;
            item.interview_date = model.interview_date;
            item.interviewer_id = model.interviewer_id;
            item.evaluation = model.evaluation;
            item.status = model.status;
            item.interview_result = model.interview_result;
            item.created_date = model.created_date;
            item.created_user_id = model.created_user_id;
            item.updated_date = model.updated_date;
            item.updated_user_id = model.updated_user_id;
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
