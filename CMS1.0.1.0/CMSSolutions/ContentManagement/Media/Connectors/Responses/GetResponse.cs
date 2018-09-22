using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors.Responses
{
    [DataContract]
    internal class GetResponse
    {
        [DataMember(Name = "content")]
        public string Content { get; set; }
    }
}