using System;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace CMSSolutions.Web.Mvc
{
    public static class ActionResultExtensions
    {
        public static string Capture(this ActionResult result, ControllerContext controllerContext)
        {
            using (var it = new ResponseCapture(controllerContext.RequestContext.HttpContext.Response))
            {
                result.ExecuteResult(controllerContext);
                return it.ToString();
            }
        }

        private class ResponseCapture : IDisposable
        {
            private readonly HttpResponseBase response;
            private readonly TextWriter originalWriter;
            private StringWriter localWriter;

            public ResponseCapture(HttpResponseBase response)
            {
                this.response = response;
                originalWriter = response.Output;
                localWriter = new StringWriter();
                response.Output = localWriter;
            }

            public override string ToString()
            {
                localWriter.Flush();
                return localWriter.ToString();
            }

            public void Dispose()
            {
                if (localWriter != null)
                {
                    localWriter.Dispose();
                    localWriter = null;
                    response.Output = originalWriter;
                }
            }
        }
    }
}