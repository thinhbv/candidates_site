using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors.Responses
{
    [DataContract]
    internal class Archive
    {
        private static readonly string[] empty = new string[0];

        [DataMember(Name = "create")]
        public IEnumerable<string> Create { get { return empty; } }

        [DataMember(Name = "extract")]
        public IEnumerable<string> Extract { get { return empty; } }
    }

    [DataContract]
    internal class Options
    {
        private static readonly string[] disabled = new[] { "extract", "create" };
        private static readonly Archive emptyArchives = new Archive();

        [DataMember(Name = "copyOverwrite")]
        public byte IsCopyOverwrite { get { return 1; } }

        [DataMember(Name = "separator")]
        public string Separator { get { return "/"; } }

        [DataMember(Name = "path")]
        public string Path { get; set; }

        [DataMember(Name = "tmbUrl")]
        public string ThumbnailsUrl { get; set; }

        [DataMember(Name = "url")]
        public string Url { get; set; }

        [DataMember(Name = "archivers")]
        public Archive Archivers { get { return emptyArchives; } }

        [DataMember(Name = "disabled")]
        public IEnumerable<string> Disabled { get { return disabled; } }
    }
}