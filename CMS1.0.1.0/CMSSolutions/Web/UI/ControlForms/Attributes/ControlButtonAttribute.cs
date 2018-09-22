using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlButtonAttribute : ControlFormAttribute
    {
        public ControlButtonAttribute()
        {
            ButtonType = "button";
        }

        public bool Disable { get; set; }

        public string OnClick { get; set; }

        public string ButtonType { get; set; }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return string.Format("<button id=\"{7}\" type=\"{4}\" name=\"{1}\" class=\"{0}\" {3} {5} {6}>{2}</button>", CssClass, Name, LabelText, !string.IsNullOrEmpty(OnClick) ? string.Format("onclick=\"{0}\"", OnClick) : string.Empty, ButtonType, string.Format("data-bind='{0}'", DataBind), Disable ? "disabled" : "", htmlHelper.ViewData.TemplateInfo.GetFullHtmlFieldId(Name));
        }
    }
}