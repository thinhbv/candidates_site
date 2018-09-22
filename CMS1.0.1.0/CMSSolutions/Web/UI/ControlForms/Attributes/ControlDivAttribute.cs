using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlDivAttribute : ControlFormAttribute
    {
        #region Overrides of ControlFormAttribute

        public override bool HasLabelControl
        {
            get { return ShowLabelControl; }
        }

        public bool ShowLabelControl { get; set; }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return string.IsNullOrEmpty(CssClass)
                ? "<div>" + Value + "</div>"
                : string.Format("<div class=\"{1}\">{0}</div>", Value, CssClass);
        }

        #endregion Overrides of ControlFormAttribute
    }
}