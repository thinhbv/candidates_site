using System;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlAutoCompleteAttribute : ControlFormAttribute
    {
        public ControlAutoCompleteAttribute()
        {
            MinLength = 2;
        }

        public int MinLength { get; set; }

        public bool MustMatch { get; set; }

        public ControlAutoCompleteOptions Options { get; set; }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            yield return ResourceType.JQueryUI;
        }

        public override string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return controlForm.FormProvider.GenerateControlUI(controlForm, this, workContext, htmlHelper);
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            if (controlForm.ReadOnly)
            {
                return null;
            }

            var options = Options ?? controlForm.GetAutoCompleteDataSource(Name);

            var attributes = new RouteValueDictionary();

            var cssClass = (controlForm.FormProvider.ControlCssClass + " " + CssClass).Trim();
            if (!string.IsNullOrEmpty(cssClass))
            {
                attributes.Add("class", cssClass);
            }

            if (ReadOnly)
            {
                attributes.Add("readonly", "readonly");
                return htmlHelper.TextBox(Name, null, attributes).ToHtmlString();
            }

            attributes.Add("data-val", "true");

            if (Required)
            {
                attributes.Add("data-val-required", Constants.Messages.Validation.Required);
            }

            attributes.Add("data-jqui-type", "autocomplete");
            attributes.Add("data-jqui-acomp-source", options.SourceUrl);
            attributes.Add("autocomplete", "off");

            if (MinLength > 0)
            {
                attributes.Add("data-jqui-acomp-minlength", MinLength);
            }

            string onChangeFunc = null;

            if (MustMatch)
            {
                var onChangeFuncName = "onAutoCompleteChange_" + Guid.NewGuid().ToString("N");
                onChangeFunc = string.Format("<script type=\"text/javascript\">function {0}(event, ui) {{ if(!ui.item){{ $(this).val(''); }} }}</script>", onChangeFuncName);
                attributes.Add("data-jqui-acomp-change", onChangeFuncName);
            }

            if (options.HasTextSelector == false)
            {
                var valMsg = string.Format("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", Name);
                return string.Join("", new[] { onChangeFunc, htmlHelper.TextBox(Name, null, attributes).ToHtmlString(), valMsg });
            }
            else
            {
                var valMsg = string.Format("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", Name + "_Text");
                var textValue = options.GetText(controlForm.Model);
                attributes.Add("data-jqui-acomp-hiddenvalue", Name);
                var autoCompleteTextControl = htmlHelper.TextBox(Name + "_Text", textValue, attributes);
                var autoCompleteValueControl = htmlHelper.Hidden(Name);
                return string.Join("", new[] { onChangeFunc, autoCompleteTextControl.ToHtmlString(), valMsg, autoCompleteValueControl.ToHtmlString() });
            }
        }
    }
}