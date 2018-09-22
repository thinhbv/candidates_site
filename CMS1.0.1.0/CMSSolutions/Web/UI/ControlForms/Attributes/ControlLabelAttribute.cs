using System.Web;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlLabelAttribute : ControlFormAttribute
    {
        public bool ShowLabelControl { get; set; }

        public ControlLabelAttribute()
        {
            ShowLabelControl = true;
        }

        public override bool HasLabelControl
        {
            get
            {
                return ShowLabelControl;
            }
        }

        public override string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return controlForm.FormProvider.GenerateControlUI(controlForm, this, workContext, htmlHelper);
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return string.Format("<p class=\"{0}\">{1}</p><input type=\"hidden\" value=\"{1}\" name=\"{2}\" />", CssClass, HttpUtility.HtmlEncode(Value), Name);
        }
    }
}