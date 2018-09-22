using System;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.UI.Notify;

namespace CMSSolutions.ContentManagement.Dashboard.Services
{
    public interface IModuleService : IDependency
    {
        /// <summary>
        /// Enables a list of features.
        /// </summary>
        /// <param name="featureIds">The IDs for the features to be enabled.</param>
        /// <param name="force">Boolean parameter indicating if the feature should enable it's dependencies if required or fail otherwise.</param>
        void EnableFeatures(IEnumerable<string> featureIds, bool force);

        /// <summary>
        /// Disables a list of features.
        /// </summary>
        /// <param name="featureIds">The IDs for the features to be disabled.</param>
        /// <param name="force">Boolean parameter indicating if the feature should disable the features which depend on it if required or fail otherwise.</param>
        void DisableFeatures(IEnumerable<string> featureIds, bool force);
    }

    [Feature(Constants.Areas.Dashboard)]
    public class ModuleService : IModuleService
    {
        private readonly IFeatureManager featureManager;
        private readonly INotifier notifier;

        public ModuleService(IFeatureManager featureManager, INotifier notifier)
        {
            this.featureManager = featureManager;
            this.notifier = notifier;
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void EnableFeatures(IEnumerable<string> featureIds, bool force)
        {
            foreach (var featureId in featureManager.EnableFeatures(featureIds, force))
            {
                var featureName = featureManager.GetAvailableFeatures().First(f => f.Id.Equals(featureId, StringComparison.OrdinalIgnoreCase)).Name;
                notifier.Information(T("{0} was enabled", featureName));
            }
        }

        public void DisableFeatures(IEnumerable<string> featureIds, bool force)
        {
            foreach (string featureId in featureManager.DisableFeatures(featureIds, force))
            {
                var featureName = featureManager.GetAvailableFeatures().First(f => f.Id == featureId).Name;
                notifier.Information(T("{0} was disabled", featureName));
            }
        }
    }
}