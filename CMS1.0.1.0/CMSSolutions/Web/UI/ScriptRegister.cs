using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using System.Web.WebPages;
using CMSSolutions.Web.Mvc.Spooling;

namespace CMSSolutions.Web.UI
{
    public class ScriptRegister : ResourceRegister
    {
        private readonly WebPageBase viewPage;
        private readonly UrlHelper urlHelper;

        public ScriptRegister(WorkContext workContext, WebPageBase viewPage = null)
            : base(workContext)
        {
            this.viewPage = viewPage;
            urlHelper = workContext.Resolve<UrlHelper>();
        }

        protected override string VirtualBasePath
        {
            get { return "~/Scripts"; }
        }

        protected override string ResourceType
        {
            get { return "text/javascript"; }
        }

        public override void IncludeInline(string code, bool ignoreExists = false)
        {
            base.IncludeInline("<script type=\"text/javascript\">" + code + "</script>", ignoreExists);
        }

        protected override string BuildResource(string url)
        {
            return string.Format("<script type=\"text/javascript\" src=\"{0}\"></script>", urlHelper.Content(url));
        }

        protected override string BuildInlineResources(IEnumerable<string> resources)
        {
            return string.Format("{0}{1}{0}", System.Environment.NewLine, string.Join(System.Environment.NewLine, resources));
        }

        public IDisposable AtFoot()
        {
            return new CaptureScope(viewPage, s => base.IncludeInline(s.ToHtmlString()));
        }

        private class CaptureScope : IDisposable
        {
            private readonly WebPageBase viewPage;
            private readonly Action<IHtmlString> callback;

            public CaptureScope(WebPageBase viewPage, Action<IHtmlString> callback)
            {
                this.viewPage = viewPage;
                this.callback = callback;
                viewPage.OutputStack.Push(new HtmlStringWriter());
            }

            void IDisposable.Dispose()
            {
                var writer = (HtmlStringWriter)viewPage.OutputStack.Pop();
                callback(writer);
            }
        }
    }
}