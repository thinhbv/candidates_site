using System;
using System.Collections.Generic;

namespace CMSSolutions.ContentManagement.SEO.SiteMaps
{
    public class SitemapUrl
    {
        public SitemapUrl(string text, string url)
        {
            Text = text;
            Url = url;
            Children = new List<SitemapUrl>();
        }

        public string Text { get; set; }
        
        public string Url { get; set; }

        public DateTime? LastModified { get; set; }

        public float? Priority { get; set; }

        public ChangeFrequency? ChangeFrequency { get; set; }

        public IList<SitemapUrl> Children { get; set; }
    }

    public enum ChangeFrequency
    {
        Always,
        Hourly,
        Daily,
        Weekly,
        Monthly,
        Yearly,
        Never
    }
}