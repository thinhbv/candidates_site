using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using CMSSolutions.Extensions;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlCascadingDropDownAttribute : ControlFormAttribute
    {
        public string ParentControl { get; set; }

        public bool AbsoluteParentControl { get; set; }

        public bool AllowMultiple { get; set; }

        public bool EnableChosen { get; set; }

        public string OnSelectedIndexChanged { get; set; }

        public string OnSuccess { get; set; }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            if (EnableChosen)
            {
                yield return ResourceType.Chosen;
            }
        }

        public override string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return controlForm.FormProvider.GenerateControlUI(controlForm, this, workContext, htmlHelper);
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            var options = controlForm.GetCascadingDropDownDataSource(Name.RemoveBetween('[', ']'));

            var attributes = new RouteValueDictionary();
            if (ReadOnly || controlForm.ReadOnly)
            {
                attributes = new RouteValueDictionary { { "disabled", "disabled" } };
            }

            var parentControl = options.ParentControl ?? ParentControl;

            if (string.IsNullOrEmpty(parentControl))
            {
                throw new ArgumentException("The ParentControl must be not null or empty.");
            }

            if (!AbsoluteParentControl)
            {
                parentControl = Name.Replace(Name.Split('.').Last(), parentControl);
            }

            var cssClass = (controlForm.FormProvider.ControlCssClass + " " + CssClass).Trim();
            if (!string.IsNullOrEmpty(cssClass))
            {
                attributes.Add("class", cssClass);
            }

            if (Required || !string.IsNullOrEmpty(RequiredIf))
            {
                attributes.Add("data-val", "true");

                if (!string.IsNullOrEmpty(RequiredIf))
                {
                    if (Name.Contains("."))
                    {
                        var dependency = RequiredIf.Split(':').First();
                        var requiredIf = RequiredIf.Replace(dependency, Name.Replace(Name.Split('.').Last(), dependency));
                        attributes.Add("data-val-requiredif", "#" + htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(requiredIf.TrimStart('#')));
                    }
                    else
                    {
                        attributes.Add("data-val-requiredif", "#" + RequiredIf.TrimStart('#'));
                    }
                }
                else
                {
                    attributes.Add("data-val-required", Constants.Messages.Validation.Required);
                }
            }

            if (AllowMultiple)
            {
                attributes.Add("multiple", "multiple");
            }

            if (!string.IsNullOrEmpty(OnSelectedIndexChanged))
            {
                attributes.Add("onchange", OnSelectedIndexChanged);
            }

            if (Value != null)
            {
                attributes.Add("data-value", Value);
            }

            var clientId = controlForm.ViewData.TemplateInfo.GetFullHtmlFieldId(Name);
            var parentControlId = htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(parentControl);

            var sb = new StringBuilder();

            sb.AppendFormat("$('#{0}').change(function(){{", parentControlId);

            sb.Append("if($(this).is(':hidden')){ return; }");

            if (EnableChosen)
            {
                if (AllowMultiple)
                {
                    var multilValue = "";
                    if (Value != null)
                    {
                        var arr = ((IEnumerable)Value).Cast<object>()
                                 .Select(x => x.ToString())
                                 .ToArray();
                        multilValue = string.Format("[{0}]", string.Join(",", arr));
                    }

                    if (string.IsNullOrEmpty(multilValue))
                    {
                        sb.AppendFormat("$.ajax({{url: '{0}', data: 'sender={3}&command={2}&' + $(this.form).serialize(), type: 'POST', dataType: 'json'," +
                                    " success: function(result){{ {5} " +
                                    "var control = $('#{1}');" +
                                    " var oldValue = control.data('value');" +
                                    "control.empty(); " +
                                    "if(!result || !result.length){{ return; }}" +
                                    " $.each(result, function(index, item){{ " +
                                    "control.append($('<option></option>').attr('value', item.Value).text(item.Text)); }}); " +
                                    "if(oldValue){{ control.val(oldValue); }} " +
                                    "control.change(); " +
                                    "if ($('#{1}').attr('data-value')!== undefined) {{" +
                                    " $('#{1}').val({4}).trigger(\"liszt:updated\"); " +
                                    "$('#{1}').removeAttr('data-value');}}" +
                                    " $('#{1}').trigger(\"chosen:updated\"); }} }});",
                        options.SourceUrl, clientId, options.Command, Name, multilValue, OnSuccess);
                    }
                    else
                    {
                        sb.AppendFormat("$.ajax({{url: '{0}', data: 'sender={3}&command={2}&' + $(this.form).serialize(), type: 'POST', dataType: 'json'," +
                                    " success: function(result){{ {5} " +
                                    "var control = $('#{1}');" +
                                    " var oldValue = {4};" +
                                    "control.empty(); " +
                                    "if(!result || !result.length){{ return; }}" +
                                    " $.each(result, function(index, item){{ " +
                                    "control.append($('<option></option>').attr('value', item.Value).text(item.Text)); }}); " +
                                    "if(oldValue){{ control.val(oldValue); }} " +
                                    "control.change(); " +
                                    "if ($('#{1}').attr('data-value')!== undefined) {{" +
                                    " $('#{1}').val({4}).trigger(\"liszt:updated\"); " +
                                    "$('#{1}').removeAttr('data-value');}}" +
                                    " $('#{1}').trigger(\"chosen:updated\"); }} }});",
                        options.SourceUrl, clientId, options.Command, Name, multilValue, OnSuccess);
                    }
                }
                else
                {
                    sb.AppendFormat("$.ajax({{url: '{0}', data: 'sender={3}&command={2}&' + $(this.form).serialize(), type: 'POST', dataType: 'json', success: function(result){{ {4} var control = $('#{1}'); var oldValue = control.data('value'); control.empty(); if(!result || !result.length){{ return; }} $.each(result, function(index, item){{ control.append($('<option></option>').attr('value', item.Value).text(item.Text)); }}); if(oldValue){{ control.val(oldValue); }} control.change(); $('#{1}').trigger(\"chosen:updated\"); }} }});", options.SourceUrl, clientId, options.Command, Name, OnSuccess);
                }
            }
            else
            {
                sb.AppendFormat("$.ajax({{url: '{0}', data: 'sender={3}&command={2}&' + $(this.form).serialize(), type: 'POST', dataType: 'json', success: function(result){{ {4} var control = $('#{1}'); var oldValue = control.data('value'); control.empty(); if(!result || !result.length){{ return; }} $.each(result, function(index, item){{ control.append($('<option></option>').attr('value', item.Value).text(item.Text)); }}); if(oldValue){{ control.val(oldValue); }} control.change(); }} }});", options.SourceUrl, clientId, options.Command, parentControl, OnSuccess);
            }

            sb.Append("});");

            if (EnableChosen)
            {
                sb.AppendFormat("$('#{0}').chosen({{ no_results_text: \"No results matched\", allow_single_deselect:true }});", clientId);
            }

            var scriptRegister = new ScriptRegister(workContext);
            scriptRegister.IncludeInline(sb.ToString());

            // Trigger parent control change once time only
            scriptRegister.IncludeInline(string.Format("$('#{0}').change();", parentControlId), true);

            var valMsg = string.Format("<span data-valmsg-for=\"{0}\" data-valmsg-replace=\"true\"></span>", Name);

            return string.Join("", new[] { htmlHelper.DropDownList(Name, new List<SelectListItem>(), attributes).ToHtmlString(), valMsg });
        }
    }
}