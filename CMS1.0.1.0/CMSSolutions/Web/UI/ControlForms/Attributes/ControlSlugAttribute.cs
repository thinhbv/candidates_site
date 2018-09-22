using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Mvc.Html;
using System.Collections.Generic;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlSlugAttribute : ControlFormAttribute
    {
        public int MaxLength { get; set; }

        public override string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return controlForm.FormProvider.GenerateControlUI(controlForm, this, workContext, htmlHelper);
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            var id = htmlHelper.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldId(Name);
            var attributes = new RouteValueDictionary();

            var cssClass = (controlForm.FormProvider.ControlCssClass + " " + CssClass).Trim();
            if (!string.IsNullOrEmpty(cssClass))
            {
                attributes.Add("class", cssClass);
            }

            if (MaxLength > 0)
            {
                attributes.Add("maxlength", MaxLength);
            }

            attributes.Add("id", id);
            attributes.Add("name", Name);
            attributes.Add("value", Value);
            attributes.Add("type", "text");
            attributes.Add("readonly", "readonly");
            attributes.Add("style", "width: 100%;");

            if (ReadOnly || controlForm.ReadOnly)
            {
                return htmlHelper.TextBox(Name, Value, (IDictionary<string, object>)attributes).ToHtmlString();
            }

            var tagBuilder = new TagBuilder("input");
            tagBuilder.MergeAttributes(attributes);

            return string.Format("{0}<button class=\"control-slug-trigger\" type=\"button\" onclick=\"var $this = $(this).prev(); $this.attr('readonly') ? $this.removeAttr('readonly') : $this.attr('readonly', 'readonly');\"></button>", tagBuilder.ToString(TagRenderMode.SelfClosing));
        }
    }
}