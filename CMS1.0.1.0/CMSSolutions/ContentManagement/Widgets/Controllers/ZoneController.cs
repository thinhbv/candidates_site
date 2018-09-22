using System;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Widgets.Domain;
using CMSSolutions.ContentManagement.Widgets.Models;
using CMSSolutions.ContentManagement.Widgets.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Routing;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Widgets.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Widgets)]
    public class ZoneController : BaseController
    {
        private readonly IZoneService zoneService;

        public ZoneController(IWorkContextAccessor workContextAccessor, IZoneService zoneService)
            : base(workContextAccessor)
        {
            this.zoneService = zoneService;
        }

        [Url("{DashboardBaseUrl}/widgets/zones")]
        public ActionResult Index()
        {
            if (!CheckPermission(WidgetPermissions.ManageWidgets))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Widgets"), Url.Action("Index", "Widget"));
            WorkContext.Breadcrumbs.Add(T("Zones"));

            var result = new ControlGridFormResult<Zone>
            {
                Title = T("Manage Zones").Text,
                UpdateActionName = "Update",
                FetchAjaxSource = GetZones,
                EnablePaginate = false,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            result.AddColumn(x => x.Name);

            result.AddAction()
                .HasText(T("Create"))
                .HasIconCssClass("cx-icon cx-icon-add")
                .HasUrl(Url.Action("Create", RouteData.Values))
                .HasButtonStyle(ButtonStyle.Primary)
                .HasBoxButton(false)
                .HasCssClass(Constants.RowLeft)
                .ShowModalDialog();

            result.AddAction()
                .HasText(T("Widgets"))
                .HasUrl(Url.Action("Index", "Widget"))
                .HasButtonStyle(ButtonStyle.Default)
                .HasBoxButton(false)
                .HasRow(false)
                .HasCssClass(Constants.RowLeft);

            result.AddRowAction()
                .HasText(T("Edit"))
                .HasUrl(x => Url.Action("Edit", RouteData.Values.Merge(new { id = x.Id })))
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
        [Url("{DashboardBaseUrl}/widgets/zones/create")]
        public ActionResult Create()
        {
            if (!CheckPermission(WidgetPermissions.ManageWidgets))
            {
                return new HttpUnauthorizedResult();
            }

            var model = new ZoneModel();

            var result = new ControlFormResult<ZoneModel>(model)
            {
                Title = T("Create Zone").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            return result;
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/widgets/zones/edit/{id}")]
        public ActionResult Edit(int id)
        {
            if (!CheckPermission(WidgetPermissions.ManageWidgets))
            {
                return new HttpUnauthorizedResult();
            }

            var model = zoneService.GetById(id);

            var result = new ControlFormResult<ZoneModel>(model)
            {
                Title = T("Edit Zone").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            return result;
        }

        [FormButton("Save")]
        [HttpPost, Url("{DashboardBaseUrl}/widgets/zones/update")]
        public ActionResult Update(ZoneModel model)
        {
            if (!CheckPermission(WidgetPermissions.ManageWidgets))
            {
                return new HttpUnauthorizedResult();
            }

            var record = model.Id != 0
                ? zoneService.GetById(model.Id)
                : new Zone();
            record.Name = model.Name;
            zoneService.Save(record);

            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").CloseModalDialog();
        }

        [FormButton("Delete")]
        [HttpPost, ActionName("Update")]
        public ActionResult Delete(int id)
        {
            if (!CheckPermission(WidgetPermissions.ManageWidgets))
            {
                return new HttpUnauthorizedResult();
            }

            var zone = zoneService.GetById(id);
            zoneService.Delete(zone);
            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE");
        }

        private ControlGridAjaxData<Zone> GetZones(ControlGridFormRequest options)
        {
            var records = zoneService.GetRecords();
            return new ControlGridAjaxData<Zone>(records);
        }
    }
}