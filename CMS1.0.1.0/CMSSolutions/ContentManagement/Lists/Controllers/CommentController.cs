using System;
using System.Web.Mvc;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.ContentManagement.Lists.Models;
using CMSSolutions.ContentManagement.Lists.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Routing;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists.Controllers
{
    [Authorize]
    [Themed(IsDashboard = true)]
    [Feature(Constants.Areas.Lists)]
    public class CommentController : BaseController
    {
        private readonly IListService listService;
        private readonly IListItemService listItemService;
        private readonly IListItemCommentService commentService;

        public CommentController(
            IWorkContextAccessor workContextAccessor,
            IListService listService,
            IListItemService listItemService,
            IListItemCommentService commentService)
            : base(workContextAccessor)
        {
            this.listService = listService;
            this.listItemService = listItemService;
            this.commentService = commentService;
        }

        [Url("{DashboardBaseUrl}/lists/comments/{listId}")]
        public ActionResult Index(int listId)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var list = listService.GetById(listId);

            WorkContext.Breadcrumbs.Add(T("Manage Lists"), Url.Action("Index", "List", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(list.Name);
            WorkContext.Breadcrumbs.Add(T("Comments"));

            var result = new ControlGridFormResult<ListComment>
            {
                Title = list.Name,
                UpdateActionName = "Update",
                FetchAjaxSource = options => GetListItemComments(options, listId),
                EnablePaginate = true,
                DefaultPageSize = WorkContext.DefaultPageSize,
                EnableSearch = true,
                GridWrapperStartHtml = Constants.Grid.GridWrapperStartHtml,
                GridWrapperEndHtml = Constants.Grid.GridWrapperEndHtml
            };

            result.AddColumn(x => x.Name);
            result.AddColumn(x => x.Email);
            result.AddColumn(x => x.IPAddress).HasHeaderText("IP Address");
            result.AddColumn(x => x.CreatedDate).HasHeaderText("Created").HasDisplayFormat("{0:d}");
            result.AddColumn(x => x.IsApproved).HasHeaderText("Approved").RenderAsStatusImage().AlignCenter();

            result.AddRowAction()
                .HasText(T("Edit"))
                .HasUrl(x => Url.Action("Edit", RouteData.Values.Merge(new { id = x.Id }, "listId")))
                .HasButtonSize(ButtonSize.ExtraSmall)
                .ShowModalDialog(700);

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
        [Url("{DashboardBaseUrl}/lists/comments/edit/{id}")]
        public ActionResult Edit(int id)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var model = commentService.GetById(id);
            var listItem = listItemService.GetById(model.ListItemId);
            var list = listService.GetById(listItem.ListId);

            WorkContext.Breadcrumbs.Add(T("Manage Lists"), Url.Action("Index", "List", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(list.Name);
            WorkContext.Breadcrumbs.Add(T("Comments"), Url.Action("Index", new { area = Constants.Areas.Lists }));
            WorkContext.Breadcrumbs.Add(T("Edit"));

            var result = new ControlFormResult<ListItemCommentModel>(model)
            {
                Title = T("Edit List").Text,
                UpdateActionName = "Update",
                ShowBoxHeader = false,
                FormWrapperStartHtml = Constants.Form.FormWrapperStartHtml,
                FormWrapperEndHtml = Constants.Form.FormWrapperEndHtml
            };

            return result;
        }

        [ValidateInput(false), FormButton("Save")]
        [HttpPost, Url("{DashboardBaseUrl}/lists/comments/update")]
        public ActionResult Update(ListItemCommentModel model)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var comment = commentService.GetById(model.Id);
            comment.Name = model.Name;
            comment.Email = model.Email;
            comment.Website = model.Website;
            comment.IsApproved = model.IsApproved;
            comment.Comments = model.Comments;
            commentService.Save(comment);

            return new AjaxResult().NotifyMessage("UPDATE_ENTITY_COMPLETE").CloseModalDialog();
        }

        [FormButton("Delete")]
        [HttpPost, ActionName("Update")]
        public ActionResult Delete(int id)
        {
            if (!CheckPermission(ListsPermissions.ManageLists))
            {
                return new HttpUnauthorizedResult();
            }

            var comment = commentService.GetById(id);
            commentService.Delete(comment);
            return new AjaxResult().NotifyMessage("DELETE_ENTITY_COMPLETE");
        }

        private ControlGridAjaxData<ListComment> GetListItemComments(ControlGridFormRequest options, int listId)
        {
            int totals;
            var records = commentService.GetCommentsOfList(listId, options.PageIndex, options.PageSize, out totals);
            return new ControlGridAjaxData<ListComment>(records, totals);
        }
    }
}