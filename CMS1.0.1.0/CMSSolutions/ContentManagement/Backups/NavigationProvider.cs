using System.Collections.Generic;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Localization;
using CMSSolutions.Web.UI.Navigation;

namespace CMSSolutions.ContentManagement.Backups
{
    [Feature(Constants.Areas.Backups)]
    public class NavigationProvider : INavigationProvider
    {
        public NavigationProvider()
        {
            T = NullLocalizer.Instance;
        }

        public Localizer T { get; set; }

        public void GetNavigation(NavigationBuilder builder)
        {
            builder.Add(T("Configuration"),
                menu => menu.Add(T("Backups"), "5", item => item
                    .Action("Index", "Backup", new { Constants.Areas.Backups })
                    .IconCssClass("cx-icon cx-icon-backups")
                    .Permission(BackupsPermissions.ManageBackups)));
        }
    }

    public class FeatureProvider : IFeatureProvider
    {
        public IEnumerable<FeatureDescriptor> AvailableFeatures()
        {
            yield return new FeatureDescriptor
            {
                Id = Constants.Areas.Backups,
                Name = "Backups",
                Category = "Configuration"
            };
        }
    }
}