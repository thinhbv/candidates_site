using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.ContentManagement.Lists.Fields;
using CMSSolutions.ContentManagement.Lists.Models;
using CMSSolutions.ContentManagement.Lists.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Serialization;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Routing;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Lists)]
    public class FieldController : BaseController
    {
        private readonly IListFieldService listFieldService;
        private readonly IListService listService;

        public FieldController(IWorkContextAccessor workContextAccessor, IListFieldService listFieldService, IListService listService)
            : base(workContextAccessor)
        {
            this.listFieldService = listFieldService;
            this.listService = listService;
        }

        [Url("{DashboardBaseUrl}/lists/fields/{listId}")]
        public ActionResult Index(int listId)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var list = listService.GetById(listId);
            WorkContext.Breadcrumbs.Add(T("Manage Lists"), Url.Action("Index", "List", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(list.Name);
            WorkContext.Breadcrumbs.Add(T("Fields"));

            var result = new ControlGridFormResult<ListField>
            {
                Title = T("Manage Fields").Text,
                UpdateActionName = "Update",
                FetchAjaxSource = options => GetFields(options, listId),
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            result.AddColumn(x => x.Title);
            result.AddColumn(x => x.Name);
            result.AddColumn(x => x.FieldType).HasHeaderText("Field Type");
            result.AddColumn(x => x.Position);
            result.AddColumn(x => x.Required).AlignCenter().RenderAsStatusImage(false, true);

            result.AddAction()
                .HasText(T("Create"))
                .HasIconCssClass("cx-icon cx-icon-add")
                .HasUrl(Url.Action("Create", RouteData.Values.Merge(new { listId })))
                .HasButtonStyle(ButtonStyle.Primary)
                .ShowModalDialog();

            result.AddRowAction()
                .HasText(T("Edit"))
                .HasUrl(x => Url.Action("Edit", RouteData.Values.Merge(new { id = x.Id }, "listId")))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .ShowModalDialog();

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

        [Themed(false)]
        [Url("{DashboardBaseUrl}/list/fields/create/{listId}")]
        public ActionResult Create(int listId)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var list = listService.GetById(listId);
            WorkContext.Breadcrumbs.Add(T("Manage Lists"), Url.Action("Index", "List", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(list.Name);
            WorkContext.Breadcrumbs.Add(T("Fields"), Url.Action("Index", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(T("Create"));

            var model = new ListFieldModel { Title = "", ListId = listId };

            var result = new ControlFormResult<ListFieldModel>(model)
            {
                Title = T("Create Field").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            var listFields = WorkContext.Resolve<IEnumerable<IListField>>();
            var fieldTypes = listFields.Select(x => new { x.FieldType, Type = GetFullTypeName(x.GetType()) }).OrderBy(x => x.FieldType).ToDictionary(x => x.Type, x => x.FieldType);
            result.RegisterExternalDataSource(x => x.FieldType, fieldTypes);

            return result;
        }

        private static string GetFullTypeName(Type type)
        {
            return type.FullName + ", " + type.Assembly.GetName().Name;
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/lists/fields/edit/{id}")]
        public ActionResult Edit(int id)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var record = listFieldService.GetById(id);
            var list = record.List ?? listService.GetById(record.ListId);

            WorkContext.Breadcrumbs.Add(T("Manage Lists"), Url.Action("Index", "List", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(list.Name);
            WorkContext.Breadcrumbs.Add(T("Fields"), Url.Action("Index", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(record.Name);
            WorkContext.Breadcrumbs.Add(T("Edit"));

            var settings = new SharpSerializerXmlSettings
            {
                IncludeAssemblyVersionInTypeName = false,
                IncludeCultureInTypeName = false,
                IncludePublicKeyTokenInTypeName = false
            };

            var sharpSerializer = new SharpSerializer(settings);

            var field = (IListField)sharpSerializer.DeserializeFromString(record.FieldProperties);

            if (field.Id == 0)
            {
                field.Id = id;
            }

            var result = new ControlFormResult<IListField>(field, field.GetType())
            {
                Title = T("Edit Field").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            result.ExcludeProperty(x => x.Name);

            result.AddHiddenValue("Name", record.Name);
            result.AddHiddenValue("FieldType", GetFullTypeName(field.GetType()));

            return result;
        }

        [FormButton("Delete")]
        [HttpPost, ActionName("Update")]
        public ActionResult Delete(int id)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var field = listFieldService.GetById(id);
            listFieldService.Delete(field);
            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE");
        }

        [ValidateInput(false), FormButton("Save")]
        [HttpPost, Url("{DashboardBaseUrl}/lists/fields/update", Priority = 10)]
        public ActionResult Update(ListFieldModel model)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var record = model.Id != 0 ? listFieldService.GetById(model.Id) : new ListField();
            record.Title = model.Title.Trim();
            record.Name = model.Name.Trim();
            record.ListId = model.ListId;
            record.Position = model.Position;
            record.Required = model.Required;

            // Validate unique name
            if (model.Id == 0)
            {
                var name = Regex.Replace(record.Name, "[^0-9a-zA-Z]+", "");
                if (!model.Name.Equals(name, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new AjaxResult().Alert(T("The field name contains invalid characters, please make sure field name can have only a-z and 0-9 characters."));
                }

                var otherField = listFieldService.GetRecord(x => x.ListId == record.ListId && x.Name == record.Name);
                if (otherField != null || record.Name.Equals("Title", StringComparison.InvariantCultureIgnoreCase))
                {
                    return new AjaxResult().Alert(T("Have other field with same name, please make sure unique name for field."));
                }
            }

            var fieldType = Type.GetType(model.FieldType);
            var fields = WorkContext.Resolve<IEnumerable<IListField>>();
            var field = fields.First(x => x.GetType() == fieldType);

            TryUpdateModel(field, fieldType);

            var settings = new SharpSerializerXmlSettings
            {
                IncludeAssemblyVersionInTypeName = false,
                IncludeCultureInTypeName = false,
                IncludePublicKeyTokenInTypeName = false
            };

            var sharpSerializer = new SharpSerializer(settings);
            record.FieldProperties = sharpSerializer.Serialize(field);
            record.FieldType = field.FieldType;

            listFieldService.Save(record);

            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").CloseModalDialog();
        }

        private ControlGridAjaxData<ListField> GetFields(ControlGridFormRequest options, int listId)
        {
            int totals;
            var records = listFieldService.GetRecords(options, out totals, x => x.ListId == listId);
            return new ControlGridAjaxData<ListField>(records, totals);
        }
    }
}