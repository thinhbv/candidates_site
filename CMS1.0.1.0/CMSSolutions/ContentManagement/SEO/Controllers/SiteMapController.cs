using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.ContentManagement.SEO.SiteMaps;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.SEO.Controllers
{
    [Feature(Constants.Areas.SEO)]
    public class SiteMapController : Controller
    {
        private readonly IEnumerable<ISitemapProvider> providers;

        public SiteMapController(IEnumerable<ISitemapProvider> providers)
        {
            this.providers = providers;
        }

        [Url("sitemap.xml")]
        public ActionResult SitemapXml()
        {
            var urls = new List<SitemapUrl>();
            foreach (var provider in providers)
            {
                urls.AddRange(provider.GetSitemapUrls(Url).ToList());
            }

            // Home page
            urls.Insert(0, new SitemapUrl("Home", Url.AbsoluteContent("~/")) { Priority = 1 });

            return new XmlSitemapResult(urls);
        }

        [Themed, Url("sitemap.html")]
        public ActionResult SitemapHtml()
        {
            var urls = new List<SitemapUrl>();
            foreach (var provider in providers)
            {
                urls.AddRange(provider.GetSitemapUrls(Url).ToList());
            }

            // Home page
            urls.Insert(0, new SitemapUrl("Home", Url.AbsoluteContent("~/")) { Priority = 1 });

            var sb = new StringBuilder();
            using(var textWriter = new StringWriter(sb))
            {
                using (var writer = new HtmlTextWriter(textWriter))
                {
                    writer.AddAttribute(HtmlTextWriterAttribute.Class, "sitemap");
                    writer.RenderBeginTag(HtmlTextWriterTag.Div);

                    writer.AddAttribute(HtmlTextWriterAttribute.Id, "primaryNav");
                    writer.RenderBeginTag(HtmlTextWriterTag.Ul);

                    foreach (var url in urls)
                    {
                        RenderSitemapUrl(writer, url);
                    }

                    writer.RenderEndTag(); // ul
                    writer.RenderEndTag(); // div

                    writer.Flush();
                    textWriter.Flush();

                    ViewBag.Title = "Sitemap";
                    return new ControlContentResult(sb.ToString());
                }
            }
        }

        private static void RenderSitemapUrl(HtmlTextWriter writer, SitemapUrl sitemapUrl)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Li);

            writer.AddAttribute(HtmlTextWriterAttribute.Href, sitemapUrl.Url);
            writer.RenderBeginTag(HtmlTextWriterTag.A);
            writer.Write(sitemapUrl.Text);
            writer.RenderEndTag(); // a

            if (sitemapUrl.Children.Count > 0)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Ul);

                foreach (var url in sitemapUrl.Children)
                {
                    RenderSitemapUrl(writer, url);
                }

                writer.RenderEndTag(); // ul
            }

            writer.RenderEndTag(); // li
        }
    }
}
