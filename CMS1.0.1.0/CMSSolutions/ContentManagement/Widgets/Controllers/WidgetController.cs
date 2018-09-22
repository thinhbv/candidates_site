using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using CMSSolutions.Collections;
using CMSSolutions.ContentManagement.Widgets.Domain;
using CMSSolutions.ContentManagement.Widgets.Models;
using CMSSolutions.ContentManagement.Widgets.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Serialization;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Routing;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Widgets.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Widgets)]
    public class WidgetController : BaseController
    {
        public WidgetController(IWorkContextAccessor workContextAccessor)
            : base(workContextAccessor)
        {
        }

        [Url("{DashboardBaseUrl}/widgets/{pageId?}", Priority = -5)]
        public ActionResult Index(int? pageId)
        {
            if (!CheckPermission(WidgetPermissions.ManageWidgets))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Widgets"));

            var result = new ControlGridFormResult<Widget>
            {
                Title = T("Manage Widgets").Text,
                UpdateActionName = "Update",
                FetchAjaxSource = o => GetWidgets(o, pageId),
                EnablePaginate = true,
                DefaultPageSize = WorkContext.DefaultPageSize,
                ActionsColumnWidth = 180,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            var zoneService = WorkContext.Resolve<IZoneService>();

            result.AddColumn(x => x.Title).HasWidth(300);
            result.AddColumn(x => x.WidgetName).HasHeaderText("Widget Type");
            result.AddColumn(x => x.ZoneId).HasHeaderText("Zone").RenderAsHtml(x =>
            {
                var zone = zoneService.GetById(x.ZoneId);
                return zone != null ? zone.Name : null;
            });
            result.AddColumn(x => x.Order);
            result.AddColumn(x => x.Enabled).AlignCenter().RenderAsStatusImage();

            result.AddAction()
                .HasText(T("Create"))
                .HasIconCssClass("cx-icon cx-icon-add")
                .HasUrl(Url.Action("Create", RouteData.Values.Merge(new { pageId })))
                .HasButtonStyle(ButtonStyle.Primary)
                .HasBoxButton(false)
                .HasCssClass(Constants.RowLeft)
                .ShowModalDialog();

            result.AddAction()
                .HasText(T("Zones"))
                .HasUrl(Url.Action("Index", "Zone"))
                .HasButtonStyle(ButtonStyle.Info)
                .HasBoxButton(false)
                .HasRow(false)
                .HasCssClass(Constants.RowLeft);

            result.AddRowAction(true)
                .HasText(T("On/Off"))
                .HasName("EnableOrDisableWidget")
                .HasValue(x => x.Id.ToString())
                .HasButtonSize(ButtonSize.ExtraSmall);

            result.AddRowAction()
                .HasText(T("Edit"))
                .HasUrl(x => Url.Action("Edit", RouteData.Values.Merge(new { id = x.Id }, "pageId")))
                .HasButtonSize(ButtonSize.ExtraSmall);

            result.AddRowAction(true)
                .HasText(T("Delete"))
                .HasName("Delete")
                .HasValue(x => x.Id.ToString())
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Danger)
                .HasConfirmMessage(T(Constants.Messages.ConfirmDeleteRecord).Text);

            result.AddReloadEvent("UPDATE_ENTITY_COMPLETE");
            result.AddReloadEvent("DELETE_ENTITY_COMPLETE");

            if (pageId.HasValue)
            {
                result.EnablePaginate = false;
                WorkContext.Breadcrumbs.Add(T("Manage Pages"), Url.Action("Index", "Page", new { area = Constants.Areas.Pages }));
            }

            return result;
        }

        [FormButton("EnableOrDisableWidget")]
        [HttpPost, ActionName("Update")]
        public ActionResult EnableOrDisable(int id)
        {
            if (!CheckPermission(WidgetPermissions.ManageWidgets))
            {
                return new HttpUnauthorizedResult();
            }

            var widgetService = WorkContext.Resolve<IWidgetService>();
            var widget = widgetService.GetById(id);
            widgetService.EnableOrDisable(widget);
            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE");
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/widgets/create/{pageId?}")]
        public ActionResult Create(int? pageId)
        {
            if (!CheckPermission(WidgetPermissions.ManageWidgets))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Widgets"), Url.Action("Index"));
            WorkContext.Breadcrumbs.Add(T("Create"));

            var model = new WidgetModel { Title = "", PageId = pageId };

            var result = new ControlFormResult<WidgetModel>(model)
            {
                Title = T("Create Widget").Text,
                UpdateActionName = "Update",
                ShowCloseButton = true,
                IsAjaxSupported = true,
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            result.ExcludeProperty(x => x.Enabled);

            var widgets = WorkContext.Resolve<IEnumerable<IWidget>>();

            var widgetTypes = widgets.Select(x => new { x.Name, Type = GetFullTypeName(x.GetType()) }).OrderBy(x => x.Name).ToDictionary(x => x.Type, x => x.Name);
            result.RegisterExternalDataSource(x => x.WidgetType, widgetTypes);

            var zoneService = WorkContext.Resolve<IZoneService>();
            var zones = zoneService.GetRecords().ToDictionary(x => x.Id, x => x.Name);
            result.RegisterExternalDataSource(x => x.ZoneId, zones);

            return result;
        }

        private static string GetFullTypeName(Type type)
        {
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }

        [Url("{DashboardBaseUrl}/widgets/edit/{id}")]
        public ActionResult Edit(int id)
        {
            if (!CheckPermission(WidgetPermissions.ManageWidgets))
            {
                return new HttpUnauthorizedResult();
            }

            var widgetService = WorkContext.Resolve<IWidgetService>();
            var records = widgetService.GetRecords(x => x.Id == id || x.RefId == id);
            var widgets = widgetService.GetWidgets(records);
            var widget = widgets.First(x => x.Id == id);
            var widgetType = widget.GetType();

            WorkContext.Breadcrumbs.Add(T("Widgets"), Url.Action("Index"));
            WorkContext.Breadcrumbs.Add(widget.Title);
            WorkContext.Breadcrumbs.Add(T("Edit"));

            var result = new ControlFormResult<IWidget>(widget, widgetType)
            {
                Title = T("Edit Widget").Text,
                UpdateActionName = "Update",
                Layout = ControlFormLayout.Tab,
                ShowCloseButton = true,
                IsAjaxSupported = false,
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            result.AddHiddenValue("WidgetType", GetFullTypeName(widget.GetType()));
            if (widget.PageId.HasValue)
            {
                result.AddHiddenValue("PageId", widget.PageId.Value.ToString());
            }

            var mainTab = result.AddTabbedLayout("Widget Settings");
            var mainGroup = mainTab.AddGroup();
            var allFields = widgetType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
            var controlAttributes = new Dictionary<string, ControlFormAttribute>();
            foreach (var propertyInfo in allFields)
            {
                var controlAttribute = propertyInfo.GetCustomAttribute<ControlFormAttribute>(true);
                if (controlAttribute == null) continue;
                mainGroup.Add(propertyInfo.Name);
                controlAttribute.PropertyInfo = propertyInfo;
                controlAttribute.PropertyType = propertyInfo.PropertyType;
                controlAttributes.Add(propertyInfo.Name, controlAttribute);
            }

            result.ExcludeProperty(x => x.Localized);
            if (!widget.HasTitle)
            {
                result.ExcludeProperty(x => x.ShowTitleOnPage);
            }

            var languageManager = WorkContext.Resolve<ILanguageManager>();
            var languages = languageManager.GetActiveLanguages(Constants.ThemeDefault, false);
            if (languages.Count > 1)
            {
                foreach (var language in languages)
                {
                    var languageTab = result.AddTabbedLayout(language.Name);
                    var languageGroup = languageTab.AddGroup();
                    var widgetForLanguage = widgets.FirstOrDefault(x => x.CultureCode == language.CultureCode) ??
                                            widget.ShallowCopy();

                    foreach (var controlAttribute in controlAttributes)
                    {
                        if (controlAttribute.Key == "Id")
                        {
                            continue;
                        }

                        var key = controlAttribute.Key + "." + language.CultureCode;
                        var value = controlAttribute.Value.PropertyInfo.GetValue(widgetForLanguage);
                        result.AddProperty(key, controlAttribute.Value.ShallowCopy(), value);
                        languageGroup.Add(key);
                    }

                    if (!widget.HasTitle)
                    {
                        result.ExcludeProperty("Title." + language.CultureCode);
                        result.ExcludeProperty("ShowTitleOnPage." + language.CultureCode);
                    }
                }
            }

            var zoneService = WorkContext.Resolve<IZoneService>();
            var zones = zoneService.GetRecords().ToDictionary(x => x.Id, x => x.Name);
            result.RegisterExternalDataSource(x => x.ZoneId, zones);

            return widget.BuildEditor(this, WorkContext, result);
        }

        [ValidateInput(false), FormButton("Save"), Transaction]
        [HttpPost, Url("{DashboardBaseUrl}/widgets/update")]
        public ActionResult Update(WidgetModel model)
        {
            if (!CheckPermission(WidgetPermissions.ManageWidgets))
            {
                return new HttpUnauthorizedResult();
            }

            var widgetService = WorkContext.Resolve<IWidgetService>();
            var widgetType = Type.GetType(model.WidgetType);
            var widgets = WorkContext.Resolve<IEnumerable<IWidget>>();
            var widget = widgets.First(x => x.GetType() == widgetType);

            var records = model.Id == 0 
                ? new List<Widget> {new Widget()} 
                : widgetService.GetRecords(x => x.Id == model.Id || x.RefId == model.Id);

            if (model.Id != 0)
            {
                var languageManager = WorkContext.Resolve<ILanguageManager>();
                var languages = languageManager.GetActiveLanguages(Constants.ThemeDefault, false);
                if (languages.Count > 1)
                {
                    foreach (var language in languages)
                    {
                        bool localized;
                        try
                        {
                            localized = Convert.ToBoolean(Request.Form["Localized." + language.CultureCode].Split(',')[0]);
                        }
                        catch
                        {
                            localized = false;
                        }

                        var languageRecord = records.FirstOrDefault(x => x.CultureCode == language.CultureCode);
                        if (localized)
                        {
                            if (languageRecord == null)
                            {
                                languageRecord = new Widget
                                {
                                    CultureCode = language.CultureCode,
                                    RefId = model.Id
                                };
                                records.Add(languageRecord);
                            }
                        }
                        else
                        {
                            if (languageRecord != null)
                            {
                                records.Remove(languageRecord);
                                widgetService.Delete(languageRecord);
                            }
                        }
                    }
                }
            }

            var settings = new SharpSerializerXmlSettings
            {
                IncludeAssemblyVersionInTypeName = false,
                IncludeCultureInTypeName = false,
                IncludePublicKeyTokenInTypeName = false
            };
            var sharpSerializer = new SharpSerializer(settings);

            foreach (var record in records)
            {
                record.WidgetName = widget.Name;
                record.WidgetType = model.WidgetType;

                if (model.PageId.HasValue)
                {
                    record.PageId = model.PageId;
                }

                if (string.IsNullOrEmpty(record.CultureCode))
                {
                    TryUpdateModel(widget, widgetType);

                    widget.Id = record.Id;
                    record.Enabled = widget.Enabled;
                    widget.OnSaving(WorkContext);

                    record.Title = widget.Title;
                    record.ZoneId = widget.ZoneId;
                    record.Order = widget.Order;
                    record.DisplayCondition = widget.DisplayCondition;
                    record.Enabled = widget.Enabled;
                    record.WidgetValues = sharpSerializer.Serialize(widget);
                }
                else
                {
                    var values = Request.Form.AllKeys.ToList().ToDictionary(key => key, key => Request.Form[key]);
                    var localizedValues = values.Keys.Where(key => key.Contains("." + record.CultureCode)).ToDictionary(key => key.Replace("." + record.CultureCode, ""), key => FixCheckboxValue(values[key]));
                    var nameValueCollection = localizedValues.ToNameValueCollection();
                    var valueProvider = new NameValueCollectionValueProvider(nameValueCollection, CultureInfo.InvariantCulture);

                    TryUpdateModel(widget, widgetType, valueProvider);

                    widget.RefId = model.Id;
                    widget.CultureCode = record.CultureCode;
                    widget.OnSaving(WorkContext);

                    record.Title = widget.Title;
                    record.ZoneId = widget.ZoneId;
                    record.Order = widget.Order;
                    record.DisplayCondition = widget.DisplayCondition;
                    record.Enabled = widget.Enabled;
                    record.WidgetValues = sharpSerializer.Serialize(widget);
                }

                widgetService.Save(record);
            }

            if (model.Id == 0)
            {
                return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").CloseModalDialog();
            }

            return new AjaxResult().Redirect(model.PageId.HasValue
                ? Url.Action("Index", new { pageId = model.PageId.Value })
                : Url.Action("Index"));
        }

        [HttpPost, ActionName("Update")]
        [FormButton("Delete")]
        public ActionResult Delete(int id)
        {
            if (!CheckPermission(WidgetPermissions.ManageWidgets))
            {
                return new HttpUnauthorizedResult();
            }

            var widgetService = WorkContext.Resolve<IWidgetService>();
            var widget = widgetService.GetById(id);
            widgetService.Delete(widget);
            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE");
        }

        [HttpPost, Url("{DashboardBaseUrl}/widgets/editor-callback")]
        public ActionResult EditorCallback(WidgetModel model)
        {
            if (!CheckPermission(WidgetPermissions.ManageWidgets))
            {
                return new HttpUnauthorizedResult();
            }

            var widgetService = WorkContext.Resolve<IWidgetService>();
            var record = widgetService.GetById(model.Id);
            if (record == null)
            {
                return null;
            }

            var widgetType = Type.GetType(record.WidgetType);
            var widgets = WorkContext.Resolve<IEnumerable<IWidget>>();
            return widgets.Where(widget => widget.GetType() == widgetType).Select(widget => widget.EditorCallback(this, WorkContext)).FirstOrDefault();
        }

        [HttpPost, Url("widgets/callback"), AllowAnonymous]
        public ActionResult DisplayCallback(int widgetId)
        {
            var widgetService = WorkContext.Resolve<IWidgetService>();
            var record = widgetService.GetById(widgetId);
            if (record == null)
            {
                return null;
            }

            var widget = widgetService.GetWidgets().FirstOrDefault(x => x.Id == widgetId);
            return widget == null ? null : widget.DisplayCallback(this, WorkContext);
        }

        private ControlGridAjaxData<Widget> GetWidgets(ControlGridFormRequest options, int? pageId)
        {
            var widgetService = WorkContext.Resolve<IWidgetService>();
            if (pageId.HasValue)
            {
                return new ControlGridAjaxData<Widget>(widgetService.GetRecords(x => x.PageId == pageId && x.RefId == null).OrderBy(x => x.ZoneId).ThenBy(x => x.Order));
            }

            int totals;
            var records = widgetService.GetRecords(options, out totals, x => x.PageId == null && x.RefId == null).OrderBy(x => x.ZoneId).ThenBy(x => x.Order);
            return new ControlGridAjaxData<Widget>(records, totals);
        }

        private static string FixCheckboxValue(string value)
        {
            return value == "true,false" ? "true" : value;
        }
    }
}