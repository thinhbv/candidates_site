using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.Core.Logging;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.Web.Mvc.ViewEngines.ThemeAwareness
{
    public class ThemeAwareViewEngine : IThemeAwareViewEngine
    {
        private readonly WorkContext workContext;
        private readonly IEnumerable<IViewEngineProvider> viewEngineProviders;
        private readonly IConfiguredEnginesCache configuredEnginesCache;
        private readonly IExtensionManager extensionManager;
        private readonly ShellDescriptor shellDescriptor;
        private readonly IViewEngine nullEngines = new ViewEngineCollectionWrapper(Enumerable.Empty<IViewEngine>());

        public ThemeAwareViewEngine(
            WorkContext workContext,
            IEnumerable<IViewEngineProvider> viewEngineProviders,
            IConfiguredEnginesCache configuredEnginesCache,
            IExtensionManager extensionManager,
            ShellDescriptor shellDescriptor)
        {
            this.workContext = workContext;
            this.viewEngineProviders = viewEngineProviders;
            this.configuredEnginesCache = configuredEnginesCache;
            this.extensionManager = extensionManager;
            this.shellDescriptor = shellDescriptor;

            Logger = NullLogger.Instance;
        }

        public ILogger Logger { get; set; }

        public ViewEngineResult FindPartialView(ControllerContext controllerContext, string partialViewName, bool useCache, bool useDeepPaths)
        {
            var engines = nullEngines;

            if (partialViewName.StartsWith("/") || partialViewName.StartsWith("~"))
            {
                engines = BareEngines();
            }
            else if (workContext.CurrentTheme != null)
            {
                engines = useDeepPaths ? DeepEngines(workContext.CurrentTheme) : ShallowEngines(workContext.CurrentTheme);
            }

            return engines.FindPartialView(controllerContext, partialViewName, useCache);
        }

        public ViewEngineResult FindView(ControllerContext controllerContext, string viewName, string masterName, bool useCache, bool useDeepPaths)
        {
            var engines = nullEngines;

            if (viewName.StartsWith("/") || viewName.StartsWith("~"))
            {
                engines = BareEngines();
            }
            else if (workContext.CurrentTheme != null)
            {
                engines = useDeepPaths ? DeepEngines(workContext.CurrentTheme) : ShallowEngines(workContext.CurrentTheme);
            }

            return engines.FindView(controllerContext, viewName, masterName, useCache);
        }

        private IViewEngine BareEngines()
        {
            return configuredEnginesCache.BindBareEngines(() => new ViewEngineCollectionWrapper(viewEngineProviders.Select(vep => vep.CreateBareViewEngine())));
        }

        private IViewEngine ShallowEngines(ExtensionDescriptor theme)
        {
            //return _configuredEnginesCache.BindShallowEngines(theme.ThemeName, () => new ViewEngineCollectionWrapper(_viewEngineProviders.Select(vep => vep.CreateBareViewEngine())));
            return DeepEngines(theme);
        }

        private IViewEngine DeepEngines(ExtensionDescriptor theme)
        {
            return configuredEnginesCache.BindDeepEngines(theme.Id, () =>
            {
                // The order for searching for views is:
                // 1. Current "theme"
                // 2. Base themes of the current theme (in "base" order)
                // 3. Active features from modules in dependency order

                var engines = Enumerable.Empty<IViewEngine>();
                // 1. current theme
                engines = engines.Concat(CreateThemeViewEngines(theme));

                // 2. Base themes of the current theme (in "base" order)
                engines = GetBaseThemes(theme).Aggregate(engines, (current, baseTheme) => current.Concat(CreateThemeViewEngines(baseTheme)));

                // 3. Active features from modules in dependency order
                var enabledModules = extensionManager.EnabledFeatures(shellDescriptor)
                    .Reverse()  // reverse from (C <= B <= A) to (A => B => C)
                    .Where(fd => DefaultExtensionTypes.IsModule(fd.Extension.ExtensionType));

                var allLocations = enabledModules.Concat(enabledModules)
                    .Select(fd => fd.Extension.Location)
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .ToList();

                var moduleParams = new CreateModulesViewEngineParams { VirtualPaths = allLocations };
                engines = engines.Concat(viewEngineProviders.Select(vep => vep.CreateModulesViewEngine(moduleParams)));

                return new ViewEngineCollectionWrapper(engines);
            });
        }

        private IEnumerable<IViewEngine> CreateThemeViewEngines(ExtensionDescriptor theme)
        {
            var themeParams = new CreateThemeViewEngineParams { VirtualPath = theme.Location };
            return viewEngineProviders.Select(vep => vep.CreateThemeViewEngine(themeParams));
        }

        private IEnumerable<ExtensionDescriptor> GetBaseThemes(ExtensionDescriptor themeExtension)
        {
            if (themeExtension.Id.Equals("Dashboard", StringComparison.OrdinalIgnoreCase))
            {
                // Special case: conceptually, the base themes of "TheAdmin" is the list of all
                // enabled themes. This is so that any enabled theme can have controller/action/views
                // in the Admin of the site.
                return extensionManager
                    .EnabledFeatures(shellDescriptor)
                    .Reverse()  // reverse from (C <= B <= A) to (A => B => C)
                    .Select(fd => fd.Extension)
                    .Where(fd => DefaultExtensionTypes.IsTheme(fd.ExtensionType));
            }
            else
            {
                var availableFeatures = extensionManager.AvailableFeatures();
                var list = new List<ExtensionDescriptor>();
                while (true)
                {
                    if (themeExtension == null)
                        break;

                    if (String.IsNullOrEmpty(themeExtension.BaseTheme))
                        break;

                    var baseFeature = availableFeatures.FirstOrDefault(fd => fd.Id == themeExtension.BaseTheme);
                    if (baseFeature == null)
                    {
                        Logger.ErrorFormat("Base theme '{0}' of theme '{1}' not found in list of features", themeExtension.BaseTheme, themeExtension.Id);
                        break;
                    }

                    // Protect against potential infinite loop
                    if (list.Contains(baseFeature.Extension))
                    {
                        Logger.ErrorFormat("Base theme '{0}' of theme '{1}' ignored, as it seems there is recursion in base themes", themeExtension.BaseTheme, themeExtension.Id);
                        break;
                    }

                    list.Add(baseFeature.Extension);

                    themeExtension = baseFeature.Extension;
                }
                return list;
            }
        }

        public void ReleaseView(ControllerContext controllerContext, IView view)
        {
            throw new NotImplementedException();
        }
    }
}