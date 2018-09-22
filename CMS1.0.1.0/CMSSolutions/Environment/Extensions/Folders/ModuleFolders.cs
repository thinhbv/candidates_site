using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CMSSolutions.Configuration;
using CMSSolutions.Environment.Descriptor;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.Environment.Extensions.Folders
{
    public class ModuleFolders : IExtensionFolders
    {
        private readonly IBuildManager buildManager;

        public ModuleFolders(IBuildManager buildManager)
        {
            this.buildManager = buildManager;
        }

        public IEnumerable<ExtensionDescriptor> AvailableExtensions()
        {
            foreach (ModuleProviderConfigurationElement module in CMSConfigurationSection.Instance.Modules)
            {
                var assembly = buildManager.GetReferencedAssembly(module.Id);
                if (assembly == null)
                {
                    continue;
                }

                var extensionDescriptor = new ExtensionDescriptor
                {
                    Id = module.Id,
                    Name = module.Name,
                    Location = "~/Modules/" + module.Id,
                    ExtensionType = "Module"
                };

                extensionDescriptor.Features = GetFeatures(extensionDescriptor, assembly, module.Category);

                yield return extensionDescriptor;
            }
        }

        private static IEnumerable<FeatureDescriptor> GetFeatures(ExtensionDescriptor extensionDescriptor, Assembly assembly, string category)
        {
            var types = assembly.GetExportedTypes();
            var featureProvider = typeof(IFeatureProvider);
            var featureDescriptors = new List<FeatureDescriptor>();

            var features = types.Where(x => x.IsClass && !x.IsAbstract && featureProvider.IsAssignableFrom(x))
                .Select(x => (IFeatureProvider)Activator.CreateInstance(x))
                .SelectMany(x => x.AvailableFeatures())
                .ToList();

            foreach (var feature in features)
            {
                feature.Extension = extensionDescriptor;
                featureDescriptors.Add(feature);
            }

            // Default feature of extension
            if (featureDescriptors.All(x => x.Id != extensionDescriptor.Id))
            {
                var defaultFeatureDescriptor = new FeatureDescriptor
                                               {
                                                   Id = extensionDescriptor.Id,
                                                   Name = extensionDescriptor.Name,
                                                   Category = category,
                                                   Extension = extensionDescriptor
                                               };
                featureDescriptors.Add(defaultFeatureDescriptor);
            }

            return featureDescriptors;
        } 
    }
}