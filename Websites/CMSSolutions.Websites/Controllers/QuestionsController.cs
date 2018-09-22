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
	public class QuestionsController : BaseAdminController
    {
        
        private readonly IQuestionsService service;
        
        public QuestionsController(IWorkContextAccessor workContextAccessor, IQuestionsService service) : 
                base(workContextAccessor)
        {
            this.service = service;
            this.TableName = "tblQuestions";
        }
        
        [Url("admin/questions")]
        public ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Questions"), Url = "#" });
            var result = new ControlGridFormResult<Questions>();
            var siteSettings = WorkContext.Resolve<SiteSettings>();
            result.Title = this.T("Management Questions");
            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = this.GetModule_Questions;
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
            result.AddColumn(x => x.Id);
            result.AddColumn(x => x.language_id);
            result.AddColumn(x => x.content);
            result.AddColumn(x => x.creator);
            result.AddColumn(x => x.created_date);
            result.AddColumn(x => x.updated_date);
            result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Edit", new { id = 0 })).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(Constants.RowLeft).HasRow(true);
            result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(Constants.Messages.ConfirmDeleteRecord));
            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");
            return result;
        }
        
        private ControlGridAjaxData<Questions> GetModule_Questions(ControlGridFormRequest options)
        {
            int totals;
            var items = this.service.GetRecords(options, out totals);
            var result = new ControlGridAjaxData<Questions>(items, totals);
            return result;
        }
        
        [Url("admin/questions/edit/{id}")]
        public ActionResult Edit(int id)
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Questions"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Questions"), Url = Url.Action("Index") });
            var model = new QuestionsModel();
            if (id > 0)
            {
				 model = this.service.GetById(id);
            }
            var result = new ControlFormResult<QuestionsModel>(model);
            result.Title = this.T("Edit Questions");
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
        [Url("admin/questions/update")]
        public ActionResult Update(QuestionsModel model)
        {
            if (!ModelState.IsValid)
            {
				return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }
            Questions item;
            if (model.Id == 0)
            {
                item = new Questions();
            }
            else
            {
                item = service.GetById(model.Id);
            }
            item.language_id = model.language_id;
            item.content = model.content;
            item.creator = model.creator;
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
