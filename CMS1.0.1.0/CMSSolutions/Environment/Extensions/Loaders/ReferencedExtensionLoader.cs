using System.IO;
using Castle.Core.Logging;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.FileSystems.Dependencies;
using CMSSolutions.FileSystems.VirtualPath;

namespace CMSSolutions.Environment.Extensions.Loaders
{
    /// <summary>
    /// Load an extension by looking through the BuildManager referenced assemblies
    /// </summary>
    public class ReferencedExtensionLoader : ExtensionLoaderBase
    {
        private readonly IVirtualPathProvider virtualPathProvider;
        private readonly IBuildManager buildManager;

        public ReferencedExtensionLoader(IDependenciesFolder dependenciesFolder, IVirtualPathProvider virtualPathProvider, IBuildManager buildManager)
            : base(dependenciesFolder)
        {
            this.virtualPathProvider = virtualPathProvider;
            this.buildManager = buildManager;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public bool Disabled { get; set; }

        public override int Order { get { return 20; } }

        public override void ExtensionDeactivated(ExtensionLoadingContext ctx, ExtensionDescriptor extension)
        {
            DeleteAssembly(ctx, extension.Id);
        }

        public override void ExtensionRemoved(ExtensionLoadingContext ctx, DependencyDescriptor dependency)
        {
            DeleteAssembly(ctx, dependency.Name);
        }

        private void DeleteAssembly(ExtensionLoadingContext ctx, string moduleName)
        {
            var assemblyPath = virtualPathProvider.Combine("~/bin", moduleName + ".dll");
            if (virtualPathProvider.FileExists(assemblyPath))
            {
                ctx.DeleteActions.Add(
                    () =>
                    {
                        Logger.InfoFormat("ExtensionRemoved: Deleting assembly \"{0}\" from bin directory (AppDomain will restart)", moduleName);
                        File.Delete(virtualPathProvider.MapPath(assemblyPath));
                    });
                ctx.RestartAppDomain = true;
            }
        }

        public override ExtensionProbeEntry Probe(ExtensionDescriptor descriptor)
        {
            if (Disabled)
                return null;

            var assembly = buildManager.GetReferencedAssembly(descriptor.Id);
            if (assembly == null)
                return null;

            var assemblyPath = virtualPathProvider.Combine("~/bin", descriptor.Id + ".dll");

            return new ExtensionProbeEntry
            {
                Descriptor = descriptor,
                Loader = this,
                Priority = 100, // Higher priority because assemblies in ~/bin always take precedence
                VirtualPath = assemblyPath,
                VirtualPathDependencies = new[] { assemblyPath },
            };
        }

        protected override ExtensionEntry LoadWorker(ExtensionDescriptor descriptor)
        {
            if (Disabled)
                return null;

            var assembly = buildManager.GetReferencedAssembly(descriptor.Id);
            if (assembly == null)
                return null;

            Logger.InfoFormat("Loaded referenced extension \"{0}\": assembly name=\"{1}\"", descriptor.Name, assembly.FullName);

            return new ExtensionEntry
            {
                Descriptor = descriptor,
                ExportedTypes = assembly.GetExportedTypes()
            };
        }
    }
}