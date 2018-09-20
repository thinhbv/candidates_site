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
    public class PositionsController : BaseController
    {
        
        private readonly IPositionsService service;
        
        public PositionsController(IWorkContextAccessor workContextAccessor, IPositionsService service) : 
                base(workContextAccessor)
        {
            this.service = service;
            this.TableName = "tblPositions";
        }
        
        [Url("admin/positions")]
        public ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Positions"), Url = "#" });
            var result = new ControlGridFormResult<Positions>();
            var siteSettings = WorkContext.Resolve<SiteSettings>();
            result.Title = this.T("Management Positions");
            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = this.GetModule_Positions;
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
            result.AddColumn(x => x.Id);
            result.AddColumn(x => x.pos_name);
            result.AddColumn(x => x.created_date);
            result.AddColumn(x => x.updated_date);
            result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Edit", new { id = 0 })).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(Constants.RowLeft).HasRow(true);
            result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall);
            result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(Constants.Messages.ConfirmDeleteRecord));
            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");
            return result;
        }
        
        private ControlGridAjaxData<Positions> GetModule_Positions(ControlGridFormRequest options)
        {
            int totals;
            var items = this.service.GetRecords(options, out totals);
            var result = new ControlGridAjaxData<Positions>(items, totals);
            return result;
        }
        
        [Url("admin/positions/edit/{id}")]
        public ActionResult Edit(int id)
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Positions"), Url = "#" });
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Positions"), Url = Url.Action("Index") });
            var model = new PositionsModel();
            if (id > 0)
            {
				 model = this.service.GetById(id);
            }
            var result = new ControlFormResult<PositionsModel>(model);
            result.Title = this.T("Edit Positions");
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
        [Url("admin/positions/update")]
        public ActionResult Update(PositionsModel model)
        {
            if (!ModelState.IsValid)
            {
				return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }
            Positions item;
            if (model.Id == 0)
            {
                item = new Positions();
            }
            else
            {
                item = service.GetById(model.Id);
            }
            item.pos_name = model.pos_name;
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