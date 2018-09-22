using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class IconFontPickerAttribute : ControlFormAttribute
    {
        public IconFontPickerAttribute()
        {
            IconSet = "glyphicon";
            Placement = "right";
            Rows = 3;
            Columns = 6;
        }

        /// <summary>
        /// The icon set, glyphicon or fontawesome
        /// </summary>
        public string IconSet { get; set; }

        public string Placement { get; set; }
        
        public int Rows { get; set; }
        
        public int Columns { get; set; }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            yield return ResourceType.IcontFontPicker;
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            var buttonTag = new TagBuilder("button");
            if (!string.IsNullOrEmpty(CssClass))
            {
                buttonTag.AddCssClass(CssClass);
            }

            if (Value != null)
            {
                buttonTag.Attributes.Add("data-icon", Value.ToString());    
            }

            buttonTag.Attributes.Add("id", controlForm.ViewData.TemplateInfo.GetFullHtmlFieldId(Name));
            buttonTag.Attributes.Add("name", controlForm.ViewData.TemplateInfo.GetFullHtmlFieldName(Name));
            buttonTag.Attributes.Add("role", "iconpicker");
            buttonTag.Attributes.Add("data-iconset", IconSet);
            buttonTag.Attributes.Add("data-placement", Placement);
            buttonTag.Attributes.Add("data-rows", Convert.ToString(Rows));
            buttonTag.Attributes.Add("data-cols", Convert.ToString(Columns));
            return buttonTag.ToString();
        }
    }
}
