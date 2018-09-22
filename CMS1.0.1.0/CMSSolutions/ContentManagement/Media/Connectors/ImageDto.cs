using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors
{
    [DataContract]
    internal class ImageDto : FileDto
    {
        [DataMember(Name = "tmb")]
        public string Thumbnail { get; set; }
    }
}