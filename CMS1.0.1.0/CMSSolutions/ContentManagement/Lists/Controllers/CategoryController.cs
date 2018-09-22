using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.ContentManagement.Lists.Models;
using CMSSolutions.ContentManagement.Lists.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Routing;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Lists)]
    public class CategoryController : BaseController
    {
        private readonly IListCategoryService listCategoryService;
        private readonly IListService listService;

        public CategoryController(IWorkContextAccessor workContextAccessor, IListCategoryService listCategoryService, IListService listService)
            : base(workContextAccessor)
        {
            this.listCategoryService = listCategoryService;
            this.listService = listService;
        }

        [Url("{DashboardBaseUrl}/lists/category/{listId}")]
        public ActionResult Index(int listId)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var list = listService.GetById(listId);

            WorkContext.Breadcrumbs.Add(T("Manage Lists"), Url.Action("Index", "List", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(list.Name);
            WorkContext.Breadcrumbs.Add(T("Categories"));

            var result = new ControlGridFormResult<ListCategoryModel>
            {
                Title = T("Manage Categories").Text,
                UpdateActionName = "Update",
                FetchAjaxSource = options => GetCategories(options, listId),
                EnablePaginate = false,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            result.AddColumn(x => x.Name).RenderAsLink(x => x.Name, x => Url.Content("~/" + list.Url + "/" + x.FullUrl));
            result.AddColumn(x => x.Url).HasHeaderText("Slug");
            result.AddColumn(x => x.Position);

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
        [Url("{DashboardBaseUrl}/lists/category/create/{listId}")]
        public ActionResult Create(int listId)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var list = listService.GetById(listId);

            WorkContext.Breadcrumbs.Add(T("Manage Lists"), Url.Action("Index", "List", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(list.Name);
            WorkContext.Breadcrumbs.Add(T("Categories"), Url.Action("Index", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(T("Create"));

            var model = new ListCategoryModel
            {
                ListId = listId
            };

            var result = new ControlFormResult<ListCategoryModel>(model)
            {
                Title = T("Create Category").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            var dataSource = listCategoryService.GetCategories(listId).Select(x => new { Key = x.Id.ToString(), Value = FormatCategoryText(x.Name, x.ParentId, listCategoryService.GetCategories(listId), "\xA0\xA0\xA0\xA0\xA0") }).ToList();
            dataSource.Insert(0, new { Key = "", Value = "" });
            result.RegisterExternalDataSource(x => x.ParentId, dataSource.ToDictionary(x => x.Key, x => x.Value));

            return result;
        }

        [Themed(false)]
        [Url("{DashboardBaseUrl}/lists/category/edit/{id}")]
        public ActionResult Edit(int id)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var model = listCategoryService.GetById(id);
            var list = listService.GetById(model.ListId);

            WorkContext.Breadcrumbs.Add(T("Manage Lists"), Url.Action("Index", "List", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(list.Name);
            WorkContext.Breadcrumbs.Add(T("Categories"), Url.Action("Index", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(model.Name);
            WorkContext.Breadcrumbs.Add(T("Edit"));

            var result = new ControlFormResult<ListCategoryModel>(model)
            {
                Title = T("Edit Category").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            var dataSource = listCategoryService.GetCategories(model.ListId).Where(x => x.Id != id).Select(x => new { Key = x.Id.ToString(), Value = FormatCategoryText(x.Name, x.ParentId, listCategoryService.GetCategories(model.ListId), "\xA0\xA0\xA0\xA0\xA0") }).ToList();
            dataSource.Insert(0, new { Key = "", Value = "" });
            result.RegisterExternalDataSource(x => x.ParentId, dataSource.ToDictionary(x => x.Key, x => x.Value));

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

            var menu = listCategoryService.GetById(id);
            listCategoryService.Delete(menu);
            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE");
        }

        [ValidateInput(false), FormButton("Save")]
        [HttpPost, Url("{DashboardBaseUrl}/lists/category/update", Priority = 10)]
        public ActionResult Update(ListCategoryModel model)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            ListCategory item;
            if (model.ParentId.HasValue && model.ParentId.Value != 0)
            {
                item = listCategoryService.GetById(model.ParentId.Value);
            }
            else
            {
                item = null;
            }

            var category = model.Id != 0
                ? listCategoryService.GetById(model.Id)
                : new ListCategory();
            category.Name = model.Name;
            category.Url = model.Url;

            if (string.IsNullOrEmpty(category.Url))
            {
                category.Url = category.Name.ToSlugUrl();
            }

            if (item != null)
                category.FullUrl = item.FullUrl + "/" + category.Url;
            else
                category.FullUrl = category.Url;

            category.ParentId = model.ParentId;
            category.ListId = model.ListId;
            listCategoryService.Save(category);

            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").CloseModalDialog();
        }

        private ControlGridAjaxData<ListCategoryModel> GetCategories(ControlGridFormRequest options, int listId)
        {
            int totals;
            var records = listCategoryService.GetRecords(options, out totals, x => x.ListId == listId);
            var sorted = SortCategories(records);
            return new ControlGridAjaxData<ListCategoryModel>(sorted);
        }

        private static IEnumerable<ListCategoryModel> SortCategories(IList<ListCategory> categories)
        {
            var sortCategories = new List<ListCategoryModel>();

            foreach (var menuItem in categories.Where(x => x.ParentId == null).OrderBy(x => x.Position).ThenBy(x => x.Name))
            {
                sortCategories.Add(menuItem);
                SortCategories(menuItem.Id, categories, sortCategories, "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
            }

            return sortCategories;
        }

        private static void SortCategories(int parentId, IList<ListCategory> items, ICollection<ListCategoryModel> sortCategories, string prefix)
        {
            var childItems = items.Where(x => x.ParentId == parentId).OrderBy(x => x.Position).ThenBy(x => x.Name);
            foreach (ListCategoryModel childItem in childItems)
            {
                childItem.Name = prefix + childItem.Name;
                sortCategories.Add(childItem);
                SortCategories(childItem.Id, items, sortCategories, prefix + "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;");
            }
        }

        private static string FormatCategoryText(string text, int? parentId, IList<ListCategory> allItems, string space)
        {
            if (parentId.HasValue)
            {
                var parent = allItems.First(x => x.Id == parentId.Value);
                text = space + text;
                if (parent.ParentId.HasValue)
                {
                    return FormatCategoryText(text, parent.ParentId, allItems, space);
                }
                return text;
            }
            return text;
        }
    }
}