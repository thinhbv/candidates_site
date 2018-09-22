using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors.Responses
{
    [DataContract]
    internal class RemoveResponse
    {
        private readonly List<string> removed;

        [DataMember(Name = "removed")]
        public List<string> Removed { get { return removed; } }

        public RemoveResponse()
        {
            removed = new List<string>();
        }
    }
}