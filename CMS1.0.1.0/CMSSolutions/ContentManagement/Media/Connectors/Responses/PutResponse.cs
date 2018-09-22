using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors.Responses
{
    [DataContract]
    internal class PutResponse
    {
        [DataMember(Name = "changed")]
        public List<FileDto> Changed { get; private set; }

        public PutResponse()
        {
            Changed = new List<FileDto>();
        }
    }
}