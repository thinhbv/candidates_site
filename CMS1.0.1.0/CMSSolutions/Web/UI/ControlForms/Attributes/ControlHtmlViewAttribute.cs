using System.IO;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlHtmlViewAttribute : ControlFormAttribute
    {
        public ControlHtmlViewAttribute(string viewName)
        {
            ViewName = viewName;
        }

        public string ViewName { get; set; }

        public object Model { get; set; }

        public override bool HasLabelControl
        {
            get { return false; }
        }

        public override string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            return SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }

        public override string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
        {
            var controllerContext = new ControllerContext
            {
                RouteData = htmlHelper.ViewContext.RouteData,
                HttpContext = htmlHelper.ViewContext.HttpContext
            };
            var result = ViewEngines.Engines.FindPartialView(controllerContext, ViewName);

            if (result != null && result.View != null)
            {
                using (var writer = new StringWriter())
                {
                    var viewData = new ViewDataDictionary(htmlHelper.ViewData);
                    var viewContext = new ViewContext(controllerContext, result.View, viewData, new TempDataDictionary(), writer);
                    viewData.Model = Model ?? Value;

                    result.View.Render(viewContext, writer);
                    return writer.ToString();
                }
            }

            return null;
        }
    }
}