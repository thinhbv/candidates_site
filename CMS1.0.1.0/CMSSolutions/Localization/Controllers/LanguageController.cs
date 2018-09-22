using System;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions.Configuration.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization.Models;
using CMSSolutions.Localization.Services;
using CMSSolutions.Web;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Routing;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Localization.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Localization)]
    public class LanguageController : BaseController
    {
        private readonly ILanguageService languageService;
        private readonly SiteSettings siteSettings;

        public LanguageController(IWorkContextAccessor workContextAccessor, ILanguageService languageService, SiteSettings siteSettings)
            : base(workContextAccessor)
        {
            this.languageService = languageService;
            this.siteSettings = siteSettings;
        }

        [Url("{DashboardBaseUrl}/languages")]
        public ActionResult Index()
        {
            if (!CheckPermission(StandardPermissions.FullAccess))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Languages"));

            var result = new ControlGridFormResult<Domain.Language>
            {
                Title = "Manage Languages",
                UpdateActionName = "Update",
                DefaultPageSize = 15,
                FetchAjaxSource = GetLanguages,
                EnablePaginate = true,
                EnableSorting = true,
                ActionsColumnWidth = 300,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            result.AddColumn(x => x.Name);
            result.AddColumn(x => x.CultureCode).HasHeaderText("Culture Code");
            result.AddColumn(x => x.Theme);
            result.AddColumn(x => x.Active).RenderAsStatusImage();
            result.AddColumn(x => x.Id).HasHeaderText("Default").RenderAsStatusImage(x => x.CultureCode == siteSettings.DefaultLanguage, false, true);
            result.AddColumn(x => x.SortOrder).HasHeaderText(T("Sort Order"));

            result.AddAction()
                .HasText(T("Create"))
                .HasIconCssClass("cx-icon cx-icon-add")
                .HasUrl(Url.Action("Create", RouteData.Values))
                .HasButtonStyle(ButtonStyle.Primary).HasParentClass(Constants.ContainerCssClassCol12)
                .ShowModalDialog();

            result.AddRowAction(true)
                .HasText(T("Set Default"))
                .HasName("SetDefault")
                .HasValue(x => x.CultureCode)
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Success)
                .EnableWhen(x => x.Active && x.CultureCode != siteSettings.DefaultLanguage);

            result.AddRowAction()
                .HasText(T("Translations"))
                .HasUrl(x => Url.Action("LocalizableStrings", "Localization", new { languageId = x.Id }))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Info);

            result.AddRowAction()
                .HasText(T("Edit"))
                .HasUrl(x => Url.Action("Edit", RouteData.Values.Merge(new { id = x.Id })))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Warning)
                .ShowModalDialog();

            result.AddRowAction(true)
                .HasText(T("Delete"))
                .HasName("Delete")
                .HasValue(x => x.Id.ToString())
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Danger)
                .HasConfirmMessage("Are you sure want delete this record?");

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");

            return result;
        }

        private ControlGridAjaxData<Domain.Language> GetLanguages(ControlGridFormRequest controlGridFormRequest)
        {
            int totals;
            var items = languageService.GetRecords(controlGridFormRequest, out totals);
            return new ControlGridAjaxData<Domain.Language>(items, totals);
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/languages/create")]
        public ActionResult Create()
        {
            if (!CheckPermission(StandardPermissions.FullAccess))
            {
                return new HttpUnauthorizedResult();
            }

            var result = new ControlFormResult<LanguageModel>(new LanguageModel { Active = true })
            {
                Title = T("Create Language"),
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            result.RegisterFileUploadOptions("ImageFlag.FileName", new ControlFileUploadOptions
            {
                AllowedExtensions = "jpg,jpeg,png,gif"
            });

            var cultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            result.RegisterExternalDataSource(x => x.CultureCode, cultures.ToList().OrderBy(x => x.DisplayName).ToDictionary(k => k.Name, v => v.DisplayName));

            var themeManager = WorkContext.Resolve<IThemeManager>();
            result.RegisterExternalDataSource(x => x.Theme, themeManager.GetInstalledThemes());

            result.ExcludeProperty(x => x.CultureCode2);

            return result;
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/languages/edit/{id}")]
        public ActionResult Edit(int id)
        {
            if (!CheckPermission(StandardPermissions.FullAccess))
            {
                return new HttpUnauthorizedResult();
            }

            LanguageModel model = languageService.GetById(id);
            model.CultureCode2 = model.CultureCode;

            var result = new ControlFormResult<LanguageModel>(model)
            {
                Title = T("Edit Language"),
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            if (model.CultureCode == siteSettings.DefaultLanguage)
            {
                result.ExcludeProperty(x => x.Active);
                result.AddHiddenValue("Active", "true");
            }

            result.RegisterFileUploadOptions("ImageFlag.FileName", new ControlFileUploadOptions
            {
                AllowedExtensions = "jpg,jpeg,png,gif"
            });

            var themeManager = WorkContext.Resolve<IThemeManager>();
            result.RegisterExternalDataSource(x => x.Theme, themeManager.GetInstalledThemes());

            result.ExcludeProperty(x => x.CultureCode);
            result.AddHiddenValue("CultureCode", model.CultureCode);

            return result;
        }

        [FormButton("Delete")]
        [HttpPost, ActionName("Update")]
        public ActionResult Delete(int id)
        {
            if (!CheckPermission(StandardPermissions.FullAccess))
            {
                return new HttpUnauthorizedResult();
            }

            var entity = languageService.GetById(id);
            languageService.Delete(entity);
            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE");
        }

        [FormButton("SetDefault")]
        [HttpPost, ActionName("Update")]
        public ActionResult SetDefault(string id)
        {
            if (!CheckPermission(StandardPermissions.FullAccess))
            {
                return new HttpUnauthorizedResult();
            }

            siteSettings.DefaultLanguage = id;
            var settingsService = WorkContext.Resolve<ISettingService>();
            settingsService.SaveSetting(siteSettings);
            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").CloseModalDialog();
        }

        [FormButton("Save")]
        [HttpPost, ValidateInput(false)]
        [Url("{DashboardBaseUrl}/languages/update")]
        public ActionResult Update(LanguageModel model)
        {
            if (!CheckPermission(StandardPermissions.FullAccess))
            {
                return new HttpUnauthorizedResult();
            }

            if (!ModelState.IsValid)
            {
                throw new InvalidModelStateException(ModelState);
            }

            var entity = model.Id == 0 ? new Domain.Language() : languageService.GetById(model.Id);

            entity.Name = model.Name;
            entity.CultureCode = model.CultureCode;
            entity.Active = model.Active;
            entity.SortOrder = model.SortOrder;
            entity.Theme = model.Theme;
            entity.IsBlocked = model.IsBlocked;
            entity.ImageFlag = model.ImageFlag;

            languageService.Save(entity);

            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").CloseModalDialog();
        }
    }
}