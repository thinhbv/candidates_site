using System.Web;

namespace CMSSolutions.ContentManagement.Media.Connectors
{
    internal static class Helper
    {
        public static string EncodePath(string path)
        {
            return HttpServerUtility.UrlTokenEncode(System.Text.Encoding.UTF8.GetBytes(path));
        }

        public static string DecodePath(string path)
        {
            return System.Text.Encoding.UTF8.GetString(HttpServerUtility.UrlTokenDecode(path));
        }
    }
}