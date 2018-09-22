using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions.Collections.Generic;
using CMSSolutions.ContentManagement.Menus.Services;
using CMSSolutions.Environment;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc.Filters;

namespace CMSSolutions.ContentManagement.Menus
{
    [Feature(Constants.Areas.Menus)]
    public class BreadcrumbFilter : FilterProvider, IResultFilter
    {
        private readonly IWorkContextAccessor workContextAccessor;
        private readonly IMenuService menuService;
        private readonly IMenuItemService menuItemService;
        private readonly UrlHelper urlHelper;
        private readonly ShellSettings shellSettings;

        public BreadcrumbFilter(IWorkContextAccessor workContextAccessor,
            IMenuItemService menuItemService,
            IMenuService menuService,
            UrlHelper urlHelper,
            ShellSettings shellSettings)
        {
            this.workContextAccessor = workContextAccessor;
            this.menuItemService = menuItemService;
            this.menuService = menuService;
            this.urlHelper = urlHelper;
            this.shellSettings = shellSettings;
        }

        #region Implementation of IResultFilter

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            // should only run on a full view rendering result
            if (!(filterContext.Result is ViewResult))
            {
                return;
            }

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                return;
            }

            var workContext = workContextAccessor.GetContext();
            if (workContext == null)
            {
                return;
            }

            var menu = menuService.GetMainMenu();
            if (menu == null)
            {
                return;
            }

            // ReSharper disable PossibleNullReferenceException
            var slug = filterContext.HttpContext.Request.Url.LocalPath.Trim('/');
            // ReSharper restore PossibleNullReferenceException

            if (!string.IsNullOrEmpty(shellSettings.RequestUrlPrefix) && slug.StartsWith(shellSettings.RequestUrlPrefix))
            {
                slug = slug.Substring(shellSettings.RequestUrlPrefix.Length);
            }

            var items = menuItemService.GetMenuItems(menu.Id);

            var item = items.FirstOrDefault(x => x.Url == slug);
            if (item == null || item.ParentId == null)
            {
                return;
            }

            var parents = new List<Triple<string, string, bool>>();

            var parent = items.FirstOrDefault(x => x.Id == item.ParentId.Value);
            while (parent != null)
            {
                parents.Add(new Triple<string, string, bool>(parent.Text, parent.Url, parent.IsExternalUrl));
                parent = parent.ParentId.HasValue ? items.FirstOrDefault(x => x.Id == parent.ParentId.Value) : null;
            }

            foreach (var triple in parents)
            {
                workContext.Breadcrumbs.Insert(0, triple.First, triple.Third ? triple.Second : urlHelper.Content("~/" + triple.Second));
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }

        #endregion Implementation of IResultFilter
    }
}