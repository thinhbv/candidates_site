using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Castle.Core.Logging;
using CMSSolutions.Caching;
using CMSSolutions.Collections;
using CMSSolutions.Environment.Extensions.Loaders;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.FileSystems.Dependencies;
using CMSSolutions.FileSystems.VirtualPath;
using CMSSolutions.Localization;
using CMSSolutions.Utility;

namespace CMSSolutions.Environment.Extensions
{
    public class ExtensionLoaderCoordinator : IExtensionLoaderCoordinator
    {
        private readonly IDependenciesFolder dependenciesFolder;
        private readonly IExtensionDependenciesManager extensionDependenciesManager;
        private readonly IExtensionManager extensionManager;
        private readonly IVirtualPathProvider virtualPathProvider;
        private readonly IEnumerable<IExtensionLoader> loaders;
        private readonly IHostEnvironment hostEnvironment;
        private readonly IParallelCacheContext parallelCacheContext;
        private readonly IBuildManager buildManager;

        public ExtensionLoaderCoordinator(
            IDependenciesFolder dependenciesFolder,
            IExtensionDependenciesManager extensionDependenciesManager,
            IExtensionManager extensionManager,
            IVirtualPathProvider virtualPathProvider,
            IEnumerable<IExtensionLoader> loaders,
            IHostEnvironment hostEnvironment,
            IParallelCacheContext parallelCacheContext,
            IBuildManager buildManager)
        {
            this.dependenciesFolder = dependenciesFolder;
            this.extensionDependenciesManager = extensionDependenciesManager;
            this.extensionManager = extensionManager;
            this.virtualPathProvider = virtualPathProvider;
            this.loaders = loaders.OrderBy(l => l.Order);
            this.hostEnvironment = hostEnvironment;
            this.parallelCacheContext = parallelCacheContext;
            this.buildManager = buildManager;

            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        public void SetupExtensions()
        {
            Logger.Info("Start loading extensions...");

            var context = CreateLoadingContext();

            // Notify all loaders about extensions removed from the web site
            foreach (var dependency in context.DeletedDependencies)
            {
                Logger.InfoFormat("Extension {0} has been removed from site", dependency.Name);
                foreach (var loader in loaders)
                {
                    if (dependency.LoaderName == loader.Name)
                    {
                        loader.ExtensionRemoved(context, dependency);
                    }
                }
            }

            // For all existing extensions in the site, ask each loader if they can
            // load that extension.
            foreach (var extension in context.AvailableExtensions)
            {
                ProcessExtension(context, extension);
            }

            // Execute all the work need by "ctx"
            ProcessContextCommands(context);

            // And finally save the new entries in the dependencies folder
            dependenciesFolder.StoreDescriptors(context.NewDependencies);
            extensionDependenciesManager.StoreDependencies(context.NewDependencies, desc => GetExtensionHash(context, desc));

            Logger.Info("Done loading extensions...");

            // Very last step: Notify the host environment to restart the AppDomain if needed
            if (context.RestartAppDomain)
            {
                Logger.Info("AppDomain restart required.");
                hostEnvironment.RestartAppDomain();
            }
        }

        private string GetExtensionHash(ExtensionLoadingContext context, DependencyDescriptor dependencyDescriptor)
        {
            var hash = new Hash();
            hash.AddStringInvariant(dependencyDescriptor.Name);

            foreach (var virtualpathDependency in context.ProcessedExtensions[dependencyDescriptor.Name].VirtualPathDependencies)
            {
                hash.AddDateTime(GetVirtualPathModificationTimeUtc(context.VirtualPathModficationDates, virtualpathDependency));
            }

            foreach (var reference in dependencyDescriptor.References)
            {
                hash.AddStringInvariant(reference.Name);
                hash.AddString(reference.LoaderName);
                hash.AddDateTime(GetVirtualPathModificationTimeUtc(context.VirtualPathModficationDates, reference.VirtualPath));
            }

            return hash.Value;
        }

        private void ProcessExtension(ExtensionLoadingContext context, ExtensionDescriptor extension)
        {
            var extensionProbes = context.AvailableExtensionsProbes.ContainsKey(extension.Id) ?
                context.AvailableExtensionsProbes[extension.Id] :
                Enumerable.Empty<ExtensionProbeEntry>();

            // materializes the list
            extensionProbes = extensionProbes.ToArray();

            if (Logger.IsDebugEnabled)
            {
                Logger.DebugFormat("Loaders for extension \"{0}\": ", extension.Id);
                foreach (var probe in extensionProbes)
                {
                    Logger.DebugFormat("  Loader: {0}", probe.Loader.Name);
                    Logger.DebugFormat("    VirtualPath: {0}", probe.VirtualPath);
                    Logger.DebugFormat("    VirtualPathDependencies: {0}", string.Join(", ", probe.VirtualPathDependencies));
                }
            }

            var moduleReferences =
                context.AvailableExtensions
                    .Where(e =>
                           context.ReferencesByModule.ContainsKey(extension.Id) &&
                           context.ReferencesByModule[extension.Id].Any(r => StringComparer.OrdinalIgnoreCase.Equals(e.Id, r.Name)))
                    .ToList();

            var processedModuleReferences =
                moduleReferences
                .Where(e => context.ProcessedExtensions.ContainsKey(e.Id))
                .Select(e => context.ProcessedExtensions[e.Id])
                .ToList();

            var activatedExtension = extensionProbes.FirstOrDefault(
                e => e.Loader.IsCompatibleWithModuleReferences(extension, processedModuleReferences)
                );

            var previousDependency = context.PreviousDependencies.FirstOrDefault(
                d => StringComparer.OrdinalIgnoreCase.Equals(d.Name, extension.Id)
                );

            if (activatedExtension == null)
            {
                Logger.WarnFormat("No loader found for extension \"{0}\"!", extension.Id);
            }

            var references = ProcessExtensionReferences(context, activatedExtension);

            foreach (var loader in loaders)
            {
                if (activatedExtension != null && activatedExtension.Loader.Name == loader.Name)
                {
                    Logger.InfoFormat("Activating extension \"{0}\" with loader \"{1}\"", activatedExtension.Descriptor.Id, loader.Name);
                    loader.ExtensionActivated(context, extension);
                }
                else if (previousDependency != null && previousDependency.LoaderName == loader.Name)
                {
                    Logger.InfoFormat("Deactivating extension \"{0}\" from loader \"{1}\"", previousDependency.Name, loader.Name);
                    loader.ExtensionDeactivated(context, extension);
                }
            }

            if (activatedExtension != null)
            {
                context.NewDependencies.Add(new DependencyDescriptor
                {
                    Name = extension.Id,
                    LoaderName = activatedExtension.Loader.Name,
                    VirtualPath = activatedExtension.VirtualPath,
                    References = references
                });
            }

            // Keep track of which loader we use for every extension
            // This will be needed for processing references from other dependent extensions
            context.ProcessedExtensions.Add(extension.Id, activatedExtension);
        }

        private ExtensionLoadingContext CreateLoadingContext()
        {
            var availableExtensions = extensionManager
                .AvailableExtensions()
                .Where(d => DefaultExtensionTypes.IsModule(d.ExtensionType) || DefaultExtensionTypes.IsTheme(d.ExtensionType))
                .OrderBy(d => d.Id)
                .ToList();

            // Check there are no duplicates
            var duplicates = availableExtensions.GroupBy(ed => ed.Id).Where(g => g.Count() >= 2).ToList();
            if (duplicates.Any())
            {
                var sb = new StringBuilder();
                sb.Append(T("There are multiple extensions with the same name installed in this instance.\r\n"));
                foreach (var dup in duplicates)
                {
                    sb.Append(T("Extension '{0}' has been found from the following locations: {1}.\r\n", dup.Key, string.Join(", ", dup.Select(e => e.Location))));
                }
                sb.Append(T("This issue can be usually solved by removing or renaming the conflicting extension."));
                Logger.Error(sb.ToString());
                throw new CMSException(new LocalizedString(sb.ToString()));
            }

            var previousDependencies = dependenciesFolder.LoadDescriptors().ToList();

            var virtualPathModficationDates = new ConcurrentDictionary<string, DateTime>(StringComparer.OrdinalIgnoreCase);

            Logger.Info("Probing extensions");
            var availableExtensionsProbes1 = parallelCacheContext
                .RunInParallel(availableExtensions, extension =>
                    loaders.Select(loader => loader.Probe(extension)).Where(entry => entry != null).ToArray())
                .SelectMany(entries => entries)
                .GroupBy(entry => entry.Descriptor.Id);

            var availableExtensionsProbes = parallelCacheContext
                .RunInParallel(availableExtensionsProbes1, g =>
                    new { Id = g.Key, Entries = SortExtensionProbeEntries(g, virtualPathModficationDates) })
                .ToDictionary(g => g.Id, g => g.Entries, StringComparer.OrdinalIgnoreCase);
            Logger.Info("Done probing extensions");

            var deletedDependencies = previousDependencies
                .Where(e => !availableExtensions.Any(e2 => StringComparer.OrdinalIgnoreCase.Equals(e2.Id, e.Name)))
                .ToList();

            // Collect references for all modules
            Logger.Info("Probing extension references");
            var references = parallelCacheContext
                .RunInParallel(availableExtensions, extension => loaders.SelectMany(loader => loader.ProbeReferences(extension)).ToList())
                .SelectMany(entries => entries)
                .ToList();
            Logger.Info("Done probing extension references");

            var referencesByModule = references
                .GroupBy(entry => entry.Descriptor.Id, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.AsEnumerable(), StringComparer.OrdinalIgnoreCase);

            var referencesByName = references
                .GroupBy(reference => reference.Name, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(g => g.Key, g => g.AsEnumerable(), StringComparer.OrdinalIgnoreCase);

            var sortedAvailableExtensions =
                availableExtensions.OrderByDependenciesAndPriorities(
                    (item, dep) => referencesByModule.ContainsKey(item.Id) &&
                                   referencesByModule[item.Id].Any(r => StringComparer.OrdinalIgnoreCase.Equals(dep.Id, r.Name)),
                    item => 0)
                    .ToList();

            return new ExtensionLoadingContext
            {
                AvailableExtensions = sortedAvailableExtensions,
                PreviousDependencies = previousDependencies,
                DeletedDependencies = deletedDependencies,
                AvailableExtensionsProbes = availableExtensionsProbes,
                ReferencesByName = referencesByName,
                ReferencesByModule = referencesByModule,
                VirtualPathModficationDates = virtualPathModficationDates,
            };
        }

        private IEnumerable<ExtensionProbeEntry> SortExtensionProbeEntries(IEnumerable<ExtensionProbeEntry> entries, ConcurrentDictionary<string, DateTime> virtualPathModficationDates)
        {
            // All "entries" are for the same extension ID, so we just need to filter/sort them by priority+ modification dates.
            var groupByPriority = entries
                .GroupBy(entry => entry.Priority)
                .OrderByDescending(g => g.Key);

            // Select highest priority group with at least one item
            var firstNonEmptyGroup = groupByPriority.FirstOrDefault(g => g.Any()) ?? Enumerable.Empty<ExtensionProbeEntry>();

            // No need for further sorting if only 1 item found
            if (firstNonEmptyGroup.Count() <= 1)
                return firstNonEmptyGroup;

            // Sort by last modification date/loader order
            return firstNonEmptyGroup
                .OrderByDescending(probe => GetVirtualPathDepedenciesModificationTimeUtc(virtualPathModficationDates, probe))
                .ThenBy(probe => probe.Loader.Order)
                .ToList();
        }

        private DateTime GetVirtualPathDepedenciesModificationTimeUtc(ConcurrentDictionary<string, DateTime> virtualPathDependencies, ExtensionProbeEntry probe)
        {
            if (!probe.VirtualPathDependencies.Any())
                return DateTime.MinValue;

            Logger.InfoFormat("Retrieving modification dates of dependencies of extension '{0}'", probe.Descriptor.Id);

            var result = probe.VirtualPathDependencies.Max(path => GetVirtualPathModificationTimeUtc(virtualPathDependencies, path));

            Logger.InfoFormat("Done retrieving modification dates of dependencies of extension '{0}'", probe.Descriptor.Id);
            return result;
        }

        private DateTime GetVirtualPathModificationTimeUtc(ConcurrentDictionary<string, DateTime> virtualPathDependencies, string path)
        {
            return virtualPathDependencies.GetOrAdd(path, p => virtualPathProvider.GetFileLastWriteTimeUtc(p));
        }

        private IEnumerable<DependencyReferenceDescriptor> ProcessExtensionReferences(ExtensionLoadingContext context, ExtensionProbeEntry activatedExtension)
        {
            if (activatedExtension == null)
                return Enumerable.Empty<DependencyReferenceDescriptor>();

            var referenceNames = (context.ReferencesByModule.ContainsKey(activatedExtension.Descriptor.Id) ?
                context.ReferencesByModule[activatedExtension.Descriptor.Id] :
                Enumerable.Empty<ExtensionReferenceProbeEntry>())
                .Select(r => r.Name)
                .Distinct(StringComparer.OrdinalIgnoreCase);

            var referencesDecriptors = new List<DependencyReferenceDescriptor>();
            foreach (var referenceName in referenceNames)
            {
                ProcessExtensionReference(context, referenceName, referencesDecriptors);
            }

            return referencesDecriptors;
        }

        private void ProcessExtensionReference(ExtensionLoadingContext context,
            string referenceName,
            IList<DependencyReferenceDescriptor> activatedReferences)
        {
            // If the reference is an extension has been processed already, use the same loader as
            // that extension, since a given extension should be loaded with a unique loader for the
            // whole application
            var bestExtensionReference = context.ProcessedExtensions.ContainsKey(referenceName) ?
                context.ProcessedExtensions[referenceName] :
                null;

            // Activated the extension reference
            if (bestExtensionReference != null)
            {
                activatedReferences.Add(new DependencyReferenceDescriptor
                {
                    LoaderName = bestExtensionReference.Loader.Name,
                    Name = referenceName,
                    VirtualPath = bestExtensionReference.VirtualPath
                });

                return;
            }

            // Skip references from "~/bin"
            if (buildManager.HasReferencedAssembly(referenceName))
                return;

            // Binary references
            var references = context.ReferencesByName.ContainsKey(referenceName) ?
                context.ReferencesByName[referenceName] :
                Enumerable.Empty<ExtensionReferenceProbeEntry>();

            var bestBinaryReference = references
                .Where(entry => !string.IsNullOrEmpty(entry.VirtualPath))
                .Select(entry => new { Entry = entry, LastWriteTimeUtc = virtualPathProvider.GetFileLastWriteTimeUtc(entry.VirtualPath) })
                .OrderBy(e => e.LastWriteTimeUtc)
                .ThenBy(e => e.Entry.Name)
                .FirstOrDefault();

            // Activate the binary ref
            if (bestBinaryReference != null)
            {
                if (!context.ProcessedReferences.ContainsKey(bestBinaryReference.Entry.Name))
                {
                    context.ProcessedReferences.Add(bestBinaryReference.Entry.Name, bestBinaryReference.Entry);
                    bestBinaryReference.Entry.Loader.ReferenceActivated(context, bestBinaryReference.Entry);
                }
                activatedReferences.Add(new DependencyReferenceDescriptor
                {
                    LoaderName = bestBinaryReference.Entry.Loader.Name,
                    Name = bestBinaryReference.Entry.Name,
                    VirtualPath = bestBinaryReference.Entry.VirtualPath
                });
            }
        }

        private void ProcessContextCommands(ExtensionLoadingContext ctx)
        {
            Logger.Info("Executing list of operations needed for loading extensions...");
            foreach (var action in ctx.DeleteActions)
            {
                action();
            }

            foreach (var action in ctx.CopyActions)
            {
                action();
            }
        }
    }
}