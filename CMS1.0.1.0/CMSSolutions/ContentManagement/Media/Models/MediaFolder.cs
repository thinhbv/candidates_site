using System;

namespace CMSSolutions.ContentManagement.Media.Models
{
    public class MediaFolder
    {
        public string Name { get; set; }

        public string Type { get; set; }

        public long Size { get; set; }

        public DateTime LastUpdated { get; set; }

        public string MediaPath { get; set; }
    }
}