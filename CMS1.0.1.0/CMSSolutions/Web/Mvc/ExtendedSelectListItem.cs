using System.Web.Mvc;

namespace CMSSolutions.Web.Mvc
{
    public class ExtendedSelectListItem : SelectListItem
    {
        public object HtmlAttributes { get; set; }

        public string Category { get; set; }
    }
}