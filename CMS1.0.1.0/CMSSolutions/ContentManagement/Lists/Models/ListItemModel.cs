using System;

namespace CMSSolutions.ContentManagement.Lists.Models
{
    public class ListItemModel
    {
        public int Id { get; set; }

        public int ListId { get; set; }

        public int[] Categories { get; set; }

        public string Title { get; set; }

        public string Slug { get; set; }

        public string PictureUrl { get; set; }

        public string MetaKeywords { get; set; }

        public string MetaDescription { get; set; }

        public int Position { get; set; }

        public bool Enabled { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime ModifiedDate { get; set; }
    }
}