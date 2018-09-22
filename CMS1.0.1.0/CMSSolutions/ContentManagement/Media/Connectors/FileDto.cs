using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors
{
    [DataContract]
    internal class FileDto : DtoBase
    {
        /// <summary>
        ///  Hash of parent directory. Required except roots dirs.
        /// </summary>
        [DataMember(Name = "phash")]
        public string ParentHash { get; set; }

        /// <summary>
        /// Url of file
        /// </summary>
        [DataMember(Name = "url")]
        public string Url { get; set; }
    }
}