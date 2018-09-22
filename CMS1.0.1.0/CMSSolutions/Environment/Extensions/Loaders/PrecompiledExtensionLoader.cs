//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Reflection;
//using Castle.Core.Logging;
//using CMSSolutions.Caching;
//using CMSSolutions.Environment.Extensions.Models;
//using CMSSolutions.FileSystems.Dependencies;
//using CMSSolutions.FileSystems.VirtualPath;

//namespace CMSSolutions.Environment.Extensions.Loaders
//{
//    /// <summary>
//    /// Load an extension by looking into the "bin" subdirectory of an
//    /// extension directory.
//    /// </summary>
//    public class PrecompiledExtensionLoader : ExtensionLoaderBase
//    {
//        private readonly IHostEnvironment hostEnvironment;
//        private readonly IAssemblyProbingFolder assemblyProbingFolder;
//        private readonly IVirtualPathProvider virtualPathProvider;
//        private readonly IVirtualPathMonitor virtualPathMonitor;

//        public PrecompiledExtensionLoader(
//            IHostEnvironment hostEnvironment,
//            IDependenciesFolder dependenciesFolder,
//            IAssemblyProbingFolder assemblyProbingFolder,
//            IVirtualPathProvider virtualPathProvider,
//            IVirtualPathMonitor virtualPathMonitor)
//            : base(dependenciesFolder)
//        {
//            this.hostEnvironment = hostEnvironment;
//            this.assemblyProbingFolder = assemblyProbingFolder;
//            this.virtualPathProvider = virtualPathProvider;
//            this.virtualPathMonitor = virtualPathMonitor;

//            Logger = NullLogger.Instance;
//        }

//        public ILogger Logger { get; set; }

//        public bool Disabled { get; set; }

//        public override int Order { get { return 30; } }

//        public override IEnumerable<ExtensionCompilationReference> GetCompilationReferences(DependencyDescriptor dependency)
//        {
//            yield return new ExtensionCompilationReference { AssemblyName = dependency.Name };
//        }

//        public override IEnumerable<string> GetVirtualPathDependencies(DependencyDescriptor dependency)
//        {
//            yield return assemblyProbingFolder.GetAssemblyVirtualPath(dependency.Name);
//        }

//        public override void ExtensionRemoved(ExtensionLoadingContext ctx, DependencyDescriptor dependency)
//        {
//            if (assemblyProbingFolder.AssemblyExists(dependency.Name))
//            {
//                ctx.DeleteActions.Add(
//                    () =>
//                    {
//                        Logger.InfoFormat("ExtensionRemoved: Deleting assembly \"{0}\" from probing directory", dependency.Name);
//                        assemblyProbingFolder.DeleteAssembly(dependency.Name);
//                    });

//                // We need to restart the appDomain if the assembly is loaded
//                if (hostEnvironment.IsAssemblyLoaded(dependency.Name))
//                {
//                    Logger.InfoFormat("ExtensionRemoved: Module \"{0}\" is removed and its assembly is loaded, forcing AppDomain restart", dependency.Name);
//                    ctx.RestartAppDomain = true;
//                }
//            }
//        }

//        public override void ExtensionActivated(ExtensionLoadingContext ctx, ExtensionDescriptor extension)
//        {
//            string sourceFileName = virtualPathProvider.MapPath(GetAssemblyPath(extension));

//            // Copy the assembly if it doesn't exist or if it is older than the source file.
//            bool copyAssembly =
//                !assemblyProbingFolder.AssemblyExists(extension.Id) ||
//                File.GetLastWriteTimeUtc(sourceFileName) > assemblyProbingFolder.GetAssemblyDateTimeUtc(extension.Id);

//            if (copyAssembly)
//            {
//                ctx.CopyActions.Add(() => assemblyProbingFolder.StoreAssembly(extension.Id, sourceFileName));

//                // We need to restart the appDomain if the assembly is loaded
//                if (hostEnvironment.IsAssemblyLoaded(extension.Id))
//                {
//                    Logger.InfoFormat("ExtensionRemoved: Module \"{0}\" is activated with newer file and its assembly is loaded, forcing AppDomain restart", extension.Id);
//                    ctx.RestartAppDomain = true;
//                }
//            }
//        }

//        public override void ExtensionDeactivated(ExtensionLoadingContext ctx, ExtensionDescriptor extension)
//        {
//            if (assemblyProbingFolder.AssemblyExists(extension.Id))
//            {
//                ctx.DeleteActions.Add(
//                    () =>
//                    {
//                        Logger.InfoFormat("ExtensionDeactivated: Deleting assembly \"{0}\" from probing directory", extension.Id);
//                        assemblyProbingFolder.DeleteAssembly(extension.Id);
//                    });

//                // We need to restart the appDomain if the assembly is loaded
//                if (hostEnvironment.IsAssemblyLoaded(extension.Id))
//                {
//                    Logger.InfoFormat("ExtensionDeactivated: Module \"{0}\" is deactivated and its assembly is loaded, forcing AppDomain restart", extension.Id);
//                    ctx.RestartAppDomain = true;
//                }
//            }
//        }

//        public override void ReferenceActivated(ExtensionLoadingContext context, ExtensionReferenceProbeEntry referenceEntry)
//        {
//            if (string.IsNullOrEmpty(referenceEntry.VirtualPath))
//                return;

//            string sourceFileName = virtualPathProvider.MapPath(referenceEntry.VirtualPath);

