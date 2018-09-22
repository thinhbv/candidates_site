using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Localization;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.ContentManagement.Dashboard
{
    [Feature(Constants.Areas.Dashboard)]
    public class NavigationProvider : INavigationProvider
    {
        public NavigationProvider()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder)
        {
            //builder.Add(T("Home"), "0", BuildHomeMenu);
            builder.Add(T("Configuration"), "99", BuildConfigurationMenu);
        }

        private static void BuildHomeMenu(NavigationItemBuilder builder)
        {
            builder.Permission(StandardPermissions.DashboardAccess);
            builder.IconCssClass("fa-home").Action("Index", "Dashboard", new { area = Constants.Areas.Dashboard });
        }

        private void BuildConfigurationMenu(NavigationItemBuilder builder)
        {
            builder.IconCssClass("fa-cog");
            builder.Add(T("Modules"), "1", b => b.Action("Features", "Module", new { area = Constants.Areas.Dashboard }).IconCssClass("fa-th").Permission(DashboardPermissions.ManageModules));
            builder.Add(T("Themes"), "2", b => b.Action("Index", "Theme", new { area = Constants.Areas.Dashboard }).IconCssClass("fa-tachometer").Permission(DashboardPermissions.ManageThemes));
            builder.Add(T("Extension Modules"), "3", b => b.Action("Index", "ModuleSettings", new { area = Constants.Areas.Dashboard }).IconCssClass("fa-tachometer").Permission(DashboardPermissions.ManageModuleSettings));
        }
    }

    public class FeatureProvider : IFeatureProvider
    {
        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
            {
                Id = Constants.Areas.Dashboard,
                Name = "Dashboard",
                Category = "Content",
                Dependencies = new string[] { }
            };
        }
    }
}