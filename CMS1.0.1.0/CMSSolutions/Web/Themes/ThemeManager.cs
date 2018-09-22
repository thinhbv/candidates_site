using System.Collections.Generic;
using System.Linq;
using System.Web.Routing;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;

namespace CMSSolutions.Web.Themes
{
    public class ThemeManager : IThemeManager
    {
        private readonly IEnumerable<IThemeSelector> themeSelectors;
        private readonly IExtensionManager extensionManager;
        private readonly ShellDescriptor shellDescriptor;        

        public ThemeManager(IEnumerable<IThemeSelector> themeSelectors,
                            IExtensionManager extensionManager, ShellDescriptor shellDescriptor)
        {
            this.themeSelectors = themeSelectors;
            this.extensionManager = extensionManager;
            this.shellDescriptor = shellDescriptor;
        }

        public string[] GetInstalledThemes()
        {
            return extensionManager.AvailableExtensions()
                .Where(x => DefaultExtensionTypes.IsTheme(x.ExtensionType) && shellDescriptor.Features.Any(y => y.Name == x.Name))
                .Select(x => x.Name)
                .ToArray();
        }

        public ExtensionDescriptor GetRequestTheme(RequestContext requestContext)
        {
            var requestThemes = themeSelectors
                .Select(x => x.GetTheme(requestContext))
                .Where(x => x != null)
                .OrderByDescending(x => x.Priority);

            if (!requestThemes.Any())
                return extensionManager.GetExtension("Default");

            foreach (var theme in requestThemes)
            {
                var t = extensionManager.GetExtension(theme.Name);
                if (t != null && t.Name == "Dashboard" && theme.IsDashboard)
                {
                    return t;
                }

                if (t != null && shellDescriptor.Features.Any(x => x.Name == t.Id) && !theme.IsDashboard)
                {
                    return t;
                }
            }

            return extensionManager.GetExtension("Default");
        }

        public ExtensionDescriptor GetTheme(string theme)
        {
            return extensionManager.AvailableExtensions().FirstOrDefault(x => DefaultExtensionTypes.IsTheme(x.ExtensionType) && x.Name == theme);
        }
    }
}