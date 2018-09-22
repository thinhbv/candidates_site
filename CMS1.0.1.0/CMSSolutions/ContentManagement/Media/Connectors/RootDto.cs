using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors
{
    [DataContract]
    internal class RootDto : DtoBase
    {
        [DataMember(Name = "volumeId")]
        public string VolumeId { get; set; }

        [DataMember(Name = "dirs")]
        public byte Dirs { get; set; }
    }
}