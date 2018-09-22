using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors.Responses
{
    [DataContract]
    internal class ReplaceResponse
    {
        [DataMember(Name = "added")]
        public List<DtoBase> Added { get; private set; }

        [DataMember(Name = "removed")]
        public List<string> Removed { get; private set; }

        public ReplaceResponse()
        {
            Added = new List<DtoBase>();
            Removed = new List<string>();
        }
    }
}