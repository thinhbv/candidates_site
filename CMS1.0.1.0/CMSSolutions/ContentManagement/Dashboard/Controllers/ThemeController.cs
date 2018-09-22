using System;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions.Configuration.Services;
using CMSSolutions.ContentManagement.Dashboard.Services;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Web;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Dashboard.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Url(BaseUrl = "{DashboardBaseUrl}/themes")]
    [Feature(Constants.Areas.Dashboard)]
    public class ThemeController : BaseController
    {
        private readonly ShellSettings shellSettings;
        private readonly IExtensionManager extensionManager;
        private readonly SiteSettings siteSettings;
        private readonly ShellDescriptor shellDescriptor;

        public ThemeController(
            IWorkContextAccessor workContextAccessor,
            ShellSettings shellSettings,
            IExtensionManager extensionManager,
            SiteSettings siteSettings, ShellDescriptor shellDescriptor)
            : base(workContextAccessor)
        {
            this.shellSettings = shellSettings;
            this.extensionManager = extensionManager;
            this.siteSettings = siteSettings;
            this.shellDescriptor = shellDescriptor;
        }

        [Url("{BaseUrl}")]
        public ActionResult Index()
        {
            if (!CheckPermission(DashboardPermissions.ManageThemes))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Themes"));

            var currentTheme = siteSettings.Theme;

            var result = new ControlGridFormResult<ExtensionDescriptorModel>
            {
                Title = T("Themes"),
                FetchAjaxSource = GetThemes,
                UpdateActionName = "Update",
                ActionsColumnWidth = 220,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            result.AddColumn(x => x.Name);
            result.AddColumn(x => x.IsDefault)
                .HasHeaderText(T("Is Default"))
                .AlignCenter()
                .RenderAsStatusImage(showTrueOnly: true);

            result.AddColumn(x => x.Installed)
                .HasHeaderText(T("Installed"))
                .AlignCenter()
                .RenderAsStatusImage();

            result.AddRowAction(true)
                .HasText(T("Set Default"))
                .HasName("SetDefault")
                .HasValue(x => x.Id)
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Primary)
                .EnableWhen(x => x.Installed && x.Id != currentTheme);

            result.AddRowAction(true)
                .HasText(T("Install"))
                .HasName("Install")
                .HasValue(x => x.Id)
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Success)
                .EnableWhen(x => !x.Installed);

            result.AddRowAction(true)
                .HasText(T("Uninstall"))
                .HasName("Uninstall")
                .HasValue(x => x.Id)
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Danger)
                .EnableWhen(x => x.Installed && x.Id != currentTheme && x.Id != "Default");

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");

            return result;
        }

        [HttpPost, FormButton("Save")]
        [Url("{BaseUrl}/update")]
        public ActionResult Update()
        {
            throw new NotSupportedException();
        }

        [ActionName("Update")]
        [HttpPost, FormButton("SetDefault")]
        public ActionResult SetDefault(string id)
        {
            if (!CheckPermission(DashboardPermissions.ManageThemes))
            {
                return new HttpUnauthorizedResult();
            }

            siteSettings.Theme = id;
            var settingService = WorkContext.Resolve<ISettingService>();
            settingService.SaveSetting(siteSettings);
            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE");
        }

        [ActionName("Update")]
        [HttpPost, FormButton("Install")]
        public ActionResult Install(string id)
        {
            if (!CheckPermission(DashboardPermissions.ManageThemes))
            {
                return new HttpUnauthorizedResult();
            }

            var moduleService = WorkContext.Resolve<IModuleService>();
            moduleService.EnableFeatures(new[] { id }, true);
            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE");
        }

        [ActionName("Update")]
        [HttpPost, FormButton("Uninstall")]
        public ActionResult Uninstall(string id)
        {
            if (!CheckPermission(DashboardPermissions.ManageThemes))
            {
                return new HttpUnauthorizedResult();
            }

            if (id == siteSettings.Theme)
            {
                return new AjaxResult().Alert(T("Cannot uninstall current default theme."));
            }
            var moduleService = WorkContext.Resolve<IModuleService>();
            moduleService.DisableFeatures(new[] { id }, true);
            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE");
        }

        private ControlGridAjaxData<ExtensionDescriptorModel> GetThemes(ControlGridFormRequest controlGridFormRequest)
        {
            var themes = extensionManager.AvailableExtensions().Where(IsAllowedTheme).ToList();
            return new ControlGridAjaxData<ExtensionDescriptorModel>(themes.Select(x => new ExtensionDescriptorModel
            {
                Id = x.Id,
                Name = x.Name,
                Installed = shellDescriptor.Features.Any(y => y.Name == x.Id) || x.Id == "Default",
                IsDefault = x.Id == siteSettings.Theme
            }).OrderByDescending(x => x.Installed).ThenBy(x => x.Name));
        }

        private bool IsAllowedTheme(ExtensionDescriptor extensionDescriptor)
        {
            if (!DefaultExtensionTypes.IsTheme(extensionDescriptor.ExtensionType))
            {
                return false;
            }

            if (extensionDescriptor.Name == "Dashboard")
            {
                return false;
            }

            if (shellSettings.Themes.Length > 0)
            {
                return shellSettings.Themes.Contains(extensionDescriptor.Id);
            }

            return true;
        }

        private class ExtensionDescriptorModel
        {
            public string Id { get; set; }

            public string Name { get; set; }

            public bool Installed { get; set; }

            public bool IsDefault { get; set; }
        }
    }
}