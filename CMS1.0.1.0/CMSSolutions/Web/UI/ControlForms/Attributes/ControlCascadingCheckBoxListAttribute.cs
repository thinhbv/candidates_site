using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlCascadingCheckBoxListAttribute : ControlFormAttribute
    {
        public string ParentControl { get; set; }

        public override string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return controlForm.FormProvider.GenerateControlUI(controlForm, this, workContext, htmlHelper);
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            var clientId = "divcbl_" + Guid.NewGuid().ToString("N").ToLowerInvariant();
            var sourceUrl = controlForm.GetCascadingCheckBoxDataSource(Name);

            if (string.IsNullOrEmpty(ParentControl))
            {
                throw new ArgumentException("The ParentControl must be not null or empty.");
            }

            if (!typeof(IEnumerable).IsAssignableFrom(PropertyType))
            {
                throw new NotSupportedException("Cannot apply control choice for non enumerable property as checkbox list.");
            }

            var cssClass = (controlForm.FormProvider.ControlCssClass + " " + (CssClass ?? "checkbox")).Trim();

            var value = Value as IEnumerable;

            var values = new List<string>();
            string selectedItems = "";
            if (value != null)
            {
                values.AddRange(from object item in value select Convert.ToString(item));
                selectedItems = string.Join(",", values.ToArray());
            }

            var sb = new StringBuilder();

            sb.AppendFormat("<div class=\"row-fluid no-padding\" id=\"{0}\">", clientId);

            sb.Append("</div>");

            sb.Append("<script type=\"text/javascript\">");
            sb.Append("$(document).ready(function(){");
            sb.AppendFormat("$('#{0}').change(function(){{", htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(ParentControl));
            if (ReadOnly)
            {
                sb.AppendFormat("$.ajax({{url: '{0}', data: 'sender={2}&' + $(this.form).serialize(), type: 'POST', dataType: 'json', success: function(result){{ var control = $('#{1}'); control.empty(); if(!result || !result.length){{ return; }} var items = '{5}'; $.each(result, function(index, item){{ if(items.indexOf(item.Value) != -1){{control.append('<label class = \"{3}\"><input type=\"checkbox\" name=\"{4}\" value=\"'+ item.Value +'\" checked=\"checked\" disabled=\"disabled\">' + item.Text + '</label>');}} else{{control.append('<label class = \"{3}\"><input type=\"checkbox\" name=\"{4}\" value=\"'+ item.Value +'\" disabled=\"disabled\">' + item.Text + '</label>');}} }}); }} }});", sourceUrl, clientId, ParentControl, cssClass, Name, selectedItems);
            }
            else
            {
                sb.AppendFormat("$.ajax({{url: '{0}', data: 'sender={2}&' + $(this.form).serialize(), type: 'POST', dataType: 'json', success: function(result){{ var control = $('#{1}'); control.empty(); if(!result || !result.length){{ return; }} var items = '{5}'; $.each(result, function(index, item){{ if(items.indexOf(item.Value) != -1){{control.append('<label class = \"{3}\"><input type=\"checkbox\" name=\"{4}\" value=\"'+ item.Value +'\" checked=\"checked\">' + item.Text + '</label>');}} else{{control.append('<label class = \"{3}\"><input type=\"checkbox\" name=\"{4}\" value=\"'+ item.Value +'\">' + item.Text + '</label>');}} }}); }} }});", sourceUrl, clientId, ParentControl, cssClass, Name, selectedItems);
            }
            sb.Append("}).change();");
            sb.Append("});");
            sb.Append("</script>");

            return sb.ToString();
        }
    }
}