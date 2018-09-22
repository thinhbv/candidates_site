using System.Collections.Generic;
using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.Environment.Extensions.Loaders
{
    public class ExtensionProbeEntry
    {
        public ExtensionDescriptor Descriptor { get; set; }

        public IExtensionLoader Loader { get; set; }

        public int Priority { get; set; }

        public string VirtualPath { get; set; }

        public IEnumerable<string> VirtualPathDependencies { get; set; }
    }
}