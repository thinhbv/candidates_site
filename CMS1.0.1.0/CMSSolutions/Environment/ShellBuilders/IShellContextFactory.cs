using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Autofac;
using Castle.Core.Logging;
using CMSSolutions.Configuration;
using CMSSolutions.Environment.Descriptor;
using CMSSolutions.Environment.Descriptor.Models;

namespace CMSSolutions.Environment.ShellBuilders
{
    /// <summary>
    /// High-level coordinator that exercises other component capabilities to
    /// build all of the artifacts for a running shell given a tenant settings.
    /// </summary>
    public interface IShellContextFactory
    {
        /// <summary>
        /// Builds a shell context given a specific tenant settings structure
        /// </summary>
        ShellContext CreateShellContext(ShellSettings settings);

        /// <summary>
        /// Builds a shell context given a specific description of features and parameters.
        /// Shell's actual current descriptor has no effect. Does not use or update descriptor cache.
        /// </summary>
        ShellContext CreateDescribedContext(ShellSettings settings, ShellDescriptor shellDescriptor);
    }

    public class ShellContextFactory : IShellContextFactory
    {
        private readonly IShellDescriptorCache shellDescriptorCache;
        private readonly ICompositionStrategy compositionStrategy;
        private readonly IShellContainerFactory shellContainerFactory;

        public ShellContextFactory(
            IShellDescriptorCache shellDescriptorCache,
            ICompositionStrategy compositionStrategy,
            IShellContainerFactory shellContainerFactory)
        {
            this.shellDescriptorCache = shellDescriptorCache;
            this.compositionStrategy = compositionStrategy;
            this.shellContainerFactory = shellContainerFactory;
            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public ShellContext CreateShellContext(ShellSettings settings)
        {
            Logger.DebugFormat("Creating shell context for tenant {0}", settings.Name);

            var knownDescriptor = shellDescriptorCache.Fetch(settings.Name);
            if (knownDescriptor == null)
            {
                Logger.Info("No descriptor cached. Starting with minimum components.");
                var multiTenancy = Convert.ToBoolean(ConfigurationManager.AppSettings["CMS.MultiTenancy"]);
                knownDescriptor = MinimumShellDescriptor(multiTenancy);
            }

            if (!string.IsNullOrEmpty(settings.DataProvider))
            {
                var provider = CMSConfigurationSection.Instance.Data.Providers[settings.DataProvider];
                if (provider != null)
                {
                    if (knownDescriptor.Features.All(x => x.Name != provider.Feature))
                    {
                        knownDescriptor.Features.Insert(1, new ShellFeature { Name = provider.Feature });
                    }
                }
            }

            var blueprint = compositionStrategy.Compose(settings, knownDescriptor);
            var shellScope = shellContainerFactory.CreateContainer(settings, blueprint);

            ShellDescriptor currentDescriptor;
            using (var standaloneEnvironment = shellScope.CreateWorkContextScope())
            {
                var shellDescriptorManager = standaloneEnvironment.Resolve<IShellDescriptorManager>();
                currentDescriptor = shellDescriptorManager.GetShellDescriptor();

                if (currentDescriptor == null)
                {
                    shellDescriptorManager.UpdateShellDescriptor(0, knownDescriptor.Features, false);
                }
                else
                {
                    // Detect new auto activated features
                    if (knownDescriptor.SerialNumber == currentDescriptor.SerialNumber)
                    {
                        var features = new List<ShellFeature>(currentDescriptor.Features);
                        var hasNew = false;
                        foreach (var feature in knownDescriptor.Features)
                        {
                            if (features.All(x => x.Name != feature.Name))
                            {
                                features.Add(feature);
                                hasNew = true;
                            }
                        }

                        if (hasNew)
                        {
                            currentDescriptor.Features = features;
                            shellDescriptorManager.UpdateShellDescriptor(currentDescriptor.SerialNumber, features, false);
                        }
                    }
                }
            }

            if (currentDescriptor != null && knownDescriptor.SerialNumber != currentDescriptor.SerialNumber)
            {
                Logger.Info("Newer descriptor obtained. Rebuilding shell container.");

                shellDescriptorCache.Store(settings.Name, currentDescriptor);
                blueprint = compositionStrategy.Compose(settings, currentDescriptor);
                shellScope.Dispose();
                shellScope = shellContainerFactory.CreateContainer(settings, blueprint);
            }

            if (currentDescriptor == null)
            {
                currentDescriptor = knownDescriptor;
            }

            return new ShellContext
            {
                Settings = settings,
                Descriptor = currentDescriptor,
                Blueprint = blueprint,
                LifetimeScope = shellScope,
                Shell = shellScope.Resolve<ICMSShell>(),
            };
        }

        private static ShellDescriptor MinimumShellDescriptor(bool multiTenancy)
        {
            var features = new List<ShellFeature>
                               {
                                   new ShellFeature {Name = "CMSSolutions"},
                                   new ShellFeature {Name = "Default"},
                                   new ShellFeature {Name = "Dashboard"}
                               };

            if (!multiTenancy)
            {
                features.Add(new ShellFeature { Name = Constants.Areas.Application });
            }

            return new ShellDescriptor
                       {
                           SerialNumber = 0,
                           Features = features
                       };
        }

        public ShellContext CreateDescribedContext(ShellSettings settings, ShellDescriptor shellDescriptor)
        {
            Logger.InfoFormat("Creating described context for tenant {0}", settings.Name);

            var blueprint = compositionStrategy.Compose(settings, shellDescriptor);
            var shellScope = shellContainerFactory.CreateContainer(settings, blueprint);

            return new ShellContext
            {
                Settings = settings,
                Descriptor = shellDescriptor,
                Blueprint = blueprint,
                LifetimeScope = shellScope,
                Shell = shellScope.Resolve<ICMSShell>(),
            };
        }
    }
}