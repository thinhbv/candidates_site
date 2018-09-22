using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Mvc;
using Castle.Core.Logging;
using CMSSolutions.Localization;
using CMSSolutions.Web.Mvc.ViewEngines.ThemeAwareness;
using CMSSolutions.Web.Security.Permissions;
using CMSSolutions.Web.UI.Notify;

namespace CMSSolutions.Web.Mvc
{
    public abstract class BaseController : Controller
    {
        public WorkContext WorkContext { get; set; }

        public Localizer T { get; set; }

        public ILogger Logger { get; set; }

        protected string ClientIPAddress
        {
            get
            {
                var ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (ipAddress == null || ipAddress.ToLower() == "unknown")
                    ipAddress = Request.ServerVariables["REMOTE_ADDR"];

                return ipAddress;
            }
        }

        public string TableName { get; set; }

        protected BaseController(IWorkContextAccessor workContextAccessor)
        {
            WorkContext = workContextAccessor.GetContext();
            T = NullLocalizer.Instance;
            Logger = NullLogger.Instance;
            TableName = "tblDefault";
        }

        #region Find View

        protected bool FindView(string viewName, object model, out ActionResult action)
        {
            var layoutAwareViewEngine = WorkContext.Resolve<ILayoutAwareViewEngine>();
            var engines = new ViewEngineCollection(new List<IViewEngine> { layoutAwareViewEngine });
            var result = engines.FindView(ControllerContext, viewName, null);
            if (result.View != null)
            {
                if (model != null)
                {
                    ViewData.Model = model;
                }

                action = new ViewResult
                {
                    ViewData = ViewData,
                    TempData = TempData,
                    View = result.View,
                    ViewName = viewName
                };
                return true;
            }
            action = null;
            return false;
        }

        #endregion Find View

        #region Authorize

        protected virtual bool CheckPermission(Permission permission, LocalizedString message = null)
        {
            var authorizationService = WorkContext.Resolve<IAuthorizationService>();
            if (authorizationService.TryCheckAccess(permission, WorkContext.CurrentUser))
            {
                return true;
            }

            if (message != null)
            {
                var notifier = WorkContext.Resolve<INotifier>();
                var user = WorkContext.CurrentUser;
                if (user == null)
                {
                    notifier.Error(T("{0}. Anonymous users do not have {1} permission.", message, permission.Name));
                }
                else
                {
                    notifier.Error(
                        T("{0}. Current user, {2}, does not have {1} permission.",
                        message, permission.Name, user.UserName));
                }
            }

            return false;
        }

        #endregion Authorize

        #region Update Model

        protected void TryUpdateModel(object model, Type modelType, IValueProvider valueProvider = null)
        {
            var binder = new DefaultModelBinder();
            var bindingContext = new ModelBindingContext
            {
                ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, modelType),
                ModelState = ViewData.ModelState,
                ValueProvider = valueProvider ?? ValueProvider //provider
            };

            binder.BindModel(ControllerContext, bindingContext);
        }

        #endregion Update Model

        public string RenderRazorViewToString(string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = System.Web.Mvc.ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                var viewContext = new ViewContext(ControllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(ControllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}