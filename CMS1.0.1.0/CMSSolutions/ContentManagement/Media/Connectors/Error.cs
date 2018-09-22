using System.Web.Mvc;
using CMSSolutions.Web.Mvc;

namespace CMSSolutions.ContentManagement.Media.Connectors
{
    internal static class Error
    {
        public static JsonResult CommandNotFound()
        {
            return Json(new { error = "errUnknownCmd" });
        }

        public static JsonResult MissedParameter(string command)
        {
            return Json(new { error = new[] { "errCmdParams", command } });
        }

        private static JsonResult Json(object data)
        {
            return new JsonDataContractResult(data) { JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}