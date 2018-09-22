using System.Reflection;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.FileSystems.Dependencies;

namespace CMSSolutions.Environment.Extensions.Loaders
{
    public class FrameworkExtensionLoader : ExtensionLoaderBase
    {
        public FrameworkExtensionLoader(IDependenciesFolder dependenciesFolder)
            : base(dependenciesFolder)
        {
        }

        public override int Order
        {
            get { return 5; }
        }

        public override ExtensionProbeEntry Probe(ExtensionDescriptor descriptor)
        {
            return null;
        }

        protected override ExtensionEntry LoadWorker(ExtensionDescriptor descriptor)
        {
            return null;
        }

        public override ExtensionEntry Load(ExtensionDescriptor descriptor)
        {
            if (descriptor.Id == "CMSSolutions")
            {
                return new ExtensionEntry
                           {
                               Descriptor = descriptor,
                               ExportedTypes = Assembly.GetCallingAssembly().GetExportedTypes()
                           };
            }
            return null;
        }
    }
}