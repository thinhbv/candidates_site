using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using CMSSolutions.Caching;
using CMSSolutions.Collections;
using CMSSolutions.Environment.Extensions.Folders;
using CMSSolutions.Environment.Extensions.Loaders;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Localization;

namespace CMSSolutions.Environment.Extensions
{
    public class ExtensionManager : IExtensionManager
    {
        private readonly IEnumerable<IExtensionFolders> folders;
        private readonly IAsyncTokenProvider asyncTokenProvider;
        private readonly ICacheManager cacheManager;
        private readonly IParallelCacheContext parallelCacheContext;
        private readonly IEnumerable<IExtensionLoader> loaders;

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        public ExtensionManager(
            IEnumerable<IExtensionFolders> folders,
            IEnumerable<IExtensionLoader> loaders,
            ICacheManager cacheManager,
            IParallelCacheContext parallelCacheContext,
            IAsyncTokenProvider asyncTokenProvider)
        {
            this.folders = folders;
            this.asyncTokenProvider = asyncTokenProvider;
            this.cacheManager = cacheManager;
            this.parallelCacheContext = parallelCacheContext;
            this.loaders = loaders.OrderBy(x => x.Order).ToArray();
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        // This method does not load extension types, simply parses extension manifests from
        // the filesystem.
        public ExtensionDescriptor GetExtension(string id)
        {
            return AvailableExtensions().FirstOrDefault(x => x.Id == id);
        }

        public IEnumerable<ExtensionDescriptor> AvailableExtensions()
        {
            return cacheManager.Get("AvailableExtensions", ctx =>
                parallelCacheContext
                    .RunInParallel(folders, folder => folder.AvailableExtensions().ToList())
                    .SelectMany(descriptors => descriptors)
                    .ToReadOnlyCollection());
        }

        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            return cacheManager.Get("AvailableFeatures", ctx =>
                AvailableExtensions()
                    .SelectMany(ext => ext.Features)
                    .OrderByDependenciesAndPriorities(HasDependency, GetPriority)
                    .ToReadOnlyCollection());
        }

        internal static int GetPriority(FeatureDescriptor featureDescriptor)
        {
            return featureDescriptor.Priority;
        }

        /// <summary>
        /// Returns true if the item has an explicit or implicit dependency on the subject
        /// </summary>
        /// <param name="item"></param>
        /// <param name="subject"></param>
        /// <returns></returns>
        internal static bool HasDependency(FeatureDescriptor item, FeatureDescriptor subject)
        {
            if (DefaultExtensionTypes.IsTheme(item.Extension.ExtensionType))
            {
                if (DefaultExtensionTypes.IsModule(subject.Extension.ExtensionType))
                {
                    // Themes implicitly depend on modules to ensure build and override ordering
                    return true;
                }

                if (DefaultExtensionTypes.IsTheme(subject.Extension.ExtensionType))
                {
                    // Theme depends on another if it is its base theme
                    return item.Extension.BaseTheme == subject.Id;
                }
            }

            // Return based on explicit dependencies
            return item.Dependencies != null &&
                   item.Dependencies.Any(x => StringComparer.OrdinalIgnoreCase.Equals(x, subject.Id));
        }

        public IEnumerable<Feature> LoadFeatures(IEnumerable<FeatureDescriptor> featureDescriptors)
        {
            Logger.Info("Loading features");

            var result =
                parallelCacheContext
                .RunInParallel(featureDescriptors, descriptor => cacheManager.Get(descriptor.Id, ctx => LoadFeature(descriptor)))
                .ToArray();

            Logger.Info("Done loading features");
            return result;
        }

        private Feature LoadFeature(FeatureDescriptor featureDescriptor)
        {
            var extensionDescriptor = featureDescriptor.Extension;
            var featureId = featureDescriptor.Id;
            var extensionId = extensionDescriptor.Id;

            ExtensionEntry extensionEntry;
            try
            {
                extensionEntry = cacheManager.Get(extensionId, ctx =>
                {
                    var entry = BuildEntry(extensionDescriptor);
                    if (entry != null)
                    {
                        ctx.Monitor(asyncTokenProvider.GetToken(monitor =>
                        {
                            foreach (var loader in loaders)
                            {
                                loader.Monitor(entry.Descriptor, monitor);
                            }
                        }));
                    }
                    return entry;
                });
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat(ex, "Error loading extension '{0}'", extensionId);
                throw new CMSException(T("Error while loading extension '{0}'.", extensionId), ex);
            }

            if (extensionEntry == null)
            {
                // If the feature could not be compiled for some reason,
                // return a "null" feature, i.e. a feature with no exported types.
                return new Feature
                {
                    Descriptor = featureDescriptor,
                    ExportedTypes = Enumerable.Empty<Type>()
                };
            }

            var extensionTypes = extensionEntry.ExportedTypes.Where(t => t.IsClass && !t.IsAbstract);
            var featureTypes = new List<Type>();

            foreach (var type in extensionTypes)
            {
                string sourceFeature = GetSourceFeatureNameForType(type, extensionId);
                if (string.Equals(sourceFeature, featureId, StringComparison.OrdinalIgnoreCase))
                {
                    featureTypes.Add(type);
                }
            }

            return new Feature
            {
                Descriptor = featureDescriptor,
                ExportedTypes = featureTypes
            };
        }

        private static string GetSourceFeatureNameForType(Type type, string extensionId)
        {
            foreach (FeatureAttribute featureAttribute in type.GetCustomAttributes(typeof(FeatureAttribute), false))
            {
                return featureAttribute.Name;
            }
            return extensionId;
        }

        private ExtensionEntry BuildEntry(ExtensionDescriptor descriptor)
        {
            foreach (var loader in loaders)
            {
                ExtensionEntry entry = loader.Load(descriptor);
                if (entry != null)
                    return entry;
            }

            Logger.WarnFormat("No suitable loader found for extension \"{0}\"", descriptor.Id);
            return null;
        }
    }
}