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
    public class MailTemplatesController : BaseController
    {
        
        private readonly IMailTemplatesService service;
        
        public MailTemplatesController(IWorkContextAccessor workContextAccessor, IMailTemplatesService service) : 
                base(workContextAccessor)
        {
            this.service = service;
            this.TableName = "tblMailTemplates";
        }
        
        [Url("admin/mailtemplates")]
        public ActionResult Index()
        {
            WorkContext.Breadcrumbs.Add(new Breadcrumb { Text = T("Mail Templates"), Url = "#" });
            var result = new ControlGridFormResult<MailTemplates>();
            var siteSettings = WorkContext.Resolve<SiteSettings>();

            result.Title = this.T("Template List");
            result.CssClass = "table table-bordered table-striped";
            result.UpdateActionName = "Update";
            result.IsAjaxSupported = true;
            result.DefaultPageSize = siteSettings.DefaultPageSize;
            result.EnablePaginate = true;
            result.FetchAjaxSource = this.GetModule_MailTemplates;
            result.GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml;
            result.GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
			result.ActionsColumnWidth = 130;

            result.AddColumn(x => x.name, T("Name"));
            result.AddColumn(x => x.url_template, T("File Path"));

            result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Edit", new { id = 0 })).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(Constants.RowLeft).HasRow(true).ShowModalDialog();
			result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall).ShowModalDialog();
            result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(Constants.Messages.ConfirmDeleteRecord));
            
			result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");
            return result;
        }
        
        private ControlGridAjaxData<MailTemplates> GetModule_MailTemplates(ControlGridFormRequest options)
        {
            int totals;
            var items = this.service.GetRecords(options, out totals);
            var result = new ControlGridAjaxData<MailTemplates>(items, totals);
            return result;
        }

		[Themed(false)]
        [Url("admin/mailtemplates/edit/{id}")]
        public ActionResult Edit(int id)
        {
			var title = T("Create Template");
            var model = new MailTemplatesModel();
            if (id > 0)
            {
				 model = this.service.GetById(id);
				 title = T("Edit Template");
            }

			var result = new ControlFormResult<MailTemplatesModel>(model)
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

			result.RegisterFileUploadOptions("url_template.FileName", new ControlFileUploadOptions
			{
				AllowedExtensions = "html,txt"
			});

			return result;
        }
        
        [HttpPost()]
        [FormButton("Save")]
        [ValidateInput(false)]
        [Url("admin/mailtemplates/update")]
        public ActionResult Update(MailTemplatesModel model)
        {
            if (!ModelState.IsValid)
            {
				return new AjaxResult().Alert(T(Constants.Messages.InvalidModel));
            }
			var text = "Create success.";
            MailTemplates item;
            if (model.Id == 0)
            {
                item = new MailTemplates();
            }
            else
            {
                item = service.GetById(model.Id);
				text = "Update success.";
            }
            item.name = model.name;
            item.url_template = model.url_template;
            service.Save(item);

			return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(text).CloseModalDialog();
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
