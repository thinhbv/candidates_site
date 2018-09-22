using System.Collections.Generic;
using System.Web.Mvc;

namespace CMSSolutions.ContentManagement.SEO.SiteMaps
{
    public interface ISitemapProvider : IDependency
    {
        IEnumerable<SitemapUrl> GetSitemapUrls(UrlHelper urlHelper);
    }
}