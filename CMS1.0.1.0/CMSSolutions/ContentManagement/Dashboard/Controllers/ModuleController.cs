using System;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.Collections;
using CMSSolutions.ContentManagement.Dashboard.Services;
using CMSSolutions.ContentManagement.Pages;
using CMSSolutions.Environment.Descriptor.Models;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Environment.Extensions.Models;
using CMSSolutions.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Dashboard.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Dashboard)]
    public class ModuleController : BaseController
    {
        private readonly IExtensionManager extensionManager;
        private readonly ShellDescriptor shellDescriptor;

        public ModuleController(IWorkContextAccessor workContextAccessor, IExtensionManager extensionManager, ShellDescriptor shellDescriptor)
            : base(workContextAccessor)
        {
            this.extensionManager = extensionManager;
            this.shellDescriptor = shellDescriptor;
        }

        [Url("{DashboardBaseUrl}/modules/features")]
        public ActionResult Features()
        {
            if (!CheckPermission(DashboardPermissions.ManageModules))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Modules"));
            WorkContext.Breadcrumbs.Add(T("Features"));

            var features = extensionManager.AvailableFeatures().Where(x => !DefaultExtensionTypes.IsTheme(x.Extension.ExtensionType)).ToList();
            var groups = features.GroupBy(x => x.Category).OrderBy(x => x.Key);
            var htmlHelper = new HtmlHelper(new ViewContext(), new ViewDataContainer(ViewData));
            var sb = new StringBuilder();

            foreach (var @group in groups)
            {
                sb.AppendFormat("<h3>{0}</h3>", string.IsNullOrEmpty(group.Key) ? "Uncategorized" : group.Key);
                foreach (var descriptor in group.OrderBy(x => x.Name))
                {
                    if (descriptor.Name.Equals("CMS") || descriptor.Name.Equals("Security"))
                    {
                        continue;
                    }
                    var missingDependencies = descriptor.Dependencies.Where(d => !features.Any(f => f.Id.Equals(d, StringComparison.OrdinalIgnoreCase))).ToList();
                    var showDisable = @group.Key != "Core";
                    var showEnable = !missingDependencies.Any();
                    var enabled = shellDescriptor.Features.Any(x => x.Name == descriptor.Id);

                    sb.AppendFormat("<div class=\"{1}\"><div class=\"well well-small feature\" id=\"{0}-feature\">", descriptor.Id.Replace(".", "-").ToSlugUrl(), Constants.ContainerCssClassCol3);
                    sb.AppendFormat("<h4>{0}</h4>", descriptor.Name);

                    sb.Append("<div class=\"actions\">");

                    if (showDisable && enabled)
                    {
                        sb.AppendFormat("<form action=\"{0}\" method=\"post\">", Url.Action("Disable"));
                        sb.AppendFormat("<input type=\"hidden\" name=\"id\" value=\"{0}\" />", descriptor.Id);
                        sb.Append(htmlHelper.AntiForgeryToken());
                        sb.AppendFormat("<button type=\"submit\" class=\"btn btn-link btn-disable-feature\">{0}</button>", T("Disable"));
                        sb.Append("</form>");
                    }

                    if (showEnable && !enabled)
                    {
                        sb.AppendFormat("<form action=\"{0}\" method=\"post\">", Url.Action("Enable"));
                        sb.AppendFormat("<input type=\"hidden\" name=\"id\" value=\"{0}\" />", descriptor.Id);
                        sb.Append(htmlHelper.AntiForgeryToken());
                        sb.AppendFormat("<button type=\"submit\" class=\"btn btn-link btn-enable-feature\">{0}</button>", T("Enable"));
                        sb.Append("</form>");
                    }

                    sb.Append("</div>");

                    var dependencies = (from d in descriptor.Dependencies
                                        select (from f in features where f.Id.Equals(d, StringComparison.OrdinalIgnoreCase) select f).SingleOrDefault()).Where(f => f != null).OrderBy(f => f.Name);
                    if (!dependencies.IsNullOrEmpty())
                    {
                        sb.Append("<div class=\"dependencies\">");
                        sb.AppendFormat("<strong>{0}:</strong>", T("Depends on"));
                        sb.Append("<ul class=\"list-unstyled inline\">");

                        foreach (var dependency in dependencies)
                        {
                            sb.AppendFormat("<li><a href=\"#{1}-feature\">{0}</a></li>", dependency.Name, dependency.Id.Replace(".", "-").ToSlugUrl());
                        }

                        sb.Append("</ul>");
                        sb.Append("</div>");
                    }

                    if (!missingDependencies.IsNullOrEmpty())
                    {
                        sb.Append("<div class=\"missing-dependencies\">");
                        sb.AppendFormat("<strong>{0}:</strong>", T("Missing"));
                        sb.Append("<ul class=\"list-unstyled inline\">");

                        foreach (var dependency in missingDependencies)
                        {
                            sb.AppendFormat("<li>{0}</li>", dependency);
                        }

                        sb.Append("</ul>");
                        sb.Append("</div>");
                    }

                    sb.Append("</div></div>");
                }

                sb.Append("<div class='clearfix'></div>");
            }

            return new ControlContentResult(sb.ToString())
            {
                Title = T("Modules")
            };
        }

        [HttpPost]
        [Url("{DashboardBaseUrl}/modules/features/enable")]
        public ActionResult Enable(string id)
        {
            if (!CheckPermission(DashboardPermissions.ManageModules))
            {
                return new HttpUnauthorizedResult();
            }

            var moduleService = WorkContext.Resolve<IModuleService>();
            moduleService.EnableFeatures(new[] { id }, true);
            return RedirectToAction("Features");
        }

        [HttpPost]
        [Url("{DashboardBaseUrl}/modules/features/disable")]
        public ActionResult Disable(string id, bool? force)
        {
            if (!CheckPermission(DashboardPermissions.ManageModules))
            {
                return new HttpUnauthorizedResult();
            }

            if (id.Equals(Constants.Areas.Dashboard) || id.Equals(Constants.Areas.Accounts))
            {
                return new ControlContentResult("Ứng dụng mặc định không thể tắt.");
            }

            var moduleService = WorkContext.Resolve<IModuleService>();
            moduleService.DisableFeatures(new[] { id }, true);
            return RedirectToAction("Features");
        }
    }
}