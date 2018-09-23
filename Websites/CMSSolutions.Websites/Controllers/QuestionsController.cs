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
	using CMSSolutions.Extensions;
	using CMSSolutions.Websites.Extensions;
    
    
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
            result.GridWrapperStartHtml = CMSSolutions.Constants.Grid.GridWrapperStartHtml;
			result.GridWrapperEndHtml = CMSSolutions.Constants.Grid.GridWrapperEndHtml;
            result.ClientId = TableName;
			result.AddColumn(x => x.language_name, T("Language"));
			result.AddColumn(x => x.type_name, T("Position"));
            result.AddColumn(x => x.content, T("Content Question"));
			result.AddAction().HasText(this.T("Create")).HasUrl(this.Url.Action("Edit", new { id = 0 })).HasButtonStyle(ButtonStyle.Primary).HasBoxButton(false).HasCssClass(CMSSolutions.Constants.RowLeft).HasRow(true);
            result.AddRowAction().HasText(this.T("Edit")).HasUrl(x => Url.Action("Edit", new { id = x.Id })).HasButtonStyle(ButtonStyle.Default).HasButtonSize(ButtonSize.ExtraSmall);
			result.AddRowAction(true).HasText(this.T("Delete")).HasName("Delete").HasValue(x => Convert.ToString(x.Id)).HasButtonStyle(ButtonStyle.Danger).HasButtonSize(ButtonSize.ExtraSmall).HasConfirmMessage(this.T(CMSSolutions.Constants.Messages.ConfirmDeleteRecord));
            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");
            return result;
        }

		private string DisplayNamePosition(string types)
		{
			string strReturn = string.Empty;
			string[] tmp;
			if (types.Contains(","))
			{
				tmp = types.Split(Char.Parse(","));
			}
			else
			{
				tmp = new string[] { types };
			}
			for (int i = 0; i < tmp.Length; i++)
			{
				int iTemp = int.Parse(tmp[i]);
				strReturn = strReturn + EnumExtensions.GetDisplayName((PositionType)iTemp) + ",";
			}
			if (!string.IsNullOrEmpty(strReturn))
			{
				strReturn = strReturn.Substring(0, strReturn.Length - 1);
			}
			return strReturn;
		}

        private ControlGridAjaxData<Questions> GetModule_Questions(ControlGridFormRequest options)
        {
            int totals;
            var items = this.service.GetRecords(options, out totals);
            var result = new ControlGridAjaxData<Questions>(items, totals);
			foreach (var item in result)
			{
				item.type_name = DisplayNamePosition(item.types);
				var service = WorkContext.Resolve<ILanguagesService>();
				var record = service.GetById(item.language_id);
				item.language_name = record.name;
			}
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
			result.FormWrapperStartHtml = CMSSolutions.Constants.Form.FormWrapperStartHtml;
			result.FormWrapperEndHtml = CMSSolutions.Constants.Form.FormWrapperEndHtml;
			result.RegisterExternalDataSource(x => x.language_id, y => BindLanguageList());
			result.RegisterExternalDataSource(x => x.types, y => EnumExtensions.GetListItems<Websites.Extensions.PositionType>());
            result.AddAction().HasText(this.T("Clear")).HasUrl(this.Url.Action("Edit", RouteData.Values.Merge(new { id = 0 }))).HasButtonStyle(ButtonStyle.Success);
            result.AddAction().HasText(this.T("Back")).HasUrl(this.Url.Action("Index")).HasButtonStyle(ButtonStyle.Danger);
            return result;
        }

		private IEnumerable<SelectListItem> BindLanguageList()
		{
			var service = WorkContext.Resolve<ILanguagesService>();
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
				return new AjaxResult().Alert(T(CMSSolutions.Constants.Messages.InvalidModel));
            }

			var text = "Create success";
            Questions item;
            if (model.Id == 0)
            {
                item = new Questions();
            }
            else
            {
                item = service.GetById(model.Id);
				text = "Update success.";
            }
            item.language_id = model.language_id;
            item.content = model.content;
            item.creator = WorkContext.CurrentUser.Id;
			item.types = string.Join(",", model.types);
            item.created_date = DateTime.Now;
			item.updated_date = DateTime.Now;
            service.Save(item);
			return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").Alert(text);
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
