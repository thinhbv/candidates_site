using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors.Responses
{
    [DataContract]
    internal class AddResponse
    {
        private readonly List<DtoBase> added;

        [DataMember(Name = "added")]
        public List<DtoBase> Added { get { return added; } }

        public AddResponse(DtoBase dtoBase)
        {
            added = new List<DtoBase> { dtoBase };
        }

        public AddResponse()
        {
            added = new List<DtoBase>();
        }
    }
}