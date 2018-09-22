using System.Collections.Generic;
using System.Runtime.Serialization;

namespace CMSSolutions.ContentManagement.Media.Connectors.Responses
{
    [DataContract]
    internal class OpenResponseBase
    {
        private static readonly DebugOptions debug = new DebugOptions();
        private readonly List<DtoBase> files;
        private readonly DtoBase currentWorkingDirectory;

        [DataMember(Name = "files")]
        public IEnumerable<DtoBase> Files { get { return files; } }

        [DataMember(Name = "cwd")]
        public DtoBase CurrentWorkingDirectory { get { return currentWorkingDirectory; } }

        [DataMember(Name = "options")]
        public Options Options { get; protected set; }

        [DataMember(Name = "debug")]
        public DebugOptions Debug { get { return debug; } }

        public OpenResponseBase(DtoBase currentWorkingDirectory)
        {
            files = new List<DtoBase>();
            this.currentWorkingDirectory = currentWorkingDirectory;
        }

        public void AddResponse(DtoBase item)
        {
            files.Add(item);
        }

        [DataContract]
        internal class DebugOptions
        {
            private static readonly string[] empty = new string[0];

            [DataMember(Name = "connector")]
            public string Connector { get { return ".net"; } }

            [DataMember(Name = "mountErrors")]
            public string[] MountErrors { get { return empty; } }
        }
    }
}