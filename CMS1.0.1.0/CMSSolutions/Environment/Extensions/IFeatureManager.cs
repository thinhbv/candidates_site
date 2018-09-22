using System;
using System.Collections.Generic;
using System.Linq;
using Castle.Core.Logging;
using CMSSolutions.Environment.Descriptor;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Localization;

namespace CMSSolutions.Environment.Extensions
{
    public interface IFeatureManager : IDependency
    {
        /// <summary>
        /// Enables a list of features.
        /// </summary>
        /// <param name="featureIds">The IDs for the features to be enabled.</param>
        /// <param name="force">Boolean parameter indicating if the feature should enable it's dependencies if required or fail otherwise.</param>
        /// <returns>An enumeration with the enabled feature IDs.</returns>
        IEnumerable<string> EnableFeatures(IEnumerable<string> featureIds, bool force);

        /// <summary>
        /// Disables a list of features.
        /// </summary>
        /// <param name="featureIds">The IDs for the features to be disabled.</param>
        /// <param name="force">Boolean parameter indicating if the feature should disable the features which depend on it if required or fail otherwise.</param>
        /// <returns>An enumeration with the disabled feature IDs.</returns>
        IEnumerable<string> DisableFeatures(IEnumerable<string> featureIds, bool force);

        /// <summary>
        /// Retrieves the available features.
        /// </summary>
        /// <returns>An enumeration of feature descriptors for the available features.</returns>
        IEnumerable<FeatureDescriptor> GetAvailableFeatures();
    }

    public class FeatureManager : IFeatureManager
    {
        private readonly IShellDescriptorManager shellDescriptorManager;
        private readonly IExtensionManager extensionManager;
        private readonly ShellDescriptor shellDescriptor;

        public FeatureManager(IShellDescriptorManager shellDescriptorManager, IExtensionManager extensionManager, ShellDescriptor shellDescriptor)
        {
            this.shellDescriptorManager = shellDescriptorManager;
            this.extensionManager = extensionManager;
            this.shellDescriptor = shellDescriptor;
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
        }

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        public IEnumerable<string> EnableFeatures(IEnumerable<string> featureIds, bool force)
        {
            var enabledFeatures = shellDescriptor.Features.ToList();

            IDictionary<FeatureDescriptor, bool> availableFeatures = GetAvailableFeatures()
                .ToDictionary(featureDescriptor => featureDescriptor,
                                featureDescriptor => enabledFeatures.FirstOrDefault(shellFeature => shellFeature.Name == featureDescriptor.Id) != null);

            var featuresToEnable = featureIds
                .Select(featureId => EnableFeature(featureId, availableFeatures, force)).ToList()
                .SelectMany(ies => ies.Select(s => s)).ToList();

            if (featuresToEnable.Any())
            {
                foreach (var featureId in featuresToEnable)
                {
                    enabledFeatures.Add(new ShellFeature { Name = featureId });
                    Logger.InfoFormat("{0} was enabled", featureId);
                }

                shellDescriptorManager.UpdateShellDescriptor(shellDescriptor.SerialNumber, enabledFeatures);
            }

            return featuresToEnable;
        }

        /// <summary>
        /// Enables a feature.
        /// </summary>
        /// <param name="featureId">The ID of the feature to be enabled.</param>
        /// <param name="availableFeatures">A dictionary of the available feature descriptors and their current state (enabled / disabled).</param>
        /// <param name="force">Boolean parameter indicating if the feature should enable it's dependencies if required or fail otherwise.</param>
        /// <returns>An enumeration of the enabled features.</returns>
        private IEnumerable<string> EnableFeature(string featureId, IDictionary<FeatureDescriptor, bool> availableFeatures, bool force)
        {
            var getDisabledDependencies =
                new Func<string, IDictionary<FeatureDescriptor, bool>, IDictionary<FeatureDescriptor, bool>>(
                    (currentFeatureId, featuresState) =>
                    {
                        KeyValuePair<FeatureDescriptor, bool> feature = featuresState.Single(featureState => featureState.Key.Id.Equals(currentFeatureId, StringComparison.OrdinalIgnoreCase));

                        // Retrieve disabled dependencies for the current feature
                        return feature.Key.Dependencies
                            .Select(fId => featuresState.Single(featureState => featureState.Key.Id.Equals(fId, StringComparison.OrdinalIgnoreCase)))
                            .Where(featureState => !featureState.Value)
                            .ToDictionary(f => f.Key, f => f.Value);
                    });

            IEnumerable<string> featuresToEnable = GetAffectedFeatures(featureId, availableFeatures, getDisabledDependencies);
            if (featuresToEnable.Count() > 1 && !force)
            {
                Logger.Warn("Additional features need to be enabled.");
                return Enumerable.Empty<string>();
            }

            return featuresToEnable;
        }

