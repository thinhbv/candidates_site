using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Widgets
{
    [Feature(Constants.Areas.Widgets)]
    public class ViewWidget : WidgetBase
    {
        public override string Name
        {
            get { return "View Widget"; }
        }

        public override bool HasTitle
        {
            get { return false; }
        }

        [ControlText(Required = true, MaxLength = 255, LabelText = "View Name", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = 3)]
        public string ViewName { get; set; }

        public object Model { get; set; }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            if (string.IsNullOrEmpty(ViewName))
            {
                return;
            }

            var controllerContext = new ControllerContext
                                        {
                                            RouteData = viewContext.RouteData,
                                            HttpContext = viewContext.HttpContext
                                        };

            var result = ViewEngines.Engines.FindPartialView(controllerContext, ViewName);

            if (result != null && result.View != null)
            {
                var childViewContext = new ViewContext(controllerContext, result.View, new ViewDataDictionary(), new TempDataDictionary(), writer);
                childViewContext.ViewData.Model = Model;
                result.View.Render(childViewContext, writer);
            }
        }
    }
}