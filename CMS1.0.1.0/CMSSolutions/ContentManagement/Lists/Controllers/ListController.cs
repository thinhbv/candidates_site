using System;
using System.Collections.Generic;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.ContentManagement.Lists.Models;
using CMSSolutions.ContentManagement.Lists.Routing;
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
    public class ListController : BaseController
    {
        private readonly IListService listService;
        private readonly IListPathConstraint listPathConstraint;

        public ListController(IWorkContextAccessor workContextAccessor, IListService listService, IListPathConstraint listPathConstraint)
            : base(workContextAccessor)
        {
            this.listService = listService;
            this.listPathConstraint = listPathConstraint;
        }

        [Url("{DashboardBaseUrl}/lists")]
        public ActionResult Index()
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Manage Lists"));

            var result = new ControlGridFormResult<List>
            {
                Title = T("Manage Lists").Text,
                UpdateActionName = "Update",
                FetchAjaxSource = GetLists,
                ActionsColumnWidth = 350,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            result.AddColumn(x => x.Name).RenderAsLink(x => x.Name, x => Url.Content("~/" + x.Url));
            result.AddColumn(x => x.Url);
            result.AddColumn(x => x.Enabled).AlignCenter().RenderAsStatusImage();

            var hasManageListsPermission = CheckPermission(ListsPermissions.ManageLists);

            if (hasManageListsPermission)
            {
                result.AddAction()
                    .HasText(T("Create"))
                    .HasIconCssClass("cx-icon cx-icon-add")
                    .HasUrl(Url.Action("Create", RouteData.Values))
                    .HasButtonStyle(ButtonStyle.Primary)
                    .HasCssClass(Constants.RowLeft)
                    .HasBoxButton(false);
            }

            result.AddRowAction()
                .HasText(T("Items"))
                .HasUrl(x => Url.Action("Index", "ListItem", RouteData.Values.Merge(new { listId = x.Id })))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .HasButtonStyle(ButtonStyle.Primary);

            if (hasManageListsPermission)
            {
                result.AddRowAction()
                    .HasText(T("Fields"))
                    .HasUrl(x => Url.Action("Index", "Field", RouteData.Values.Merge(new { listId = x.Id })))
                    .HasButtonSize(ButtonSize.ExtraSmall);
            }

            result.AddRowAction()
                .HasText(T("Categories"))
                .HasUrl(x => Url.Action("Index", "Category", RouteData.Values.Merge(new { listId = x.Id })))
                .HasButtonSize(ButtonSize.ExtraSmall);

            result.AddRowAction(true)
                .HasText(T("On/Off"))
                .HasName("EnableOrDisable")
                .HasValue(x => x.Id.ToString())
                .HasButtonSize(ButtonSize.ExtraSmall);

            if (hasManageListsPermission)
            {
                result.AddRowAction()
                .HasText(T("Edit"))
                .HasUrl(x => Url.Action("Edit", RouteData.Values.Merge(new { id = x.Id })))
                .HasButtonSize(ButtonSize.ExtraSmall);

                result.AddRowAction(true)
                    .HasText(T("Delete"))
                    .HasName("Delete")
                    .HasValue(x => x.Id.ToString())
                    .HasButtonSize(ButtonSize.ExtraSmall)
                    .HasButtonStyle(ButtonStyle.Danger)
                    .HasConfirmMessage(T(Constants.Messages.ConfirmDeleteRecord).Text);
            }

            result.AddReloadEvent("UPDATE_LIST_COMPLETE");
            result.AddReloadEvent("DELETE_LIST_COMPLETE");

            return result;
        }

        [Url("{DashboardBaseUrl}/lists/create")]
        public ActionResult Create()
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            WorkContext.Breadcrumbs.Add(T("Manage Lists"), Url.Action("Index", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(T("Create"));

            var model = new ListModel
            {
                Enabled = true,
                PageSize = 10
            };

            var result = new ControlFormResult<ListModel>(model)
            {
                Title = T("Create List").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            result.ExcludeProperty(x => x.Sorting);
            result.ExcludeProperty(x => x.CssClass);
            result.ExcludeProperty(x => x.SummaryTemplate);
            result.ExcludeProperty(x => x.DetailTemplate);

            return result;
        }

        [Url("{DashboardBaseUrl}/lists/edit/{id}")]
        public ActionResult Edit(int id)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var model = listService.GetById(id);

            WorkContext.Breadcrumbs.Add(T("Manage Lists"), Url.Action("Index", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(model.Name);
            WorkContext.Breadcrumbs.Add(T("Edit"));

            var result = new ControlFormResult<ListModel>(model)
            {
                Title = T("Edit List").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            var sortByFields = new Dictionary<object, string>
            {
                {"Position asc", "Position (Ascending)"},
                {"Position desc", "Position (Descending)"},
                {"Title asc", "Title (Ascending)"},
                {"Title desc", "Title (Descending)"},
                {"Slug asc", "Slug (Ascending)"},
                {"Slug desc", "Slug (Descending)"},
                {"Url asc", "Url (Ascending)"},
                {"Url desc", "Url (Descending)"},
                {"CreatedDate asc", "Created Date (Ascending)"},
                {"CreatedDate desc", "Created Date (Descending)"},
                {"ModifiedDate asc", "Modified Date (Ascending)"},
                {"ModifiedDate desc", "Modified Date (Descending)"},
            };

            result.RegisterExternalDataSource(x => x.Sorting, sortByFields);
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

            var list = listService.GetById(id);
            listService.EnableOrDisable(list);

            if (list.Enabled)
            {
                listPathConstraint.AddPath(list.Url, list.Id);
            }
            else
            {
                listPathConstraint.RemovePath(list.Url);
            }

            return new AjaxResult().NotifyMessage("UPDATE_LIST_COMPLETE");
        }

        [FormButton("Delete")]
        [HttpPost, ActionName("Update")]
        public ActionResult Delete(int id)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var list = listService.GetById(id);
            listService.Delete(list);
            listPathConstraint.RemovePath(list.Url);
            return new AjaxResult().NotifyMessage("DELETE_LIST_COMPLETE");
        }

        [ValidateInput(false), FormButton("Save")]
        [HttpPost, Url("{DashboardBaseUrl}/lists/update")]
        public ActionResult Update(ListModel model)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            string url;

            if (string.IsNullOrEmpty(model.Url))
            {
                url = model.Name.ToSlugUrl();
            }
            else
            {
                url = model.Url.Trim(' ', '/', '.', '~');
                if (string.IsNullOrEmpty(url))
                {
                    return new AjaxResult().Alert("Please correct Url value.");
                }
            }

            // Check unique url
            if (!listService.IsUniqueUrl(model.Id, url))
            {
                return new AjaxResult().Alert("That URL already exists. Please specify another.");
            }

            var list = model.Id != 0 ? listService.GetById(model.Id) : new List();
            list.Name = model.Name;
            list.Url = url;
            list.Sorting = model.Sorting;
            list.PageSize = model.PageSize;
            list.Enabled = model.Enabled;
            list.EnabledMetaTags = model.EnabledMetaTags;
            list.SummaryTemplate = model.SummaryTemplate;
            list.DetailTemplate = model.DetailTemplate;
            list.CssClass = model.CssClass;
            list.EnabledComments = model.EnabledComments;
            list.AutoModeratedComments = model.AutoModeratedComments;

            listService.Save(list);

            if (list.Enabled)
            {
                listPathConstraint.AddPath(list.Url, list.Id);
            }
            else
            {
                listPathConstraint.RemovePath(list.Url);
            }

            return new AjaxResult().Redirect(Url.Action("Index"));
        }

        private ControlGridAjaxData<List> GetLists(ControlGridFormRequest options)
        {
            int totals;
            var records = listService.GetRecords(options, out totals);
            return new ControlGridAjaxData<List>(records, totals);
        }
    }
}