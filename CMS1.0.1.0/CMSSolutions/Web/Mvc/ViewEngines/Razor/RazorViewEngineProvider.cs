using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Castle.Core.Logging;
using CMSSolutions.DisplayManagement.Descriptors.ShapeTemplateStrategy;

namespace CMSSolutions.Web.Mvc.ViewEngines.Razor
{
    public class RazorViewEngineProvider : IViewEngineProvider, IShapeTemplateViewEngine
    {
        public RazorViewEngineProvider()
        {
            Logger = NullLogger.Instance;
            RazorCompilationEventsShim.EnsureInitialized();
        }

        private static readonly string[] disabledFormats = { "~/Disabled" };

        public ILogger Logger { get; set; }

        public IViewEngine CreateThemeViewEngine(CreateThemeViewEngineParams parameters)
        {
            // Area: if "area" in RouteData. Url hit for module...
            // Area-Layout Paths - no-op because LayoutViewEngine uses multi-pass instead of layout paths
            // Area-View Paths - no-op because LayoutViewEngine relies entirely on Partial view resolution
            // Area-Partial Paths - enable theming views associated with a module based on the route

            // Layout Paths - no-op because LayoutViewEngine uses multi-pass instead of layout paths
            // View Paths - no-op because LayoutViewEngine relies entirely on Partial view resolution
            // Partial Paths -
            //   {area}/{controller}/

            // for "routed" request views...
            // enable /Views/{area}/{controller}/{viewName}

            // enable /Views/{partialName}
            // enable /Views/"DisplayTemplates/"+{templateName}
            // enable /Views/"EditorTemplates/+{templateName}
            var partialViewLocationFormats = new[] {
                parameters.VirtualPath + "/Views/{1}/{0}.cshtml",
                parameters.VirtualPath + "/Views/{0}.cshtml"
            };

            var areaPartialViewLocationFormats = new[] {
                parameters.VirtualPath + "/Views/{2}/{1}/{0}.cshtml",
                parameters.VirtualPath + "/Views/{1}/{0}.cshtml"
            };

            var viewEngine = new RazorViewEngine
            {
                MasterLocationFormats = disabledFormats,
                ViewLocationFormats = disabledFormats,
                PartialViewLocationFormats = partialViewLocationFormats,
                AreaMasterLocationFormats = disabledFormats,
                AreaViewLocationFormats = disabledFormats,
                AreaPartialViewLocationFormats = areaPartialViewLocationFormats,
                ViewLocationCache = new ThemeViewLocationCache(parameters.VirtualPath),
            };

            return viewEngine;
        }

        public IViewEngine CreateModulesViewEngine(CreateModulesViewEngineParams parameters)
        {
            var areaFormats = new[] {
                                        "~/Modules/{2}/Views/{1}/{0}.cshtml",
                                        "~/Themes/{2}/Views/{1}/{0}.cshtml",
                                        "~/Views/Shared/{0}.cshtml"
                                    };

            var universalFormats = parameters.VirtualPaths
                .SelectMany(x => new[] {
                                           x + "/{1}/{0}.cshtml",
                                           x + "/Shared/{0}.cshtml"
                                       })
                .ToArray();

            var viewEngine = new RazorViewEngine
            {
                MasterLocationFormats = disabledFormats,
                ViewLocationFormats = universalFormats,
                PartialViewLocationFormats = universalFormats,
                AreaMasterLocationFormats = disabledFormats,
                AreaViewLocationFormats = areaFormats,
                AreaPartialViewLocationFormats = areaFormats,
            };

            return viewEngine;
        }

        public IViewEngine CreateBareViewEngine()
        {
            return new RazorViewEngine
            {
                MasterLocationFormats = disabledFormats,
                ViewLocationFormats = disabledFormats,
                PartialViewLocationFormats = disabledFormats,
                AreaMasterLocationFormats = disabledFormats,
                AreaViewLocationFormats = disabledFormats,
                AreaPartialViewLocationFormats = disabledFormats,
            };
        }

        public IEnumerable<string> DetectTemplateFileNames(IEnumerable<string> fileNames)
        {
            return fileNames.Where(fileName => fileName.EndsWith(".cshtml", StringComparison.OrdinalIgnoreCase));
        }
    }
}