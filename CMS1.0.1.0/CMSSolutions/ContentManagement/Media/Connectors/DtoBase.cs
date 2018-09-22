using System;
using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors
{
    [DataContract]
    internal abstract class DtoBase
    {
        protected static DateTime UnixOrigin = new DateTime(1970, 1, 1, 0, 0, 0);

        /// <summary>
        ///  Name of file/dir. Required
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>
        ///  Hash of current file/dir path, first symbol must be letter, symbols before _underline_ - volume id, Required.
        /// </summary>
        [DataMember(Name = "hash")]
        public string Hash { get; set; }

        /// <summary>
        ///  mime type. Required.
        /// </summary>
        [DataMember(Name = "mime")]
        public string Mime { get; set; }

        /// <summary>
        /// file modification time in unix timestamp. Required.
        /// </summary>
        [DataMember(Name = "ts")]
        public long UnixTimeStamp { get; set; }

        /// <summary>
        ///  file size in bytes
        /// </summary>
        [DataMember(Name = "size")]
        public long Size { get; set; }

        /// <summary>
        ///  is readable
        /// </summary>
        [DataMember(Name = "read")]
        public byte Read { get; set; }

        /// <summary>
        /// is writable
        /// </summary>
        [DataMember(Name = "write")]
        public byte Write { get; set; }

        /// <summary>
        ///  is file locked. If locked that object cannot be deleted and renamed
        /// </summary>
        [DataMember(Name = "locked")]
        public byte Locked { get; set; }
    }
}