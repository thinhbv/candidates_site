using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json;
using CMSSolutions.ContentManagement.Menus.Domain;
using CMSSolutions.ContentManagement.Menus.Services;
using CMSSolutions.ContentManagement.Pages.Domain;
using CMSSolutions.ContentManagement.Pages.Models;
using CMSSolutions.ContentManagement.Pages.Services;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Routing;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Pages.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Pages)]
    public class PageController : BaseController
    {
        private readonly ShellDescriptor descriptor;
        private readonly IExtensionManager extensionManager;
        private readonly IHistoricPageService historicPageService;
        private readonly ILanguageManager languageManager;
        private readonly IMenuItemService menuItemService;
        private readonly IMenuService menuService;
        private readonly IPageService pageService;
        private readonly IPageTagService pageTagService;

        public PageController(
            IWorkContextAccessor workContextAccessor,
            IPageService pageService,
            IExtensionManager extensionManager,
            IMenuService menuService,
            IMenuItemService menuItemService,
            ShellDescriptor descriptor,
            ILanguageManager languageManager,
            IPageTagService pageTagService,
            IHistoricPageService historicPageService)
            : base(workContextAccessor)
        {
            this.pageService = pageService;
            this.extensionManager = extensionManager;
            this.menuService = menuService;
            this.menuItemService = menuItemService;
            this.descriptor = descriptor;
            this.languageManager = languageManager;
            this.pageTagService = pageTagService;
            this.historicPageService = historicPageService;
        }

        [Url("{DashboardBaseUrl}/pages/create")]
        public ActionResult Create()
        {
            if (!CheckPermission(PagesPermissions.ManagePages))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Pages"), Url.Action("Index", new { area = Constants.Areas.Pages }));
            WorkContext.Breadcrumbs.Add(T("Create"));

            var model = new PageModel { IsEnabled = true, Title = "" };

            var result = new ControlFormResult<PageModel>(model)
            {
                Title = T("Create Page").Text,
                UpdateActionName = "Update",
                CssClass = "form-edit-page",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            //result.AddAction(true, false, false).HasText(T("Preview")).HasUrl(Url.Action("Preview")).HasButtonStyle(ButtonStyle.Default);

            var themes = extensionManager.AvailableExtensions().Where(x => DefaultExtensionTypes.IsTheme(x.ExtensionType) && descriptor.Features.Any(f => f.Name == x.Id)).ToDictionary(k => k.Id, v => v.Name);
            result.RegisterExternalDataSource(x => x.Theme, themes);
            result.RegisterExternalDataSource(x => x.ShowOnMenuId, menuService.GetRecords().ToDictionary(k => k.Id, v => v.Name));

            // Page tags
            var tags = pageTagService.GetRecords();
            if (tags.Count > 0)
            {
                result.AddHiddenValue("RichtextCustomTags", JsonConvert.SerializeObject(tags.Select(x => new[] { x.Name, "[%" + x.Name + "%]" })));
            }

            return result;
        }

        [HttpPost, FormButton("Delete")]
        [ActionName("Update")]
        public ActionResult Delete(int id)
        {
            if (!CheckPermission(PagesPermissions.ManagePages))
            {
                return new HttpUnauthorizedResult();
            }

            var page = pageService.GetById(id);
            pageService.Delete(page);

            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE");
        }

        [Url("{DashboardBaseUrl}/pages/edit/{id}")]
        public ActionResult Edit(int id)
        {
            if (!CheckPermission(PagesPermissions.ManagePages))
            {
                return new HttpUnauthorizedResult();
            }

            PageModel model = pageService.GetById(id);

            WorkContext.Breadcrumbs.Add(T("Pages"), Url.Action("Index", new { area = Constants.Areas.Pages }));
            WorkContext.Breadcrumbs.Add(model.Title);
            WorkContext.Breadcrumbs.Add(T("Edit"));

            var menuItem = menuItemService.GetMenuItemByRefId(id);
            if (menuItem != null)
            {
                model.ShowOnMenuId = menuItem.MenuId;
            }

            var result = new ControlFormResult<PageModel>(model)
            {
                Title = T("Edit Page").Text,
                UpdateActionName = "Update",
                CssClass = "form-edit-page",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            result.AddAction(true, true, false).HasText(T("Save & Continue")).HasName("SaveAndContinue").HasButtonStyle(ButtonStyle.Info);

            result.AddAction(addToTop: false)
                .HasText(T("Preview"))
                .HasClientId("btnPreview")
                .HasButtonStyle(ButtonStyle.Info);

            var themes = extensionManager.AvailableExtensions().Where(x => DefaultExtensionTypes.IsTheme(x.ExtensionType) && descriptor.Features.Any(f => f.Name == x.Id)).ToDictionary(k => k.Id, v => v.Name);
            result.RegisterExternalDataSource(x => x.Theme, themes);
            result.RegisterExternalDataSource(x => x.ShowOnMenuId, menuService.GetRecords().ToDictionary(k => k.Id, v => v.Name));

            // Page tags
            var tags = pageTagService.GetRecords();
            if (tags.Count > 0)
            {
                result.AddHiddenValue("RichtextCustomTags", JsonConvert.SerializeObject(tags.Select(x => new[] { x.Name, "[%" + x.Name + "%]" })));
            }

            var scriptRegister = new ScriptRegister(WorkContext);
            scriptRegister.IncludeInline(Constants.Scripts.JQueryFormExtension);
            scriptRegister.IncludeInline(Constants.Scripts.JQueryFormParams);

            var sbScript = new StringBuilder(256);
            sbScript.Append("$(document).ready(function(){");
            sbScript.Append("$('#btnPreview').click(function(e){");
            sbScript.Append("var data = $('form').formParams();");
            sbScript.Append("var bodyContent = $('#idContentoEdit0').contents().find('body').html();");
            sbScript.Append("data.BodyContent = bodyContent;");
            sbScript.AppendFormat("$.form('{0}',", Url.Action("Preview"));
            sbScript.Append("data");
            sbScript.Append(").attr('target', '_blank').submit().remove(); return false;");
            sbScript.Append("});})");

            scriptRegister.IncludeInline(sbScript.ToString());

            return result;
        }

        [FormButton("EnableOrDisable")]
        [HttpPost, ActionName("Update")]
        public ActionResult EnableOrDisable(int id)
        {
            if (!CheckPermission(PagesPermissions.ManagePages))
            {
                return new HttpUnauthorizedResult();
            }

            var page = pageService.GetById(id);
            pageService.ToggleEnabled(id, !page.IsEnabled);
            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE");
        }

        [Url("{DashboardBaseUrl}/pages")]
        public ActionResult Index()
        {
            if (!CheckPermission(PagesPermissions.ManagePages))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Pages"));

            var result = new ControlGridFormResult<Page>
            {
                Title = T("Manage Pages").Text,
                UpdateActionName = "Update",
                FetchAjaxSource = GetPages,
                EnablePaginate = true,
                EnableSearch = true,
                DefaultPageSize = WorkContext.DefaultPageSize,
                ActionsColumnWidth = 380,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            result.AddColumn(x => x.Title).RenderAsHtml(x => string.Format("<a href=\"{1}\" target=\"_blank\">{0}</a>", x.Title, Url.Content("~/" + x.Slug)));
            result.AddColumn(x => x.Theme);
            result.AddColumn(x => x.IsEnabled).HasHeaderText(T("Enabled")).AlignCenter().RenderAsStatusImage();

            result.AddAction()
                .HasText(T("Tags"))
                .HasUrl(Url.Action("Index", "PageTag"))
                .HasIconCssClass("cx-icon cx-icon-tags")
                .HasButtonStyle(ButtonStyle.Info)
                .HasCssClass(Constants.RowLeft)
                .HasBoxButton(false);

            result.AddAction()
                .HasText(T("Create"))
                .HasIconCssClass("cx-icon cx-icon-add")
                .HasUrl(Url.Action("Create", RouteData.Values))
                .HasButtonStyle(ButtonStyle.Primary)
                .HasBoxButton(false)
                .HasCssClass(Constants.RowLeft);

            result.AddRowAction(true)
                .HasText(T("On/Off"))
                .HasName("EnableOrDisable")
                .HasValue(x => x.Id)
                .HasButtonSize(ButtonSize.ExtraSmall);

            result.AddRowAction()
                .HasText(T("Edit"))
                .HasUrl(x => Url.Action("Edit", RouteData.Values.Merge(new { id = x.Id })))
                .HasButtonSize(ButtonSize.ExtraSmall);

            var languages = languageManager.GetActiveLanguages(Constants.ThemeDefault, false);
            if (languages.Any())
            {
                result.AddRowAction()
                .HasText(T("Translations"))
                .HasUrl(x => Url.Action("Translations", RouteData.Values.Merge(new { id = x.Id })))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Info)
                .ShowModalDialog();
            }

            if (descriptor.Features.Any(x => x.Name == Constants.Areas.Widgets))
            {
                result.AddRowAction()
                .HasText(T("Widgets"))
                .HasUrl(x => Url.Action("Index", "Widget", RouteData.Values.Merge(new { pageId = x.Id, area = Constants.Areas.Widgets })))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Success);
            }

            result.AddRowAction()
                .HasText(T("History"))
                .HasUrl(x => Url.Action("History", RouteData.Values.Merge(new { pageId = x.Id })))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Warning);

            result.AddRowAction(true)
                .HasText(T("Delete"))
                .HasName("Delete")
                .HasValue(x => x.Id.ToString())
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Danger)
                .HasConfirmMessage(T(Constants.Messages.ConfirmDeleteRecord).Text);

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");

            return result;
        }

        [Url("{DashboardBaseUrl}/pages/history/{pageId}")]
        public ActionResult History(int pageId)
        {
            if (!CheckPermission(PagesPermissions.ManagePages))
            {
                return new HttpUnauthorizedResult();
            }

            var page = pageService.GetById(pageId);

            WorkContext.Breadcrumbs.Add(T("Pages"), Url.Action("Index", new { area = Constants.Areas.Pages }));
            WorkContext.Breadcrumbs.Add(page.Title);
            WorkContext.Breadcrumbs.Add(T("History"));

            var result = new ControlGridFormResult<HistoricPage>
            {
                Title = T("Manage Pages").Text,
                UpdateActionName = "Update",
                FetchAjaxSource = options => GetHistoricPages(options, pageId),
                EnablePaginate = true,
                EnableSearch = true,
                DefaultPageSize = WorkContext.DefaultPageSize,
                ActionsColumnWidth = 250,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            result.AddColumn(x => x.Title).RenderAsHtml(x => string.Format("<a href=\"{1}\" target=\"_blank\">{0}</a>", x.Title, Url.Content("~/" + x.Slug)));
            result.AddColumn(x => x.Theme);
            result.AddColumn(x => x.ArchivedDate, T("Archived Date"));

            result.AddRowAction()
                .HasText(T("View"))
                .HasUrl(x => Url.Action("ViewHistoricPage", RouteData.Values.Merge(new { id = x.Id })))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Primary);

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("RESTORE_ENTITY_COMPLETE");

            return result;
        }

        [Url("{DashboardBaseUrl}/pages/history/view/{id}")]
        public ActionResult ViewHistoricPage(int id)
        {
            if (!CheckPermission(PagesPermissions.ManagePages))
            {
                return new HttpUnauthorizedResult();
            }

            HistoricPageModel model = historicPageService.GetById(id);

            WorkContext.Breadcrumbs.Add(T("Pages"), Url.Action("Index", new { area = Constants.Areas.Pages }));
            WorkContext.Breadcrumbs.Add(model.Title);
            WorkContext.Breadcrumbs.Add(T("Edit"));

            var menuItem = menuItemService.GetMenuItemByRefId(id);
            if (menuItem != null)
            {
                model.ShowOnMenuId = menuItem.MenuId;
            }

            var result = new ControlFormResult<HistoricPageModel>(model)
            {
                Title = T("Edit Page").Text,
                UpdateActionName = "Update",
                CssClass = "form-edit-page",
                ReadOnly = true,
                CancelButtonText = T("Close"),
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            result.AddAction(true)
                .HasText(T("Restore Version"))
                .HasName("RestoreVersion")
                .HasButtonStyle(ButtonStyle.Warning)
                .HasConfirmMessage(T("Are you sure you want to restore this version of the page?").Text);

            var themes = extensionManager.AvailableExtensions().Where(x => DefaultExtensionTypes.IsTheme(x.ExtensionType) && descriptor.Features.Any(f => f.Name == x.Id)).ToDictionary(k => k.Id, v => v.Name);
            result.RegisterExternalDataSource(x => x.Theme, themes);
            result.RegisterExternalDataSource(x => x.ShowOnMenuId, menuService.GetRecords().ToDictionary(k => k.Id, v => v.Name));

            // Page tags
            var tags = pageTagService.GetRecords();
            if (tags.Count > 0)
            {
                result.AddHiddenValue("RichtextCustomTags", JsonConvert.SerializeObject(tags.Select(x => new[] { x.Name, "[%" + x.Name + "%]" })));
            }

            return result;
        }

        [ValidateInput(false), FormButton("SaveAndContinue"), ActionName("Update")]
        public ActionResult SaveAndContinue(PageModel model)
        {
            if (!CheckPermission(PagesPermissions.ManagePages))
            {
                return new HttpUnauthorizedResult();
            }

            Page page = new Page();

            if (model.Id != 0)
            {
                page = pageService.GetById(model.Id);
                var historicPage = new HistoricPage
                {
                    ArchivedDate = DateTime.UtcNow,
                    BodyContent = page.BodyContent,
                    CssClass = page.CssClass,
                    CultureCode = page.CultureCode,
                    IsEnabled = page.IsEnabled,
                    MetaDescription = page.MetaDescription,
                    MetaKeywords = page.MetaKeywords,
                    PageId = page.Id,
                    RefId = page.RefId,
                    Slug = page.Slug,
                    Theme = page.Theme,
                    Title = page.Title
                };
                historicPageService.Insert(historicPage);
            }

            page.Title = model.Title;
            page.IsEnabled = model.IsEnabled;
            page.MetaKeywords = model.MetaKeywords;
            page.MetaDescription = model.MetaDescription;
            page.BodyContent = model.BodyContent;
            page.CultureCode = model.CultureCode;
            page.RefId = model.RefId;
            page.Theme = model.Theme;
            page.CssClass = model.CssClass;
            page.Slug = string.IsNullOrEmpty(model.Slug) ? model.Title.ToSlugUrl() : model.Slug.Trim(' ', '/');

            pageService.Save(page);

            if (page.RefId == null)
            {
                if (model.ShowOnMenuId == null)
                {
                    if (model.Id != 0)
                    {
                        var menuItem = menuItemService.GetMenuItemByRefId(model.Id);
                        if (menuItem != null)
                        {
                            menuItemService.Delete(menuItem);
                        }
                    }
                }
                else
                {
                    var menuItem = menuItemService.GetMenuItemByRefId(model.Id) ?? new MenuItem
                    {
                        RefId = page.Id,
                        Position = 0,
                        Enabled = true,
                        IsExternalUrl = false,
                        Text = page.Title
                    };
                    menuItem.Url = page.Slug;
                    menuItem.MenuId = model.ShowOnMenuId.Value;
                    menuItemService.Save(menuItem);
                }
            }

            return null;
        }

        [Url("{DashboardBaseUrl}/pages/translate/{id}/{cultureCode}")]
        public ActionResult Translate(int id, string cultureCode)
        {
            if (!CheckPermission(PagesPermissions.ManagePages))
            {
                return new HttpUnauthorizedResult();
            }

            PageModel model = pageService.GetPageByLanguage(id, cultureCode);

            WorkContext.Breadcrumbs.Add(T("Pages"), Url.Action("Index", new { area = Constants.Areas.Pages }));
            if (model != null)
            {
                WorkContext.Breadcrumbs.Add(model.Title);
            }

            WorkContext.Breadcrumbs.Add(T("Translate"));
            WorkContext.Breadcrumbs.Add(cultureCode);

            var showSaveAndContinue = false;

            if (model == null)
            {
                model = pageService.GetById(id);
                model.Id = 0;
                model.CultureCode = cultureCode;
                model.RefId = id;
                ViewData.ModelState["Id"] = new ModelState { Value = new ValueProviderResult(Guid.Empty, Guid.Empty.ToString(), null) };
            }
            else
            {
                ViewData.ModelState["Id"] = new ModelState { Value = new ValueProviderResult(model.Id, model.Id.ToString(), null) };
                showSaveAndContinue = true;
            }

            var result = new ControlFormResult<PageModel>(model)
            {
                Title = T("Translate Page").Text,
                UpdateActionName = "Update",
                CssClass = "form-edit-page",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            result.AssignGridLayout(x => x.Id, 0, 0);
            result.AssignGridLayout(x => x.CultureCode, 0, 0);
            result.AssignGridLayout(x => x.RefId, 0, 0);
            result.AssignGridLayout(x => x.Title, 0, 0);
            result.AssignGridLayout(x => x.MetaKeywords, 1, 0);
            result.AssignGridLayout(x => x.IsEnabled, 0, 1);
            result.AssignGridLayout(x => x.MetaDescription, 1, 1);
            result.AssignGridLayout(x => x.BodyContent, 0, 2, 2);

            result.ExcludeProperty(x => x.Slug);
            result.AddHiddenValue("Slug", model.Slug);

            result.ExcludeProperty(x => x.Theme);
            result.AddHiddenValue("Theme", model.Theme);

            result.ExcludeProperty(x => x.CssClass);
            result.AddHiddenValue("CssClass", model.CssClass);

            result.ExcludeProperty(x => x.ShowOnMenuId);
            if (model.ShowOnMenuId.HasValue)
            {
                result.AddHiddenValue("ShowOnMenuId", model.ShowOnMenuId.Value.ToString());
            }

            if (showSaveAndContinue)
            {
                result.AddAction(true, true, false).HasText(T("Save & Continue")).HasName("SaveAndContinue").HasButtonStyle(ButtonStyle.Info);
            }

            // Page tags
            var tags = pageTagService.GetRecords();
            if (tags.Count > 0)
            {
                result.AddHiddenValue("RichtextCustomTags", JsonConvert.SerializeObject(tags.Select(x => new[] { x.Name, "[%" + x.Name + "%]" })));
            }

            return result;
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/pages/translations/{id}")]
        public ActionResult Translations(Guid id)
        {
            if (!CheckPermission(PagesPermissions.ManagePages))
            {
                return new HttpUnauthorizedResult();
            }

            var model = new TranslationModel { Id = id };

            var result = new ControlFormResult<TranslationModel>(model)
            {
                Title = T("Select Language").Text,
                UpdateActionName = "Translations",
                SubmitButtonText = T("OK"),
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            var languages = languageManager.GetActiveLanguages(Constants.ThemeDefault, false);
            result.RegisterExternalDataSource(x => x.CultureCode, languages.ToDictionary(k => k.CultureCode, v => v.Name));

            return result;
        }

        [HttpPost, FormButton("Save")]
        public ActionResult Translations(TranslationModel model)
        {
            if (!CheckPermission(PagesPermissions.ManagePages))
            {
                return new HttpUnauthorizedResult();
            }

            return new AjaxResult().Redirect(Url.Action("Translate", new { id = model.Id, cultureCode = model.CultureCode }), true);
        }

        [HttpPost]
        [ValidateInput(false)]
        [Url("{DashboardBaseUrl}/pages/page-preview")]
        public ActionResult Preview(PageModel page)
        {
            if (!CheckPermission(PagesPermissions.ManagePages))
            {
                return new HttpUnauthorizedResult();
            }

            var currentCulture = WorkContext.CurrentCulture;

            WorkContext.Breadcrumbs.Add(page.Title);

            if (!string.IsNullOrEmpty(page.Theme))
            {
                var themeManager = WorkContext.Resolve<IThemeManager>();
                var theme = themeManager.GetTheme(page.Theme);
                if (theme != null)
                {
                    WorkContext.CurrentTheme = theme;
                }
            }

            var bodyContent = string.IsNullOrEmpty(page.CssClass)
                ? string.Format("<article class=\"page-content\"><header><h1>{1}</h1></header><div class=\"article-content\">{0}</div></article>", page.BodyContent, page.Title)
                : string.Format("<article class=\"page-content {2}\"><header><h1>{1}</h1></header><div class=\"article-content\">{0}</div></article>", page.BodyContent, page.Title, page.CssClass);

            // Replace tags
            var tags = pageTagService.GetRecords();
            if (tags.Count > 0)
            {
                foreach (var tag in tags)
                {
                    bodyContent = bodyContent.Replace("[%" + tag.Name + "%]", tag.Content);
                }
            }

            return new ContentViewResult
            {
                Title = page.Title,
                MetaKeywords = page.MetaKeywords,
                MetaDescription = page.MetaDescription,
                BodyContent = bodyContent
            };
        }

        [ValidateInput(false), FormButton("Save")]
        [HttpPost, Url("{DashboardBaseUrl}/pages/update")]
        public ActionResult Update(PageModel model)
        {
            SaveAndContinue(model);

            return new AjaxResult().Redirect(Url.Action("Index"));
        }

        private ControlGridAjaxData<Page> GetPages(ControlGridFormRequest options)
        {
            int totals;
            var records = pageService.GetRecords(options, out totals, x => x.RefId == null);
            return new ControlGridAjaxData<Page>(records, totals);
        }

        private ControlGridAjaxData<HistoricPage> GetHistoricPages(ControlGridFormRequest options, int pageId)
        {
            int totals;
            var records = historicPageService.GetRecords(options, out totals, x => x.PageId == pageId);
            return new ControlGridAjaxData<HistoricPage>(records, totals);
        }

        [HttpPost, FormButton("RestoreVersion")]
        [ActionName("Update")]
        public ActionResult RestoreVersion(int id)
        {
            if (!CheckPermission(PagesPermissions.ManagePages))
            {
                return new HttpUnauthorizedResult();
            }

            var pageToRestore = historicPageService.GetById(id);

            var model = new PageModel
            {
                Id = pageToRestore.PageId,// <-- important
                BodyContent = pageToRestore.BodyContent,
                CssClass = pageToRestore.CssClass,
                CultureCode = pageToRestore.CultureCode,
                IsEnabled = pageToRestore.IsEnabled,
                MetaDescription = pageToRestore.MetaDescription,
                MetaKeywords = pageToRestore.MetaKeywords,
                RefId = pageToRestore.RefId,
                Slug = pageToRestore.Slug,
                Theme = pageToRestore.Theme,
                Title = pageToRestore.Title
            };

            // this will now archive the current version and restore the specified historic page to be the live page
            SaveAndContinue(model);

            return new AjaxResult().NotifyMessage("RESTORE_ENTITY_COMPLETE");
        }
    }
}