//            // Copy the assembly if it doesn't exist or if it is older than the source file.
//            bool copyAssembly =
//                !assemblyProbingFolder.AssemblyExists(referenceEntry.Name) ||
//                File.GetLastWriteTimeUtc(sourceFileName) > assemblyProbingFolder.GetAssemblyDateTimeUtc(referenceEntry.Name);

//            if (copyAssembly)
//            {
//                context.CopyActions.Add(() => assemblyProbingFolder.StoreAssembly(referenceEntry.Name, sourceFileName));

//                // We need to restart the appDomain if the assembly is loaded
//                if (hostEnvironment.IsAssemblyLoaded(referenceEntry.Name))
//                {
//                    Logger.InfoFormat("ReferenceActivated: Reference \"{0}\" is activated with newer file and its assembly is loaded, forcing AppDomain restart", referenceEntry.Name);
//                    context.RestartAppDomain = true;
//                }
//            }
//        }

//        public override void Monitor(ExtensionDescriptor descriptor, Action<IVolatileToken> monitor)
//        {
//            if (Disabled)
//                return;

//            // If the assembly exists, monitor it
//            string assemblyPath = GetAssemblyPath(descriptor);
//            if (assemblyPath != null)
//            {
//                Logger.DebugFormat("Monitoring virtual path \"{0}\"", assemblyPath);
//                monitor(virtualPathMonitor.WhenPathChanges(assemblyPath));
//                return;
//            }

//            // If the assembly doesn't exist, we monitor the containing "bin" folder, as the assembly
//            // may exist later if it is recompiled in Visual Studio for example, and we need to
//            // detect that as a change of configuration.
//            var assemblyDirectory = virtualPathProvider.Combine(descriptor.Location, "bin");
//            if (virtualPathProvider.DirectoryExists(assemblyDirectory))
//            {
//                Logger.DebugFormat("Monitoring virtual path \"{0}\"", assemblyDirectory);
//                monitor(virtualPathMonitor.WhenPathChanges(assemblyDirectory));
//            }
//        }

//        public override IEnumerable<ExtensionReferenceProbeEntry> ProbeReferences(ExtensionDescriptor descriptor)
//        {
//            if (Disabled)
//                return Enumerable.Empty<ExtensionReferenceProbeEntry>();

//            Logger.InfoFormat("Probing references for module '{0}'", descriptor.Id);

//            var assemblyPath = GetAssemblyPath(descriptor);
//            if (assemblyPath == null)
//                return Enumerable.Empty<ExtensionReferenceProbeEntry>();

//            var result = virtualPathProvider
//                .ListFiles(virtualPathProvider.GetDirectoryName(assemblyPath))
//                .Where(s => StringComparer.OrdinalIgnoreCase.Equals(Path.GetExtension(s), ".dll"))
//                .Where(s => !StringComparer.OrdinalIgnoreCase.Equals(Path.GetFileNameWithoutExtension(s), descriptor.Id))
//                .Select(path => new ExtensionReferenceProbeEntry
//                {
//                    Descriptor = descriptor,
//                    Loader = this,
//                    Name = Path.GetFileNameWithoutExtension(path),
//                    VirtualPath = path
//                })
//                .ToList();

//            Logger.InfoFormat("Done probing references for module '{0}'", descriptor.Id);
//            return result;
//        }

//        public override ExtensionProbeEntry Probe(ExtensionDescriptor descriptor)
//        {
//            if (Disabled)
//                return null;

//            Logger.InfoFormat("Probing for module '{0}'", descriptor.Id);

//            var assemblyPath = GetAssemblyPath(descriptor);
//            if (assemblyPath == null)
//                return null;

//            var result = new ExtensionProbeEntry
//            {
//                Descriptor = descriptor,
//                Loader = this,
//                Priority = 80,
//                VirtualPath = assemblyPath,
//                VirtualPathDependencies = new[] { assemblyPath },
//            };

//            Logger.InfoFormat("Done probing for module '{0}'", descriptor.Id);
//            return result;
//        }

//        public override Assembly LoadReference(DependencyReferenceDescriptor reference)
//        {
//            if (Disabled)
//                return null;

//            Logger.InfoFormat("Loading reference '{0}'", reference.Name);

//            var result = assemblyProbingFolder.LoadAssembly(reference.Name);

//            Logger.InfoFormat("Done loading reference '{0}'", reference.Name);
//            return result;
//        }

//        protected override ExtensionEntry LoadWorker(ExtensionDescriptor descriptor)
//        {
//            if (Disabled)
//                return null;

//            Logger.InfoFormat("Start loading pre-compiled extension \"{0}\"", descriptor.Name);

//            var assembly = assemblyProbingFolder.LoadAssembly(descriptor.Id);
//            if (assembly == null)
//                return null;

//            Logger.InfoFormat("Done loading pre-compiled extension \"{0}\": assembly name=\"{1}\"", descriptor.Name, assembly.FullName);

//            return new ExtensionEntry
//            {
//                Descriptor = descriptor,
//                ExportedTypes = assembly.GetExportedTypes()
//            };
//        }

//        public string GetAssemblyPath(ExtensionDescriptor descriptor)
//        {
//            var assemblyPath = virtualPathProvider.Combine(descriptor.Location, "bin", descriptor.Id + ".dll");
//            if (!virtualPathProvider.FileExists(assemblyPath))
//                return null;

//            return assemblyPath;
//        }
//    }
//}