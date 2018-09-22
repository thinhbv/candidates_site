using System;
using System.Linq;
using Castle.Core.Logging;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.FileSystems.Dependencies;
using CMSSolutions.FileSystems.VirtualPath;

namespace CMSSolutions.Environment.Extensions.Loaders
{
    public class RawThemeExtensionLoader : ExtensionLoaderBase
    {
        private readonly IVirtualPathProvider virtualPathProvider;

        public RawThemeExtensionLoader(IDependenciesFolder dependenciesFolder, IVirtualPathProvider virtualPathProvider)
            : base(dependenciesFolder)
        {
            this.virtualPathProvider = virtualPathProvider;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool Disabled { get; set; }

        public override int Order { get { return 10; } }

        public override ExtensionProbeEntry Probe(ExtensionDescriptor descriptor)
        {
            if (Disabled)
                return null;

            if (descriptor.Location.StartsWith("~/Themes/"))
            {
                string projectPath = virtualPathProvider.Combine(descriptor.Location, descriptor.Id + ".csproj");

                // ignore themes including a .csproj in this loader
                if (virtualPathProvider.FileExists(projectPath))
                {
                    return null;
                }

                var assemblyPath = virtualPathProvider.Combine(descriptor.Location, "bin", descriptor.Id + ".dll");

                // ignore themes with /bin in this loader
                if (virtualPathProvider.FileExists(assemblyPath))
                    return null;

                return new ExtensionProbeEntry
                {
                    Descriptor = descriptor,
                    Loader = this,
                    VirtualPath = "~/Themes/" + descriptor.Id,
                    VirtualPathDependencies = Enumerable.Empty<string>(),
                };
            }
            return null;
        }

        protected override ExtensionEntry LoadWorker(ExtensionDescriptor descriptor)
        {
            if (Disabled)
                return null;

            Logger.InfoFormat("Loaded no-code theme \"{0}\"", descriptor.Name);

            return new ExtensionEntry
            {
                Descriptor = descriptor,
                ExportedTypes = new Type[0]
            };
        }
    }
}