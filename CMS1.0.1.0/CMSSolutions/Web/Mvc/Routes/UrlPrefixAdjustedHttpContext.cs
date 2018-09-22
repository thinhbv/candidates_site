using System.Web;
using System.Web.SessionState;

namespace CMSSolutions.Web.Mvc.Routes
{
    public class UrlPrefixAdjustedHttpContext : HttpContextBaseWrapper
    {
        private readonly UrlPrefix prefix;

        public UrlPrefixAdjustedHttpContext(HttpContextBase httpContextBase, UrlPrefix prefix)
            : base(httpContextBase)
        {
            this.prefix = prefix;
        }

        public override HttpRequestBase Request
        {
            get
            {
                return new AdjustedRequest(HttpContextBase.Request, prefix);
            }
        }

        public override void SetSessionStateBehavior(SessionStateBehavior sessionStateBehavior)
        {
            HttpContextBase.SetSessionStateBehavior(sessionStateBehavior);
        }

        private class AdjustedRequest : HttpRequestBaseWrapper
        {
            private readonly UrlPrefix urlPrefix;

            public AdjustedRequest(HttpRequestBase httpRequestBase, UrlPrefix urlPrefix)
                : base(httpRequestBase)
            {
                this.urlPrefix = urlPrefix;
            }

            public override string AppRelativeCurrentExecutionFilePath
            {
                get
                {
                    return urlPrefix.RemoveLeadingSegments(HttpRequestBase.AppRelativeCurrentExecutionFilePath);
                }
            }
        }
    }
}