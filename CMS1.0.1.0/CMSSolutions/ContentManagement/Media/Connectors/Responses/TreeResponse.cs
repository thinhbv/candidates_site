using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors.Responses
{
    [DataContract]
    internal class TreeResponse
    {
        [DataMember(Name = "tree")]
        public List<DtoBase> Tree { get; private set; }

        public TreeResponse()
        {
            Tree = new List<DtoBase>();
        }
    }
}