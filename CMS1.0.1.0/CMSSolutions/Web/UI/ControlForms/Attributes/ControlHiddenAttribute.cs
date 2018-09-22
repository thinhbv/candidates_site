using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlHiddenAttribute : ControlFormAttribute
    {
        public override bool HasLabelControl
        {
            get { return false; }
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            var attrs = new Dictionary<string, object>();
            if (!string.IsNullOrEmpty(DataBind))
            {
                attrs.Add("data-bind", DataBind);
            }
            return htmlHelper.Hidden(Name, Value, attrs).ToHtmlString();
        }
    }
}