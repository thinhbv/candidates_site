using System.Web.UI;

namespace CMSSolutions.Extensions
{
    public static class HtmlTextWriterExtensions
    {
        public static void AddStyleAttributeIfHave(this HtmlTextWriter writer, HtmlTextWriterStyle key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.AddStyleAttribute(key, value);
            }
        }

        public static void AddStyleAttributeIfHave(this HtmlTextWriter writer, string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                writer.AddStyleAttribute(key, value);
            }
        }
    }
}