using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors
{
    [DataContract]
    internal class DirectoryDto : DtoBase
    {
        /// <summary>
        ///  Hash of parent directory. Required except roots dirs.
        /// </summary>
        [DataMember(Name = "phash")]
        public string ParentHash { get; set; }

        /// <summary>
        /// Is directory contains subfolders
        /// </summary>
        [DataMember(Name = "dirs")]
        public byte ContainsChildDirs { get; set; }

        public DirectoryDto()
        {
            Mime = "directory";
        }
    }
}