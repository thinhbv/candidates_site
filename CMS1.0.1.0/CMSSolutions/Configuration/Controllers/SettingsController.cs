using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions.Configuration.Services;
using CMSSolutions.ContentManagement.Dashboard.Models;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.Notify;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Configuration.Controllers
{
    [Themed(IsDashboard = true), Authorize]
    [Feature(Constants.Areas.Core)]
    [Url(BaseUrl = "{DashboardBaseUrl}/settings")]
    public class SettingsController : BaseController
    {
        private readonly ISettingService service;
        private readonly Lazy<IEnumerable<ISettings>> settings;
        private readonly INotifier notifier;

        public SettingsController(IWorkContextAccessor workContextAccessor, ISettingService service, Lazy<IEnumerable<ISettings>> settings, INotifier notifier)
            : base(workContextAccessor)
        {
            this.service = service;
            this.settings = settings;
            this.notifier = notifier;
        }

        [Url("{BaseUrl}")]
        public ActionResult Index()
        {
            if (!CheckPermission(Permissions.ManageSiteSettings))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Settings"));

            var controlGrid = new ControlGridFormResult<SettingsModel>
            {
                Title = T("Settings"),
                FetchAjaxSource = GetSettings,
                ActionsColumnWidth = 100,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            controlGrid.AddColumn(x => x.Name);
            controlGrid.AddRowAction()
                .HasUrl(x => Url.Action("Edit", new { id = x.Id }))
                .HasText(T("Edit"))
                .HasButtonSize(ButtonSize.ExtraSmall);

            return controlGrid;
        }

        private ControlGridAjaxData<SettingsModel> GetSettings(ControlGridFormRequest options)
        {
            var models = settings.Value.Where(x => !x.Hidden).Select(x => new SettingsModel
            {
                Id = x.GetType().FullName,
                Name = x.Name
            }).OrderBy(x => x.Name).ToList();
            return new ControlGridAjaxData<SettingsModel>(models);
        }

        [Url("{BaseUrl}/edit/{id}")]
        public ActionResult Edit(string id, string returnUrl)
        {
            if (!CheckPermission(Permissions.ManageSiteSettings))
            {
                return new HttpUnauthorizedResult();
            }

            var model = settings.Value.FirstOrDefault(x => x.GetType().FullName == id);
            if (model == null)
            {
                return HttpNotFound();
            }

            WorkContext.Breadcrumbs.Add(T("Settings"), Url.Action("Index", new { area = Constants.Areas.Core }));
            WorkContext.Breadcrumbs.Add(model.Name);
            WorkContext.Breadcrumbs.Add(T("Edit"));

            var value = service.GetSettings(model.GetType());

            var controlForm = new ControlFormResult<ISettings>(value, model.GetType())
            {
                Title = model.Name,
                UpdateActionName = "Save",
                CancelButtonUrl = string.IsNullOrEmpty(returnUrl) ? Url.Action("Index") : returnUrl,
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };
            controlForm.AddHiddenValue("Id", id);
            controlForm.AddHiddenValue("_ReturnUrl", returnUrl);

            model.OnEditing(controlForm, WorkContext);

            return controlForm;
        }

        [FormButton("Save")]
        [HttpPost, Url("{BaseUrl}/save")]
        public ActionResult Save(string id)
        {
            if (!CheckPermission(Permissions.ManageSiteSettings))
            {
                return new HttpUnauthorizedResult();
            }

            var model = settings.Value.FirstOrDefault(x => x.GetType().FullName == id);
            if (model == null)
            {
                return HttpNotFound();
            }

            TryUpdateModel(model, ControllerContext);

            service.SaveSetting(model);

            notifier.Add(NotifyType.Info, T(Constants.Messages.GenericSaveSuccess));

            //var returnUrl = Request.Form["_ReturnUrl"];
            //if (string.IsNullOrEmpty(returnUrl))
            //{
            //    returnUrl = Url.Action("Index");
            //}

            return new AjaxResult().Alert(T("Cập nhật thành công."));
        }

        private static void TryUpdateModel(ISettings model, ControllerContext controllerContext)
        {
            var binder = new DefaultModelBinder();
            var bindingContext = new ModelBindingContext
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, model.GetType()),
                ModelState = controllerContext.Controller.ViewData.ModelState,
                ValueProvider = controllerContext.Controller.ValueProvider
            };

            binder.BindModel(controllerContext, bindingContext);
        }
    }
}