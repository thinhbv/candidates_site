using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Caching;
using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.Environment.Extensions.Folders
{
    public class FrameworkExtension : IExtensionFolders
    {
        private readonly ICacheManager cacheManager;

        public FrameworkExtension(ICacheManager cacheManager)
        {
            this.cacheManager = cacheManager;
        }

        public IEnumerable<ExtensionDescriptor> AvailableExtensions()
        {
            yield return cacheManager.Get("Framework_AvailableExtensions", ctx =>
            {
                var descriptor = new ExtensionDescriptor
                {
                    Location = "~/Views",
                    Id = "CMSSolutions",
                    ExtensionType = "Module",
                    Name = "CMSSolutions"
                };

                var types = typeof(FrameworkExtension).Assembly.GetExportedTypes();
                var featureProvider = typeof(IFeatureProvider);

                var features = types.Where(x => x.IsClass && !x.IsAbstract && featureProvider.IsAssignableFrom(x))
                    .Select(x => (IFeatureProvider)Activator.CreateInstance(x))
                    .SelectMany(x => x.AvailableFeatures())
                    .ToList();

                foreach (var feature in features)
                {
                    feature.Extension = descriptor;
                }

                // Default feature
                features.Insert(0, new FeatureDescriptor
                {
                    Id = "CMSSolutions",
                    Name = "CMS",
                    Category = "Core",
                    Extension = descriptor
                });

                descriptor.Features = features;

                return descriptor;
            });
        }
    }
}