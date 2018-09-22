using System.IO;
using System.Text;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlViewResult : BaseControlFormResult
    {
        public object ViewModel { get; set; }

        public ControlViewResult(string viewName, object model = null)
        {
            ViewName = viewName;
            ViewModel = model;
        }

        public override string GenerateControlFormUI(ControllerContext controllerContext)
        {
            var result = ViewEngines.Engines.FindPartialView(controllerContext, ViewName);

            if (result != null && result.View != null)
            {
                var viewData = new ViewDataDictionary();

                if (ViewModel != null)
                {
                    viewData.Model = ViewModel;
                }

                var sb = new StringBuilder();
                var textWriter = new StringWriter(sb);
                var viewContext = new ViewContext(controllerContext, result.View, viewData, new TempDataDictionary(), textWriter);
                result.View.Render(viewContext, textWriter);
                return sb.ToString();
            }

            return null;
        }
    }
}