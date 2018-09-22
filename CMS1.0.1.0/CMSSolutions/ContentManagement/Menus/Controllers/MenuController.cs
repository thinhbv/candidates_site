using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Menus.Domain;
using CMSSolutions.ContentManagement.Menus.Models;
using CMSSolutions.ContentManagement.Menus.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Routing;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Menus.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Menus)]
    public class MenuController : BaseController
    {
        private readonly IMenuService menuService;
        private readonly IMenuItemService menuItemService;

        public MenuController(IWorkContextAccessor workContextAccessor, IMenuService menuService, IMenuItemService menuItemService)
            : base(workContextAccessor)
        {
            this.menuService = menuService;
            this.menuItemService = menuItemService;
        }

        #region Menu

        [Url("{DashboardBaseUrl}/menus")]
        public ActionResult Index()
        {
            if (!CheckPermission(MenusPermissions.ManageMenus))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Menus"));

            var result = new ControlGridFormResult<Menu>
            {
                Title = T("Manage Menus").Text,
                UpdateActionName = "Update",
                FetchAjaxSource = GetMenus,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            result.AddColumn(x => x.Name);
            result.AddColumn(x => x.IsMainMenu).HasHeaderText(T("Is Main Menu")).AlignCenter().RenderAsStatusImage();

            result.AddRowAction()
                .HasText(T("Menu Items"))
                .HasUrl(x => Url.Action("MenuItems", RouteData.Values.Merge(new { menuId = x.Id })))
                .HasButtonSize(ButtonSize.ExtraSmall);

            result.AddAction()
                .HasText(T("Create"))
                .HasIconCssClass("cx-icon cx-icon-add")
                .HasUrl(Url.Action("Create", RouteData.Values))
                .HasButtonStyle(ButtonStyle.Primary)
                .ShowModalDialog()
                .HasCssClass(Constants.RowLeft)
                .HasBoxButton(false);

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

            result.AddReloadEvent("UPDATE_MENU_COMPLETE");
            result.AddReloadEvent("DELETE_MENU_COMPLETE");

            return result;
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/menus/create")]
        public ActionResult Create()
        {
            if (!CheckPermission(MenusPermissions.ManageMenus))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Menus"), Url.Action("Index", new { area = Constants.Areas.Menus }));
            WorkContext.Breadcrumbs.Add(T("Create"));

            var model = new MenuModel();

            var result = new ControlFormResult<MenuModel>(model)
            {
                Title = T("Create Menu").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            return result;
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/menus/edit/{id}")]
        public ActionResult Edit(int id)
        {
            if (!CheckPermission(MenusPermissions.ManageMenus))
            {
                return new HttpUnauthorizedResult();
            }

            var model = menuService.GetById(id);

            return new ControlFormResult<MenuModel>(model)
            {
                Title = T("Edit Menu").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };
        }

        [FormButton("Delete")]
        [HttpPost, ActionName("Update")]
        public ActionResult Delete(int id)
        {
            if (!CheckPermission(MenusPermissions.ManageMenus))
            {
                return new HttpUnauthorizedResult();
            }

            var menu = menuService.GetById(id);
            menuService.Delete(menu);
            return new AjaxResult().NotifyMessage("DELETE_MENU_COMPLETE");
        }

        [ValidateInput(false), FormButton("Save")]
        [HttpPost, Url("{DashboardBaseUrl}/menus/update")]
        public ActionResult Update(MenuModel model)
        {
            if (!CheckPermission(MenusPermissions.ManageMenus))
            {
                return new HttpUnauthorizedResult();
            }

            var menu = model.Id == 0
                ? new Menu()
                : menuService.GetById(model.Id);
            menu.Name = model.Name;
            menu.IsMainMenu = model.IsMainMenu;
            menuService.Save(menu);

            return new AjaxResult().NotifyMessage("UPDATE_MENU_COMPLETE").CloseModalDialog();
        }

        private ControlGridAjaxData<Menu> GetMenus(ControlGridFormRequest options)
        {
            int totals;
            var records = menuService.GetRecords(options, out totals);
            return new ControlGridAjaxData<Menu>(records, totals);
        }

        #endregion Menu

        #region Menu Items

        [Url("{DashboardBaseUrl}/menu-items/{menuId}")]
        public ActionResult MenuItems(int menuId)
        {
            if (!CheckPermission(MenusPermissions.ManageMenus))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Menus"), Url.Action("Index", new { area = Constants.Areas.Menus }));
            WorkContext.Breadcrumbs.Add(T("Menu Items"));

            var result = new ControlGridFormResult<MenuItemModel>
            {
                Title = T("Manage Menu Items").Text,
                UpdateActionName = "UpdateItem",
                FetchAjaxSource = request => GetMenuItems(menuId),
                EnablePaginate = false,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            result.AddColumn(x => x.Text);
            result.AddColumn(x => x.Url).RenderAsLink(x => x.Url, x => x.IsExternalUrl ? x.Url : Url.Content("~/" + x.Url));
            result.AddColumn(x => x.Position);
            result.AddColumn(x => x.Enabled).AlignCenter().RenderAsStatusImage();

            result.AddRowAction(true)
                .HasText(T("On/Off"))
                .HasName("Enable")
                .HasValue(x => x.Id.ToString())
                .HasButtonSize(ButtonSize.ExtraSmall);

            result.AddAction()
                .HasText(T("Create"))
                .HasIconCssClass("cx-icon cx-icon-add")
                .HasUrl(Url.Action("CreateItem", RouteData.Values))
                .HasButtonStyle(ButtonStyle.Primary)
                .ShowModalDialog();

            result.AddRowAction()
                .HasText(T("Edit"))
                .HasUrl(x => Url.Action("EditItem", RouteData.Values.Merge(new { id = x.Id })))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .ShowModalDialog();

            result.AddRowAction(true)
                .HasText(T("Delete"))
                .HasName("Delete")
                .HasValue(x => x.Id.ToString())
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Danger)
                .HasConfirmMessage(T(Constants.Messages.ConfirmDeleteRecord).Text);

            result.AddReloadEvent("UPDATE_MENUITEM_COMPLETE");
            result.AddReloadEvent("DELETE_MENUITEM_COMPLETE");

            return result;
        }

        [FormButton("Enable")]
        [HttpPost, ActionName("UpdateItem")]
        public ActionResult EnableItem(int id)
        {
            if (!CheckPermission(MenusPermissions.ManageMenus))
            {
                return new HttpUnauthorizedResult();
            }

            var item = menuItemService.GetById(id);
            item.Enabled = !item.Enabled;
            menuItemService.Update(item);

            return new AjaxResult().NotifyMessage("UPDATE_MENUITEM_COMPLETE");
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/menu-items/create/{menuId}")]
        public ActionResult CreateItem(int menuId)
        {
            if (!CheckPermission(MenusPermissions.ManageMenus))
            {
                return new HttpUnauthorizedResult();
            }

            var model = new MenuItemModel
            {
                MenuId = menuId,
                Enabled = true
            };

            var result = new ControlFormResult<MenuItemModel>(model)
            {
                Title = T("Create Menu Item").Text,
                UpdateActionName = "UpdateItem",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            var menuItems = GetMenuItems(menuItemService.GetMenuItems(menuId));
            result.RegisterExternalDataSource(x => x.ParentId, menuItems.ToDictionary(x => x.Id, x => x.Text.Replace("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", "\xA0\xA0\xA0\xA0\xA0")));

            return result;
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/menu-item/edit/{id}")]
        public ActionResult EditItem(int id)
        {
            if (!CheckPermission(MenusPermissions.ManageMenus))
            {
                return new HttpUnauthorizedResult();
            }

            var menuItem = menuItemService.GetById(id);

            MenuItemModel model = menuItem;

            WorkContext.Breadcrumbs.Add(T("Menus"), Url.Action("Index", new { area = Constants.Areas.Menus }));
            WorkContext.Breadcrumbs.Add(T("Items"), Url.Action("MenuItems", new { area = Constants.Areas.Menus, menuId = menuItem.MenuId }));
            WorkContext.Breadcrumbs.Add(T("Edit"));

            if (string.IsNullOrEmpty(model.Url))
            {
                model.Url = "/";
            }

            var result = new ControlFormResult<MenuItemModel>(model)
            {
                Title = T("Edit Menu Item").Text,
                UpdateActionName = "UpdateItem",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            var items = menuItemService.GetMenuItems(model.MenuId);
            if (model.ParentId.HasValue)
            {
                items = items.Where(x => x.Id != model.Id && x.ParentId != model.ParentId.Value).ToList();
            }
            else
            {
                items = items.Where(x => x.Id != model.Id).ToList();
            }
            var menuItems = GetMenuItems(items);
            result.RegisterExternalDataSource(x => x.ParentId, menuItems.ToDictionary(x => x.Id, x => x.Text.Replace("&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;", "\xA0\xA0\xA0\xA0\xA0")));

            return result;
        }

        [FormButton("Delete")]
        [HttpPost, ActionName("UpdateItem")]
        public ActionResult DeleteItem(int id)
        {
            if (!CheckPermission(MenusPermissions.ManageMenus))
            {
                return new HttpUnauthorizedResult();
            }

            var menuItem = menuItemService.GetById(id);
            menuItemService.Delete(menuItem);
            return new AjaxResult().NotifyMessage("DELETE_MENUITEM_COMPLETE");
        }

        [ValidateInput(false), FormButton("Save")]
        [HttpPost, Url("{DashboardBaseUrl}/menu-item/update")]
        public ActionResult UpdateItem(MenuItemModel model)
        {
            if (!CheckPermission(MenusPermissions.ManageMenus))
            {
                return new HttpUnauthorizedResult();
            }

            var menuItem = model.Id == 0
                ? new MenuItem()
                : menuItemService.GetById(model.Id);

            menuItem.Text = model.Text;
            menuItem.Description = model.Description;
            menuItem.Url = model.Url.Trim(' ', '/', '~');
            menuItem.Position = model.Position;
            menuItem.ParentId = model.ParentId;
            menuItem.Enabled = model.Enabled;
            menuItem.CssClass = model.CssClass;
            menuItem.MenuId = model.MenuId;
            menuItem.IsExternalUrl = IsExternalUrl(menuItem.Url);
            menuItemService.Save(menuItem);

            return new AjaxResult().NotifyMessage("UPDATE_MENUITEM_COMPLETE").CloseModalDialog();
        }

        private static bool IsExternalUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
            {
                return false;
            }

            if (url.StartsWith("javascript:"))
            {
                return true;
            }

            return Uri.IsWellFormedUriString(url, UriKind.Absolute);
        }

        private ControlGridAjaxData<MenuItemModel> GetMenuItems(int menuId)
        {
            var menuItems = menuItemService.GetMenuItems(menuId);
            var sortedMenuItems = GetMenuItems(menuItems);

            return new ControlGridAjaxData<MenuItemModel>(sortedMenuItems);
        }

        private static IEnumerable<MenuItemModel> GetMenuItems(IList<MenuItem> menuItems)
        {
            var sortedMenuItems = new List<MenuItemModel>();

            foreach (var menuItem in menuItems.Where(x => x.ParentId == null).OrderBy(x => x.Position).ThenBy(x => x.Text))
            {
                sortedMenuItems.Add(menuItem);
                GetMenuItems(menuItem.Id, menuItems, sortedMenuItems, "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
            }

            return sortedMenuItems;
        }

        private static void GetMenuItems(int parentId, IList<MenuItem> items, ICollection<MenuItemModel> models, string prefix)
        {
            var childItems = items.Where(x => x.ParentId == parentId).OrderBy(x => x.Position).ThenBy(x => x.Text);
            foreach (MenuItemModel childItem in childItems)
            {
                childItem.Text = prefix + childItem.Text;
                models.Add(childItem);
                GetMenuItems(childItem.Id, items, models, prefix + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
            }
        }

        #endregion Menu Items
    }
}