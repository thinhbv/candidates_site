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

            result.Title = this.T("Position List");
            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = this.GetModule_Positions;
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
			result.ActionsColumnWidth = 150;

			result.AddColumn(x => x.pos_name, T("Position Name"));
            result.AddColumn(x => x.created_date, T("Created Date"));

			result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Edit", new { id = 0 })).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(Constants.RowLeft).HasRow(true).ShowModalDialog();
			result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall).ShowModalDialog();
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

		[Themed(false)]
        [Url("admin/positions/edit/{id}")]
        public ActionResult Edit(int id)
        {
			var title = T("Create Position");
            var model = new PositionsModel();
            if (id > 0)
            {
				 model = this.service.GetById(id);
            }
			title = T("Edit Position");

			var result = new ControlFormResult<PositionsModel>(model)
            {
				Title = title,
                FormMethod = FormMethod.Post,
                UpdateActionName = "Update",
                SubmitButtonText = T("Save"),
                CancelButtonText = T("Close"),
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

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

			var text = "Create success.";
            Positions item;
            if (model.Id == 0)
            {
                item = new Positions();
				item.created_date = DateTime.Now;
            }
            else
            {
                item = service.GetById(model.Id);
				item.updated_date = DateTime.Now;
				text = "Update success.";
            }

            item.pos_name = model.pos_name;
            service.Save(item);

			return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(text).CloseModalDialog();
        }
        
        [ActionName("Update")]
        [FormButton("Delete")]
        public ActionResult Delete(int id)
        {
            var model = service.GetById(id);
			var interSV = WorkContext.Resolve<IInterviewService>();
			var total = interSV.GetRecords(x => x.position_id == id).Count;
			if (total <= 0)
			{
				service.Delete(model);
				return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE").Alert("Delete success.");
			}
			else
			{
				return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE").Alert("Cannot delete because using for interview.");
			}
        }
    }
}