        private static IEnumerable<string> GetAffectedFeatures(string featureId, IDictionary<FeatureDescriptor, bool> features, Func<string, IDictionary<FeatureDescriptor, bool>, IDictionary<FeatureDescriptor, bool>> getAffectedDependencies)
        {
            var dependencies = new List<string> { featureId };

            foreach (KeyValuePair<FeatureDescriptor, bool> dependency in getAffectedDependencies(featureId, features))
            {
                dependencies.AddRange(GetAffectedFeatures(dependency.Key.Id, features, getAffectedDependencies));
            }

            return dependencies;
        }

        public IEnumerable<string> DisableFeatures(IEnumerable<string> featureIds, bool force)
        {
            var enabledFeatures = shellDescriptor.Features.ToList();

            IDictionary<FeatureDescriptor, bool> availableFeatures = GetAvailableFeatures()
                .ToDictionary(featureDescriptor => featureDescriptor,
                                featureDescriptor => enabledFeatures.FirstOrDefault(shellFeature => shellFeature.Name.Equals(featureDescriptor.Id)) != null);

            var featuresToDisable = featureIds
                .Select(featureId => DisableFeature(featureId, availableFeatures, force)).ToList()
                .SelectMany(ies => ies.Select(s => s)).ToList();

            if (featuresToDisable.Any())
            {
                foreach (var featureId in featuresToDisable)
                {
                    enabledFeatures.RemoveAll(shellFeature => shellFeature.Name == featureId);
                    Logger.InfoFormat("{0} was disabled", featureId);
                }

                shellDescriptorManager.UpdateShellDescriptor(shellDescriptor.SerialNumber, enabledFeatures);
            }

            return featuresToDisable;
        }

        /// <summary>
        /// Disables a feature.
        /// </summary>
        /// <param name="featureId">The ID of the feature to be enabled.</param>
        /// <param name="availableFeatures"></param>
        /// <param name="force">Boolean parameter indicating if the feature should enable it's dependencies if required or fail otherwise.</param>
        /// <returns>An enumeration of the disabled features.</returns>
        private IEnumerable<string> DisableFeature(string featureId, IDictionary<FeatureDescriptor, bool> availableFeatures, bool force)
        {
            var getEnabledDependants =
                new Func<string, IDictionary<FeatureDescriptor, bool>, IDictionary<FeatureDescriptor, bool>>(
                    (currentFeatureId, fs) => fs.Where(f => f.Value && f.Key.Dependencies != null && f.Key.Dependencies.Select(s => s.ToLowerInvariant()).Contains(currentFeatureId.ToLowerInvariant()))
                    .ToDictionary(f => f.Key, f => f.Value));

            IEnumerable<string> featuresToDisable = GetAffectedFeatures(featureId, availableFeatures, getEnabledDependants);
            if (featuresToDisable.Count() > 1 && !force)
            {
                Logger.Warn("Additional features need to be disabled.");
                return Enumerable.Empty<string>();
            }

            return featuresToDisable;
        }

        public IEnumerable<FeatureDescriptor> GetAvailableFeatures()
        {
            return extensionManager.AvailableFeatures();
        }
    }
}