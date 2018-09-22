using System.Web;

namespace CMSSolutions.Web.Fakes
{
    public class FakeHttpContext : HttpContextWrapper
    {
        private readonly HttpResponseBase response;

        public FakeHttpContext()
            : base(new HttpContext(new FakeHttpWorkerRequest()))
        {
        }

        public FakeHttpContext(HttpContextBase httpContext)
            : base(httpContext.ApplicationInstance.Context)
        {
            response = new FakeHttpResponse();
        }

        public override HttpResponseBase Response
        {
            get { return response ?? base.Response; }
        }
    }
}