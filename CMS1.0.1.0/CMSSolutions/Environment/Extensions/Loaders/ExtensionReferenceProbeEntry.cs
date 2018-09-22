using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.Environment.Extensions.Loaders
{
    public class ExtensionReferenceProbeEntry
    {
        public ExtensionDescriptor Descriptor { get; set; }

        public IExtensionLoader Loader { get; set; }

        public string Name { get; set; }

        public string VirtualPath { get; set; }
    }
}