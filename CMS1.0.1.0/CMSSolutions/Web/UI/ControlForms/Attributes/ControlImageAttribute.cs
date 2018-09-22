using System.Text;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlImageAttribute : ControlFormAttribute
    {
        public string Width { get; set; }

        public string Height { get; set; }

        public string MaxHeight { get; set; }

        public override string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return controlForm.FormProvider.GenerateControlUI(controlForm, this, workContext, htmlHelper);
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            if (Value != null)
            {
                var sb = new StringBuilder();
                sb.AppendFormat("<div style=\"{0}\">", GenerateAttr("width", Width));
                sb.AppendFormat("<a href=\"{0}\" class=\"thumbnail\" target=\"_blank\">", Value);
                sb.AppendFormat("<img src=\"{0}\" alt=\"\" style=\"width: 100%;{1}{2}\" />", Value, GenerateAttr("height", Height), GenerateAttr("max-height", MaxHeight));
                sb.Append("</a>");
                sb.Append("</div>");

                return sb.ToString();
            }
            return null;
        }

        private static string GenerateAttr(string attr, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                return attr + ":" + value + ";";
            }
            return null;
        }
    }
}