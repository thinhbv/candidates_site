using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;

namespace CMSSolutions.ContentManagement.SEO.SiteMaps
{
    public class XmlSitemapResult : ActionResult
    {
        private readonly IEnumerable<SitemapUrl> items;

        public XmlSitemapResult(IEnumerable<SitemapUrl> items)
        {
            this.items = items;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.ContentType = "application/xml";
            var encoding = context.HttpContext.Response.ContentEncoding.WebName;
            var xmlns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");

            var sitemap = new XDocument(new XDeclaration("1.0", encoding, null),
                 new XElement(xmlns + "urlset",
                      from item in items
                      select CreateItemElement(item, xmlns)
                      )
                 );

            context.HttpContext.Response.Write(sitemap.Declaration + sitemap.ToString());
            context.HttpContext.Response.Flush();
            context.HttpContext.Response.Close();
        }

        private static XElement CreateItemElement(SitemapUrl item, XNamespace xmlns)
        {
            var itemElement = new XElement(xmlns + "url", new XElement(xmlns + "loc", item.Url.ToLower()));

            if (item.LastModified.HasValue)
                itemElement.Add(new XElement(xmlns + "lastmod", item.LastModified.Value.ToString("yyyy-MM-dd")));

            if (item.ChangeFrequency.HasValue)
                itemElement.Add(new XElement(xmlns + "changefreq", item.ChangeFrequency.Value.ToString().ToLower()));

            if (item.Priority.HasValue)
                itemElement.Add(new XElement(xmlns + "priority", item.Priority.Value.ToString(CultureInfo.InvariantCulture)));

            return itemElement;
        }
    }
}
