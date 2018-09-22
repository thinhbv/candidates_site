using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using Autofac.Core;
using Castle.Core.Logging;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Environment.ShellBuilders.Models;

namespace CMSSolutions.Environment.ShellBuilders
{
    /// <summary>
    /// Service at the host level to transform the cachable descriptor into the loadable blueprint.
    /// </summary>
    public interface ICompositionStrategy
    {
        /// <summary>
        /// Using information from the IExtensionManager, transforms and populates all of the
        /// blueprint model the shell builders will need to correctly initialize a tenant IoC container.
        /// </summary>
        ShellBlueprint Compose(ShellSettings settings, ShellDescriptor descriptor);
    }

    public class CompositionStrategy : ICompositionStrategy
    {
        private readonly IExtensionManager extensionManager;

        public CompositionStrategy(IExtensionManager extensionManager)
        {
            this.extensionManager = extensionManager;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public ShellBlueprint Compose(ShellSettings settings, ShellDescriptor descriptor)
        {
            Logger.Debug("Composing blueprint");

            var enabledFeatures = extensionManager.EnabledFeatures(descriptor).ToList();
            var applicationFeature = enabledFeatures.FirstOrDefault(x => x.Id == Constants.Areas.Application);
            if (applicationFeature != null)
            {
                EnabledDependencyFeatures(enabledFeatures, descriptor, applicationFeature);
            }

            var features = extensionManager.LoadFeatures(enabledFeatures);
            var excludedTypes = GetExcludedTypes(features);
            var modules = BuildBlueprint(features, IsModule, BuildModule, excludedTypes);
            var dependencies = BuildBlueprint(features, IsDependency, BuildDependency, excludedTypes);
            var controllers = BuildBlueprint(features, IsController, BuildController, excludedTypes);
            var httpControllers = BuildBlueprint(features, IsHttpController, BuildController, excludedTypes);

            var result = new ShellBlueprint
            {
                Settings = settings,
                Descriptor = descriptor,
                Dependencies = dependencies.Concat(modules).ToArray(),
                Controllers = controllers,
                HttpControllers = httpControllers
            };

            Logger.Debug("Done composing blueprint");
            return result;
        }

        private void EnabledDependencyFeatures(List<FeatureDescriptor> enabledFeatures, ShellDescriptor shellDescriptor, FeatureDescriptor featureDescriptor)
        {
            foreach (var dependency in featureDescriptor.Dependencies)
            {
                if (enabledFeatures.All(x => x.Id != dependency))
                {
                    var features = extensionManager.EnabledFeature(dependency);
                    foreach (var feature in features)
                    {
                        enabledFeatures.Add(feature);
                        var shellFeatures = new List<ShellFeature>(shellDescriptor.Features)
                                                {
                                                    new ShellFeature {Name = feature.Id}
                                                };
                        shellDescriptor.Features = shellFeatures;
                        EnabledDependencyFeatures(enabledFeatures, shellDescriptor, feature);
                    }
                }
            }
        }

        private static IEnumerable<string> GetExcludedTypes(IEnumerable<Feature> features)
        {
            var excludedTypes = new HashSet<string>();

            // Identify replaced types
            foreach (Feature feature in features)
            {
                foreach (Type type in feature.ExportedTypes)
                {
                    foreach (SuppressDependencyAttribute replacedType in type.GetCustomAttributes(typeof(SuppressDependencyAttribute), false))
                    {
                        excludedTypes.Add(replacedType.FullName);
                    }
                }
            }

            return excludedTypes;
        }

        private static IEnumerable<T> BuildBlueprint<T>(
            IEnumerable<Feature> features,
            Func<Type, bool> predicate,
            Func<Type, Feature, T> selector,
            IEnumerable<string> excludedTypes)
        {
            // Load types excluding the replaced types
            return features.SelectMany(
                feature => feature.ExportedTypes
                               .Where(predicate)
                               .Where(type => !excludedTypes.Contains(type.FullName))
                               .Select(type => selector(type, feature)))
                .ToArray();
        }

        private static bool IsModule(Type type)
        {
            return typeof(IModule).IsAssignableFrom(type);
        }

        private static DependencyBlueprint BuildModule(Type type, Feature feature)
        {
            return new DependencyBlueprint { Type = type, Feature = feature };
        }

        private static bool IsDependency(Type type)
        {
            return typeof(IDependency).IsAssignableFrom(type);
        }

        private static DependencyBlueprint BuildDependency(Type type, Feature feature)
        {
            return new DependencyBlueprint
            {
                Type = type,
                Feature = feature
            };
        }

        private static bool IsController(Type type)
        {
            return typeof(IController).IsAssignableFrom(type);
        }

        private static bool IsHttpController(Type type)
        {
            return typeof(IHttpController).IsAssignableFrom(type);
        }

        private static ControllerBlueprint BuildController(Type type, Feature feature)
        {
            var areaName = feature.Descriptor.Id;

            if (areaName == Constants.Areas.Application)
            {
                areaName = "";
            }

            var controllerName = type.Name;
            if (controllerName.EndsWith("Controller"))
                controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);

            // Detect have sub area name
            // ReSharper disable PossibleNullReferenceException
            if (type.FullName.Contains(".Controllers."))
            // ReSharper restore PossibleNullReferenceException
            {
                var split = type.FullName.Split('.').ToList();
                var indexOfControllers = split.IndexOf("Controllers");
                var indexOfController = split.IndexOf(type.Name);

                if (indexOfController - indexOfControllers > 1)
                {
                    var subArea = string.Join(".", split.ToArray(), indexOfControllers + 1, indexOfController - indexOfControllers - 1);
                    areaName = (areaName + "." + subArea).Trim('.');
                }
            }

            return new ControllerBlueprint
            {
                Type = type,
                Feature = feature,
                AreaName = areaName,
                ControllerName = controllerName,
            };
        }
    }
}