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
using ListItem = CMSSolutions.ContentManagement.Lists.Domain.ListItem;

namespace CMSSolutions.ContentManagement.Lists.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Lists)]
    public class ListItemController : BaseController
    {
        private readonly IListService listService;
        private readonly IListCategoryService listCategoryService;
        private readonly IListItemService listItemService;
        private readonly IListFieldService listFieldService;

        public ListItemController(IWorkContextAccessor workContextAccessor, IListItemService listItemService, IListFieldService listFieldService, IListService listService, IListCategoryService listCategoryService)
            : base(workContextAccessor)
        {
            this.listItemService = listItemService;
            this.listFieldService = listFieldService;
            this.listService = listService;
            this.listCategoryService = listCategoryService;
        }

        [Url("{DashboardBaseUrl}/lists/items/{listId}")]
        public ActionResult Index(int listId)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var list = listService.GetById(listId);

            var result = new ControlGridFormResult<ListItem>
            {
                Title = list.Name,
                UpdateActionName = "Update",
                FetchAjaxSource = options => GetListItems(options, listId),
                EnablePaginate = true,
                DefaultPageSize = WorkContext.DefaultPageSize,
                EnableSearch = true,
                ActionsColumnWidth = 200,
            };

            result.AddColumn(x => x.Title).EnableFilter();
            result.AddColumn(x => x.Slug).EnableFilter();
            result.AddColumn(x => x.Position).HasWidth(100);
            result.AddColumn(x => x.Enabled).EnableFilter().HasWidth(100).AlignCenter().RenderAsStatusImage();

            result.AddAction()
                .HasText(T("Create"))
                .HasIconCssClass("cx-icon cx-icon-add")
                .HasUrl(Url.Action("Create", RouteData.Values.Merge(new { listId })))
                .HasButtonStyle(ButtonStyle.Primary);

            result.AddAction()
                .HasText(T("Comments"))
                .HasIconCssClass("cx-icon cx-icon-comments")
                .HasUrl(Url.Action("Index", "Comment", RouteData.Values.Merge(new { listId })))
                .HasButtonStyle(ButtonStyle.Info);

            result.AddRowAction(true)
                .HasText(T("On/Off"))
                .HasName("EnableOrDisable")
                .HasValue(x => x.Id.ToString())
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Info);

            result.AddRowAction()
                .HasText(T("Edit"))
                .HasUrl(x => Url.Action("Edit", RouteData.Values.Merge(new { id = x.Id }, "listId")))
                .HasButtonSize(ButtonSize.ExtraSmall);

            result.AddRowAction()
                .HasText(T("Copy"))
                .HasUrl(x => Url.Action("Copy", RouteData.Values.Merge(new { id = x.Id }, "listId")))
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

            WorkContext.Breadcrumbs.Add(T("Manage Lists"), Url.Action("Index", "List", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(list.Name);

            return result;
        }

        [FormButton("EnableOrDisable")]
        [HttpPost, ActionName("Update")]
        public ActionResult EnableOrDisable(int id)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var item = listItemService.GetById(id);
            item.Enabled = !item.Enabled;
            listItemService.Save(item);
            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE");
        }

        [Url("{DashboardBaseUrl}/lists/items/create/{listId}")]
        public ActionResult Create(int listId)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var list = listService.GetById(listId);

            var result = new ControlFormResult
            {
                Title = T("Create List Item").Text,
                UpdateActionName = "Update",
                CssClass = "form-edit-list-item",
            };

            result.AddProperty("Id", new ControlHiddenAttribute(), 0);
            result.AddProperty("ListId", new ControlHiddenAttribute(), listId);
            result.AddProperty("Title", new ControlTextAttribute
            {
                Required = true,
                MaxLength = 255,
                LabelText = "Title",
                ContainerCssClass = "col-md-6",
                ContainerRowIndex = 0
            }, string.Empty);

            result.AddProperty("Slug", new ControlSlugAttribute
            {
                MaxLength = 255,
                HelpText = string.Format(T("List item url will become {0}/{{slug}}").Text, list.Url),
                LabelText = "Slug",
                ContainerCssClass = "col-md-6",
                ContainerRowIndex = 0
            }, string.Empty);

            result.AddProperty("PictureUrl", new ControlFileUploadAttribute
            {
                EnableFineUploader = true,
                LabelText = "Picture Url",
                UploadFolder = list.Name,
                ContainerCssClass = "col-md-6",
                ContainerRowIndex = 1
            }, string.Empty);

            var categories = listCategoryService.GetCategories(listId);
            var selectList = new List<SelectListItem>();
            var rootCategories = categories.Where(x => x.ParentId == null).OrderBy(x => x.Position).ThenBy(x => x.Name);
            GetCategoriesSelectList(rootCategories, selectList, categories, "");

            result.AddProperty("Categories", new ControlChoiceAttribute(ControlChoice.DropDownList)
            {
                LabelText = "Categories",
                SelectListItems = selectList,
                PropertyType = typeof(string),
                EnableChosen = true,
                AllowMultiple = true,
                ContainerCssClass = "col-md-6",
                ContainerRowIndex = 1
            }, new Guid[] { });

            if (list.EnabledMetaTags)
            {
                result.AddProperty("MetaKeywords", new ControlTextAttribute
                {
                    MaxLength = 255,
                    LabelText = "Meta Keywords",
                    ContainerCssClass = "col-md-6",
                    ContainerRowIndex = 2
                }, string.Empty);

                result.AddProperty("MetaDescription", new ControlTextAttribute
                {
                    MaxLength = 255,
                    LabelText = "Meta Description",
                    ContainerCssClass = "col-md-6",
                    ContainerRowIndex = 2
                }, string.Empty);
            }

            result.AddProperty("Position", new ControlNumericAttribute
            {
                Required = true,
                PropertyType = typeof(int),
                LabelText = "Position",
                ContainerCssClass = "col-md-1",
                ContainerRowIndex = 3
            }, 0);

            result.AddProperty("Enabled", new ControlChoiceAttribute(ControlChoice.CheckBox)
            {
                PropertyType = typeof(bool),
                LabelText = "",
                AppendText = "Enabled",
                ContainerCssClass = "col-md-2",
                ContainerRowIndex = 3
            }, true);

            var fields = listFieldService.GetFields(listId);
            foreach (var field in fields)
            {
                field.BindControlField(result, string.Empty);
            }

            return result;
        }

        [Url("{DashboardBaseUrl}/lists/items/copy/{id}")]
        public ActionResult Copy(int id)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var result = (ControlFormResult)Edit(id);
            result.AddProperty("Id", new ControlHiddenAttribute(), 0);
            result.Title = T("Copy List Item").Text;
            ViewData.ModelState["id"] = new ModelState { Value = new ValueProviderResult(0, "0", null) };
            return result;
        }

        [Url("{DashboardBaseUrl}/lists/items/edit/{id}")]
        public ActionResult Edit(int id)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var listItem = listItemService.GetById(id);
            var list = listService.GetById(listItem.ListId);

            var result = new ControlFormResult
            {
                Title = T("Edit List Item").Text,
                UpdateActionName = "Update",
                CssClass = "form-edit-list-item"
            };

            result.AddProperty("Id", new ControlHiddenAttribute(), listItem.Id);
            result.AddProperty("ListId", new ControlHiddenAttribute(), listItem.ListId);
            result.AddProperty("Title", new ControlTextAttribute
            {
                LabelText = "Title",
                Required = true,
                MaxLength = 255,
                ContainerCssClass = "col-md-6",
                ContainerRowIndex = 0
            }, listItem.Title);

            result.AddProperty("Slug", new ControlSlugAttribute
            {
                LabelText = "Slug",
                Required = true,
                MaxLength = 255,
                HelpText = string.Format(T("List item url will become {0}/{{slug}}").Text, list.Url),
                ContainerCssClass = "col-md-6",
                ContainerRowIndex = 0
            }, listItem.Slug);

            result.AddProperty("PictureUrl", new ControlFileUploadAttribute
            {
                EnableFineUploader = true,
                LabelText = "Picture Url",
                UploadFolder = list.Name,
                ContainerCssClass = "col-md-6",
                ContainerRowIndex = 1
            }, listItem.PictureUrl);

            var categories = listCategoryService.GetCategories(listItem.ListId);
            var selectLists = new List<SelectListItem>();
            var rootCategories = categories.Where(x => x.ParentId == null).OrderBy(x => x.Position).ThenBy(x => x.Name);
            GetCategoriesSelectList(rootCategories, selectLists, categories, "");

            var itemCategories = listCategoryService.GetItemCategories(id);

            result.AddProperty("Categories", new ControlChoiceAttribute(ControlChoice.DropDownList)
            {
                LabelText = "Categories",
                SelectListItems = selectLists,
                PropertyType = typeof(string),
                EnableChosen = true,
                AllowMultiple = true,
                ContainerCssClass = "col-md-6",
                ContainerRowIndex = 1
            }, itemCategories);

            if (list.EnabledMetaTags)
            {
                result.AddProperty("MetaKeywords", new ControlTextAttribute
                {
                    MaxLength = 255,
                    LabelText = "Meta Keywords",
                    ContainerCssClass = "col-md-6",
                    ContainerRowIndex = 2
                }, listItem.MetaKeywords);
                result.AddProperty("MetaDescription", new ControlTextAttribute
                {
                    MaxLength = 255,
                    LabelText = "Meta Description",
                    ContainerCssClass = "col-md-6",
                    ContainerRowIndex = 2
                }, listItem.MetaDescription);
            }

            result.AddProperty("Position", new ControlNumericAttribute
            {
                LabelText = "Position",
                Required = true,
                PropertyType = typeof(int),
                ContainerCssClass = "col-md-1",
                ContainerRowIndex = 3
            }, listItem.Position);

            result.AddProperty("Enabled", new ControlChoiceAttribute(ControlChoice.CheckBox)
            {
                LabelText = "",
                AppendText = "Enabled",
                PropertyType = typeof(bool),
                ContainerCssClass = "col-md-1",
                ContainerRowIndex = 3
            }, listItem.Enabled);

            var fields = listFieldService.GetFields(listItem.ListId);

            IDictionary<string, object> values;

            if (!string.IsNullOrEmpty(listItem.Values))
            {
                try
                {
                    values = listItem.Values.SharpDeserialize<IDictionary<string, object>>();
                }
                catch
                {
                    values = new Dictionary<string, object>();
                }
            }
            else
            {
                values = new Dictionary<string, object>();
            }

            foreach (var field in fields)
            {
                var value = values.ContainsKey(field.Name) ? values[field.Name] : null;
                field.BindControlField(result, value);
            }

            WorkContext.Breadcrumbs.Add(T("Manage Lists"), Url.Action("Index", "List"));
            WorkContext.Breadcrumbs.Add(list.Name, Url.Action("Index", "ListItem", new { listId = list.Id }));

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

            var listItem = listItemService.GetById(id);
            listItemService.Delete(listItem);
            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE");
        }

        [ValidateInput(false), FormButton("Save")]
        [HttpPost, Url("{DashboardBaseUrl}/lists/items/update", Priority = 10)]
        public ActionResult Update(ListItemModel model)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var listItem = model.Id != 0
                ? listItemService.GetById(model.Id)
                : new ListItem();
            listItem.Title = model.Title;
            listItem.PictureUrl = model.PictureUrl;
            listItem.Position = model.Position;
            listItem.Enabled = model.Enabled;
            listItem.MetaKeywords = model.MetaKeywords;
            listItem.MetaDescription = model.MetaDescription;

            if (!string.IsNullOrEmpty(model.Slug))
            {
                listItem.Slug = model.Slug.Trim(' ', '/', '.', '~');
            }

            if (string.IsNullOrEmpty(listItem.Slug))
            {
                listItem.Slug = model.Title.ToSlugUrl();
            }

            if (model.Id == 0)
            {
                listItem.CreatedDate = DateTime.UtcNow;
                listItem.ModifiedDate = listItem.CreatedDate;
                listItem.ListId = model.ListId;
            }
            else
            {
                listItem.ModifiedDate = DateTime.UtcNow;
            }

            var fields = listFieldService.GetFields(listItem.ListId);

            var values = fields.ToDictionary(field => field.Name, field => field.GetControlFormValue(this, WorkContext));

            listItem.Values = values.SharpSerialize();

            listItemService.Save(listItem);

            // Save categories
            if (model.Id != 0)
            {
                listItemService.RemoveCategories(model.Id);
            }

            if (model.Categories != null && model.Categories.Length > 0)
            {
                listItemService.AddCategories(listItem.Id, model.Categories);
            }

            return new AjaxResult().Redirect(Url.Action("Index", "ListItem", new { listId = model.ListId }));
        }

        private ControlGridAjaxData<ListItem> GetListItems(ControlGridFormRequest options, int listId)
        {
            int totals;
            var records = listItemService.GetRecords(options, out totals, x => x.ListId == listId);
            return new ControlGridAjaxData<ListItem>(records, totals);
        }

        private static void GetCategoriesSelectList(IEnumerable<ListCategory> categories, ICollection<SelectListItem> selectList, IList<ListCategory> allCategories, string prefix)
        {
            foreach (var category in categories)
            {
                selectList.Add(new SelectListItem
                                   {
                                       Text = prefix + category.Name,
                                       Value = category.Id.ToString()
                                   });
                var categoryId = category.Id;
                var childCategories = allCategories.Where(x => x.ParentId == categoryId).OrderBy(x => x.Position).ThenBy(x => x.Name).ToList();
                if (childCategories.Count > 0)
                {
                    GetCategoriesSelectList(childCategories, selectList, allCategories, prefix + "\xA0\xA0\xA0\xA0\xA0");
                }
            }
        }
    }
}