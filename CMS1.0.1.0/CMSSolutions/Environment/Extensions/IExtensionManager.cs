using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.Environment.Extensions
{
    public interface IExtensionManager
    {
        IEnumerable<ExtensionDescriptor> AvailableExtensions();

        IEnumerable<FeatureDescriptor> AvailableFeatures();

        ExtensionDescriptor GetExtension(string id);

        IEnumerable<Feature> LoadFeatures(IEnumerable<FeatureDescriptor> featureDescriptors);
    }

    public static class ExtensionManagerExtensions
    {
        public static IEnumerable<FeatureDescriptor> EnabledFeatures(this IExtensionManager extensionManager, ShellDescriptor descriptor)
        {
            return extensionManager.AvailableFeatures().Where(fd => descriptor.Features.Any(sf => sf.Name == fd.Id));
        }

        public static IEnumerable<FeatureDescriptor> EnabledFeature(this IExtensionManager extensionManager, string featureName)
        {
            return extensionManager.AvailableFeatures().Where(fd => fd.Id == featureName);
        }
    }
}