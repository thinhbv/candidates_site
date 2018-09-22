using System;
using System.IO;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.Web.Fakes;

namespace CMSSolutions.ContentManagement.Pages
{
    public class ContentViewResult : ViewResult
    {
        public string Title { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public string BodyContent { get; set; }

        protected override ViewEngineResult FindView(ControllerContext context)
        {
            return ViewEngineCollection.FindView(context, "ControlFormResult_", null);
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.Controller.ViewBag.Title = Title;
            var fakeHttpContext = new FakeHttpContext(context.HttpContext);
            var fakeContext = new ControllerContext(fakeHttpContext, context.RouteData, context.Controller);
            ViewData = context.Controller.ViewData;
            base.ExecuteResult(fakeContext);

            using (var reader = new StreamReader(fakeHttpContext.Response.OutputStream))
            {
                var str = reader.ReadToEnd();
                var sb = new StringBuilder(str);

                // Add metas
                if (!string.IsNullOrEmpty(MetaKeywords) || !string.IsNullOrEmpty(MetaDescription))
                {
                    var indexOfTitle = str.IndexOf("</title>", StringComparison.InvariantCultureIgnoreCase);
                    if (indexOfTitle > 0)
                    {
                        if (!string.IsNullOrEmpty(MetaDescription))
                        {
                            sb.Insert(indexOfTitle + 8, string.Format("<meta name=\"description\" content=\"{0}\">", MetaDescription));
                        }

                        if (!string.IsNullOrEmpty(MetaKeywords))
                        {
                            sb.Insert(indexOfTitle + 8, string.Format("<meta name=\"keywords\" content=\"{0}\">", MetaKeywords));
                        }
                    }
                }

                str = sb.Replace("[THIS_IS_CONTENT_HOLDER_FOR_ROBO_FORM]", BodyContent).ToString();
                context.HttpContext.Response.Write(str);
            }
        }

        //TESTING
        public string RenderToString(ControllerContext context)
        {
            context.Controller.ViewBag.Title = Title;
            var fakeHttpContext = new FakeHttpContext(context.HttpContext);
            var fakeContext = new ControllerContext(fakeHttpContext, context.RouteData, context.Controller);
            ViewData = context.Controller.ViewData;
            base.ExecuteResult(fakeContext);

            using (var reader = new StreamReader(fakeHttpContext.Response.OutputStream))
            {
                var str = reader.ReadToEnd();
                var sb = new StringBuilder(str);

                // Add metas
                if (!string.IsNullOrEmpty(MetaKeywords) || !string.IsNullOrEmpty(MetaDescription))
                {
                    var indexOfTitle = str.IndexOf("</title>", StringComparison.InvariantCultureIgnoreCase);
                    if (indexOfTitle > 0)
                    {
                        if (!string.IsNullOrEmpty(MetaDescription))
                        {
                            sb.Insert(indexOfTitle + 8, string.Format("<meta name=\"description\" content=\"{0}\">", MetaDescription));
                        }

                        if (!string.IsNullOrEmpty(MetaKeywords))
                        {
                            sb.Insert(indexOfTitle + 8, string.Format("<meta name=\"keywords\" content=\"{0}\">", MetaKeywords));
                        }
                    }
                }

                str = sb.Replace("[THIS_IS_CONTENT_HOLDER_FOR_ROBO_FORM]", BodyContent).ToString();
                return str;
            }
        }
    }
}