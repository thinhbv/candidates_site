using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web.Mvc.ControlForms
{
    public class ControlForm<TModel> : IHtmlString where TModel : class
    {
        private readonly HtmlHelper html;
        private readonly ControlFormResult<TModel> model;

        public ControlForm(HtmlHelper html)
        {
            model = new ControlFormResult<TModel>(html.ViewData.Model as TModel);
            this.html = html;
        }

        public ControlForm<TModel> AddHiddenValue(string name, string value)
        {
            model.AddHiddenValue(name, value);
            return this;
        }

        public ControlForm<TModel> CancelButtonCssClass(string cancelButtonCssClass)
        {
            model.CancelButtonCssClass = cancelButtonCssClass;
            return this;
        }

        public ControlForm<TModel> CancelButtonText(string cancelButtonText)
        {
            model.CancelButtonText = cancelButtonText;
            return this;
        }

        public ControlForm<TModel> CancelButtonUrl(string cancelButtonUrl)
        {
            model.CancelButtonUrl = cancelButtonUrl;
            return this;
        }

        public ControlForm<TModel> ClientId(string clientId)
        {
            model.ClientId = clientId;
            return this;
        }

        public ControlForm<TModel> CssClass(string cssClass)
        {
            model.CssClass = cssClass;
            return this;
        }

        public ControlForm<TModel> Description(string description)
        {
            model.Description = description;
            return this;
        }

        public ControlForm<TModel> EnableAjax(bool isAjaxSupported)
        {
            model.IsAjaxSupported = isAjaxSupported;
            return this;
        }

        public ControlForm<TModel> EnableKnockoutJs(bool enableKnockoutJs)
        {
            model.EnableKnockoutJs = enableKnockoutJs;
            return this;
        }

        public ControlForm<TModel> FormMethod(FormMethod formMethod)
        {
            model.FormMethod = formMethod;
            return this;
        }

        public ControlForm<TModel> FormProvider(ControlFormProvider formProvider)
        {
            model.FormProvider = formProvider;
            return this;
        }

        public ControlForm<TModel> Layout(ControlFormLayout layout)
        {
            model.Layout = layout;
            return this;
        }

        public ControlForm<TModel> OnClientSubmitButtonClick(string onClientSubmitButtonClick)
        {
            model.OnClientSubmitButtonClick = onClientSubmitButtonClick;
            return this;
        }

        public ControlForm<TModel> ReadOnly(bool readOnly)
        {
            model.ReadOnly = readOnly;
            return this;
        }

        public ControlForm<TModel> RegisterAutoCompleteDataSource<TValue>(Expression<Func<TModel, TValue>> expression, ControlAutoCompleteOptions<TModel> options)
        {
            model.RegisterAutoCompleteDataSource(expression, options);
            return this;
        }

        public ControlForm<TModel> RegisterAutoCompleteDataSource<TValue>(Expression<Func<TModel, TValue>> expression, string sourceUrl)
        {
            model.RegisterAutoCompleteDataSource(expression, sourceUrl);
            return this;
        }

        public ControlForm<TModel> RegisterAutoCompleteDataSource<TValue>(Expression<Func<TModel, TValue>> expression, string sourceUrl, Func<TModel, string> textSelector)
        {
            model.RegisterAutoCompleteDataSource(expression, sourceUrl, textSelector);
            return this;
        }

        public ControlForm<TModel> RegisterCascadingDropDownDataSource<TValue>(Expression<Func<TModel, TValue>> expression, string sourceUrl)
        {
            model.RegisterCascadingDropDownDataSource(expression, sourceUrl);
            return this;
        }

        public ControlForm<TModel> RegisterCascadingDropDownDataSource<TValue>(Expression<Func<TModel, TValue>> expression, ControlCascadingDropDownOptions options)
        {
            model.RegisterCascadingDropDownDataSource(expression, options);
            return this;
        }

        public ControlForm<TModel> RegisterCascadingDropDownDataSource(string property, string sourceUrl)
        {
            model.RegisterCascadingDropDownDataSource(property, sourceUrl);
            return this;
        }

        public ControlForm<TModel> RegisterCascadingDropDownDataSource(string property, ControlCascadingDropDownOptions options)
        {
            model.RegisterCascadingDropDownDataSource(property, options);
            return this;
        }

        public ControlForm<TModel> RegisterExternalDataSource<TProperty>(Expression<Func<TModel, TProperty>> expression, Func<TModel, IEnumerable<SelectListItem>> items)
        {
            model.RegisterExternalDataSource(expression, items);
            return this;
        }

        public ControlForm<TModel> RegisterExternalDataSource<TProperty>(Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> items)
        {
            model.RegisterExternalDataSource(expression, items);
            return this;
        }

        public ControlForm<TModel> RegisterExternalDataSource<TProperty>(Expression<Func<TModel, TProperty>> expression, params string[] values)
        {
            model.RegisterExternalDataSource(expression, values);
            return this;
        }

        public ControlForm<TModel> RegisterExternalDataSource<TProperty, TKey>(Expression<Func<TModel, TProperty>> expression, IDictionary<TKey, string> values)
        {
            model.RegisterExternalDataSource(expression, values);
            return this;
        }

        public ControlForm<TModel> RegisterExternalDataSource(string property, Func<TModel, IEnumerable<SelectListItem>> items)
        {
            model.RegisterExternalDataSource(property, items);
            return this;
        }

        public ControlForm<TModel> RegisterExternalDataSource(string property, params string[] values)
        {
            model.RegisterExternalDataSource(property, values);
            return this;
        }

        public ControlForm<TModel> RegisterExternalDataSource(string property, IEnumerable<string> values)
        {
            model.RegisterExternalDataSource(property, values);
            return this;
        }

        public ControlForm<TModel> RegisterExternalDataSource<TKey>(string property, IDictionary<TKey, string> values)
        {
            model.RegisterExternalDataSource(property, values);
            return this;
        }

        public ControlForm<TModel> RegisterExternalDataSource<TKey>(string property, Dictionary<TKey, string> dataSourceValues)
        {
            model.RegisterExternalDataSource(property, dataSourceValues);
            return this;
        }

        public ControlForm<TModel> RegisterExternalDataSource(string property, IEnumerable<SelectListItem> items)
        {
            model.RegisterExternalDataSource(property, items);
            return this;
        }

        public ControlForm<TModel> RegisterFileUploadOptions<TValue>(Expression<Func<TModel, TValue>> expression, ControlFileUploadOptions options)
        {
            model.RegisterFileUploadOptions(expression, options);
            return this;
        }

        public ControlForm<TModel> RegisterFileUploadOptions(string property, string uploadUrl)
        {
            model.RegisterFileUploadOptions(property, uploadUrl);
            return this;
        }

        public ControlForm<TModel> RegisterFileUploadOptions(string property, ControlFileUploadOptions options)
        {
            model.RegisterFileUploadOptions(property, options);
            return this;
        }

        public ControlForm<TModel> ShowCancelButton(bool showCancelButton)
        {
            model.ShowCancelButton = showCancelButton;
            return this;
        }

        public ControlForm<TModel> ShowSubmitButton(bool showSubmitButton)
        {
            model.ShowSubmitButton = showSubmitButton;
            return this;
        }

        public ControlForm<TModel> ShowValidationSummary(bool showValidationSummary)
        {
            model.ShowValidationSummary = showValidationSummary;
            return this;
        }

        public ControlForm<TModel> SubmitButtonCssClass(string submitButtonCssClass)
        {
            model.SubmitButtonCssClass = submitButtonCssClass;
            return this;
        }

        public ControlForm<TModel> SubmitButtonText(string submitButtonText)
        {
            model.SubmitButtonText = submitButtonText;
            return this;
        }

        public ControlForm<TModel> SubmitButtonValue(string submitButtonValue)
        {
            model.SubmitButtonValue = submitButtonValue;
            return this;
        }

        public ControlForm<TModel> Title(string title)
        {
            model.Title = title;
            return this;
        }

        public string ToHtmlString()
        {
            return model.GenerateControlFormUI(html.ViewContext.Controller.ControllerContext);
        }

        public ControlForm<TModel> UpdateActionName(string updateActionName)
        {
            model.UpdateActionName = updateActionName;
            return this;
        }

        public ControlForm<TModel> ValidationSupport(bool validationSupport)
        {
            model.ValidationSupport = validationSupport;
            return this;
        }
    }
}