using System;
using System.Web.Mvc;
using Newtonsoft.Json;

namespace CMSSolutions.Web.Mvc
{
    public class JsonDataContractResult : JsonResult
    {
        public JsonDataContractResult(object data)
        {
            Data = data;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }

            if (JsonRequestBehavior == JsonRequestBehavior.DenyGet &&
                String.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            {
                throw new InvalidOperationException("Get is not allowed");
            }

            var response = context.HttpContext.Response;

            response.ContentType = !string.IsNullOrEmpty(ContentType) ? ContentType : "application/json";
            if (ContentEncoding != null)
            {
                response.ContentEncoding = ContentEncoding;
            }

            if (Data != null)
            {
                var serializer = new JsonSerializer();
                serializer.Serialize(response.Output, Data);
            }
        }
    }
}