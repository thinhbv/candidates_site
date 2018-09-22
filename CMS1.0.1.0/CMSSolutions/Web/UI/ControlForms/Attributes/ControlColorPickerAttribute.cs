using System.Collections.Generic;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlColorPickerAttribute : ControlChoiceAttribute
    {
        public ControlColorPickerAttribute()
            : base(ControlChoice.DropDownList)
        {
        }

        /// <summary>
        /// Show the colors inside a picker instead of inline, default: False
        /// </summary>
        public bool Picker { get; set; }

        /// <summary>
        /// Font to use for the ok/check mark, default: '', available themes: regularfont, fontawesome, glyphicons
        /// </summary>
        public string Theme { get; set; }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            yield return ResourceType.ColorPicker;
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            var scriptRegister = new ScriptRegister(workContext);
            var clientId = controlForm.ViewData.TemplateInfo.GetFullHtmlFieldId(Name);
            scriptRegister.IncludeInline(string.Format("$('#{0}').simplecolorpicker({{ picker: {1}, theme: '{2}' }});", clientId, Picker.ToString().ToLowerInvariant(), Theme));

            return base.SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }
    }
}
