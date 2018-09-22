using System;
using System.Collections.Generic;
using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.Environment.Extensions.Folders
{
    internal class ApplicationFolders : IExtensionFolders
    {
        private readonly Func<IEnumerable<string>> getDependencies;

        public ApplicationFolders(Func<IEnumerable<string>> getDependencies)
        {
            this.getDependencies = getDependencies;
        }

        public IEnumerable<ExtensionDescriptor> AvailableExtensions()
        {
            var descriptor = new ExtensionDescriptor
                {
                    Location = "~/",
                    Id = Constants.Areas.Application,
                    ExtensionType = "Module",
                    Name = Constants.Areas.Application
                };

            var features = new List<FeatureDescriptor>();

            // Default feature
            features.Insert(0, new FeatureDescriptor
            {
                Id = Constants.Areas.Application,
                Name = "Application",
                Category = "Core",
                Extension = descriptor,
                Dependencies = getDependencies()
            });

            descriptor.Features = features;

            yield return descriptor;
        }
    }
}