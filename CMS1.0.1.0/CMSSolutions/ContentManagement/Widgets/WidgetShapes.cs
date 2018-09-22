using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.DisplayManagement;
using CMSSolutions.DisplayManagement.Descriptors;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Widgets
{
    [Feature(Constants.Areas.Widgets)]
    public class WidgetShapes : IShapeTableProvider
    {
        private readonly IWorkContextAccessor workContextAccessor;

        public WidgetShapes(IWorkContextAccessor workContextAccessor)
        {
            this.workContextAccessor = workContextAccessor;
        }

        public void Discover(ShapeTableBuilder builder)
        {
            builder.Describe("Widget")
                .Configure(descriptor => descriptor.Wrappers.Add("Widget_Wrapper"))
                .OnDisplaying(displaying =>
                {
                    var widget = displaying.Shape;
                    widget.Classes.Add("widget");
                });
        }

        #region Shapes

        [Shape]
        // ReSharper disable InconsistentNaming
        public void Widget(ViewContext ViewContext, dynamic Shape, IWidget Widget, TextWriter Output)
        // ReSharper restore InconsistentNaming
        {
            //string id = Shape.Id;
            var classes = new List<string>((IEnumerable<string>)Shape.Classes);

            if (!string.IsNullOrEmpty(Widget.CssClass))
            {
                classes.Add(Widget.CssClass);
            }
            var htmlTextWriter = new HtmlTextWriter(Output);
            Widget.BuildDisplay(htmlTextWriter, ViewContext, workContextAccessor);
        }

        #endregion Shapes
    }
}