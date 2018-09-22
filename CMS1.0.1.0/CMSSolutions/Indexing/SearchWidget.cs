using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.ContentManagement.Widgets;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Indexing
{
    [Feature(Constants.Areas.Indexing)]
    public class SearchWidget : WidgetBase
    {
        public override string Name
        {
            get { return "Search Form Widget"; }
        }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            var urlHelper = new UrlHelper(viewContext.RequestContext);

            if (!string.IsNullOrEmpty(CssClass))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            writer.Write("<form class=\"form-search\" method=\"get\" action=\"{0}\">", urlHelper.Action("Search", "Search", new { area = Constants.Areas.Indexing }));
            writer.Write("<i class=\"cx-icon cx-icon-search\"></i>");
            writer.Write("<input type=\"text\" class=\"input-medium search-query\" name=\"q\" value=\"{0}\" />", viewContext.HttpContext.Request.QueryString["q"]);
            writer.Write("<button type=\"submit\" class=\"btn\">Search</button>");
            writer.Write("</form>");

            writer.RenderEndTag(); // div
        }
    }
}