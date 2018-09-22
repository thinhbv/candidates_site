using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Widgets
{
    [Feature(Constants.Areas.Widgets)]
    public class HtmlWidget : WidgetBase
    {
        public override string Name
        {
            get { return "Html Widget"; }
        }

        [AllowHtml]
        [ControlText(Type = ControlText.RichText, LabelText = "Html Content", ContainerCssClass = "col-xs-12 col-sm-12", ContainerRowIndex = 3)]
        public string ContentBody { get; set; }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            if (!string.IsNullOrEmpty(CssClass))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (ShowTitleOnPage)
            {
                writer.Write("<header><h3>{0}</h3></header>", Title);
            }

            writer.Write(ContentBody);

            writer.RenderEndTag(); // div
        }
    }
}