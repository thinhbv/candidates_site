using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors.Responses
{
    [DataContract]
    internal class InitResponse : OpenResponseBase
    {
        private static readonly string[] empty = new string[0];

        [DataMember(Name = "api")]
        public string Api { get { return "2.0"; } }

        [DataMember(Name = "uplMaxSize")]
        public string UploadMaxSize { get { return string.Empty; } }

        [DataMember(Name = "netDrivers")]
        public IEnumerable<string> NetDrivers { get { return empty; } }

        public InitResponse(DtoBase currentWorkingDirectory)
            : base(currentWorkingDirectory)
        {
            Options = new Options();
        }
    }
}