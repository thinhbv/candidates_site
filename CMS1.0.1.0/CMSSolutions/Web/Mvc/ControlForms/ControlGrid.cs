using System;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web.Mvc.ControlForms
{
    public class ControlGrid<TModel> : IHtmlString where TModel : class
    {
        private readonly HtmlHelper html;
        private readonly ControlGridFormResult<TModel> model;

        public ControlGrid(HtmlHelper html)
        {
            model = new ControlGridFormResult<TModel>();
            this.html = html;
        }

        public ControlGrid<TModel> Title(string title)
        {
            model.Title = title;
            return this;
        }

        public ControlGrid<TModel> Description(string description)
        {
            model.Description = description;
            return this;
        }

        public ControlGrid<TModel> FormProvider(ControlFormProvider formProvider)
        {
            model.FormProvider = formProvider;
            return this;
        }

        public ControlGrid<TModel> AddHiddenValue(string name, string value)
        {
            model.AddHiddenValue(name, value);
            return this;
        }

        public ControlGrid<TModel> ClientId(string clientId)
        {
            model.ClientId = clientId;
            return this;
        }

        public ControlGrid<TModel> CssClass(string cssClass)
        {
            model.CssClass = cssClass;
            return this;
        }

        public ControlGrid<TModel> UpdateActionName(string updateActionName)
        {
            model.UpdateActionName = updateActionName;
            return this;
        }

        public ControlGrid<TModel> EnableAjax(bool isAjaxSupported)
        {
            model.IsAjaxSupported = isAjaxSupported;
            return this;
        }

        public ControlGrid<TModel> EnableSearch(bool enableSearch)
        {
            model.EnableSearch = enableSearch;
            return this;
        }

        public ControlGrid<TModel> EnableSorting(bool enableSorting)
        {
            model.EnableSorting = enableSorting;
            return this;
        }

        public ControlGrid<TModel> FetchAjaxSource(Func<ControlGridFormRequest, ControlGridAjaxData<TModel>> fetchAjaxSource)
        {
            model.FetchAjaxSource = fetchAjaxSource;
            return this;
        }

        public ControlGrid<TModel> EnablePaginate(bool enablePaginate)
        {
            model.EnablePaginate = enablePaginate;
            return this;
        }

        public ControlGrid<TModel> EnablePageSizeChange(bool enablePageSizeChange)
        {
            model.EnablePageSizeChange = enablePageSizeChange;
            return this;
        }

        public ControlGrid<TModel> DefaultPageSize(int defaultPageSize)
        {
            model.DefaultPageSize = defaultPageSize;
            return this;
        }

        public ControlGrid<TModel> HideActionsColumn(bool hideActionsColumn)
        {
            model.HideActionsColumn = hideActionsColumn;
            return this;
        }

        public ControlGrid<TModel> AddColumn<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            model.AddColumn(expression);
            return this;
        }

        public ControlGrid<TModel> AddColumn<TValue>(Expression<Func<TModel, TValue>> expression, string headerText)
        {
            model.AddColumn(expression, headerText);
            return this;
        }

        public ControlGrid<TModel> AddAction(ControlFormAction action)
        {
            model.AddAction(action);
            return this;
        }

        public ControlGrid<TModel> AddRowAction(ControlGridFormRowAction<TModel> action)
        {
            model.AddRowAction(action);
            return this;
        }

        public ControlGrid<TModel> AddReloadEvent(string eventName)
        {
            model.AddReloadEvent(eventName);
            return this;
        }

        public ControlGrid<TModel> AddCustomVar(string name, object value, bool isFunction = false)
        {
            model.AddCustomVar(name, value, isFunction);
            return this;
        }

        public ControlGrid<TModel> GetRecordsUrl(string url)
        {
            model.GetRecordsUrl = url;
            return this;
        }

        public ControlGrid<TModel> EnableCheckboxes(bool value = true)
        {
            model.EnableCheckboxes = value;
            return this;
        }

        public string ToHtmlString()
        {
            return model.GenerateControlFormUI(html.ViewContext.Controller.ControllerContext);
        }
    }
}