using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using JetBrains.Annotations;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.ContentManagement.Lists.Models;
using CMSSolutions.ContentManagement.Lists.Routing;
using CMSSolutions.ContentManagement.Lists.Services;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Extensions;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Themes;
using CMSSolutions.Web.UI;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists.Controllers
{
    [Themed]
    [Feature(Constants.Areas.Lists)]
    public class HomeController : BaseController
    {
        private readonly IListService listService;
        private readonly IListCategoryService listCategoryService;
        private readonly IListItemService listItemService;
        private readonly IListItemCommentService listItemCommentService;
        private readonly IListFieldService listFieldService;
        private readonly IListCategoryPathConstraint listCategoryPathConstraint;

        public HomeController(IWorkContextAccessor workContextAccessor,
            IListService listService,
            IListItemService listItemService,
            IListItemCommentService listItemCommentService,
            IListCategoryService listCategoryService,
            IListFieldService listFieldService,
            IListCategoryPathConstraint listCategoryPathConstraint)
            : base(workContextAccessor)
        {
            this.listService = listService;
            this.listItemService = listItemService;
            this.listItemCommentService = listItemCommentService;
            this.listCategoryService = listCategoryService;
            this.listFieldService = listFieldService;
            this.listCategoryPathConstraint = listCategoryPathConstraint;
        }

        public ActionResult Index(int listId, int pageIndex)
        {
            var list = listService.GetById(listId);
            if (list == null)
            {
                return new HttpNotFoundResult();
            }

            int totals;
            var items = listItemService.GetListItems(listId, list.Sorting, pageIndex, list.PageSize, out totals);

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(list.CssClass))
            {
                sb.AppendFormat("<div class=\"{0}\">", list.CssClass);
            }

            if (items.Count > 0 && !string.IsNullOrEmpty(list.SummaryTemplate))
            {
                var fields = listFieldService.GetFields(listId);
                TemplateHelper.BuildContent(list, fields, items, list.SummaryTemplate, sb, Url);
            }

            TemplateHelper.BuildPagination(sb, Url, RouteData.Values, totals, pageIndex, list.PageSize);

            if (!string.IsNullOrEmpty(list.CssClass))
            {
                sb.Append("</div>");
            }

            return new ControlContentResult(sb.ToString())
            {
                Title = list.Name
            };
        }

        public ActionResult Category(int listId, string pathInfo)
        {
            var segments = pathInfo.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries);
            var pageIndex = 1;
            string slug;

            if (segments.Length > 1)
            {
                if (int.TryParse(segments[segments.Length - 1], out pageIndex))
                {
                    slug = string.Join("/", segments, 0, segments.Length - 1);
                }
                else
                {
                    slug = string.Join("/", segments);
                    pageIndex = 1;
                }
            }
            else
            {
                slug = segments[0];
            }

            int categoryId;
            if (listCategoryPathConstraint.Match(listId, slug, out categoryId))
            {
                var category = listCategoryService.GetById(categoryId);
                if (category == null)
                {
                    return new HttpNotFoundResult();
                }

                var list = listService.GetById(category.ListId);
                if (list == null)
                {
                    return new HttpNotFoundResult();
                }

                int totals;
                var items = listItemService.GetListItemsByCategoryId(listId, categoryId, list.Sorting, pageIndex, list.PageSize, out totals);

                var sb = new StringBuilder();

                if (!string.IsNullOrEmpty(list.CssClass))
                {
                    sb.AppendFormat("<div class=\"{0}\">", list.CssClass);
                }

                if (items.Count > 0 && !string.IsNullOrEmpty(list.SummaryTemplate))
                {
                    var fields = listFieldService.GetFields(list.Id);
                    TemplateHelper.BuildContent(list, fields, items, list.SummaryTemplate, sb, Url);
                }

                if (totals > 0)
                {
                    TemplateHelper.BuildPagination(sb, Url, RouteData.Values, totals, pageIndex, list.PageSize);
                }

                if (!string.IsNullOrEmpty(list.CssClass))
                {
                    sb.Append("</div>");
                }

                return new ControlContentResult(sb.ToString())
                {
                    Title = list.Name + " - " + category.Name
                };
            }

            return Item(listId, slug);
        }

        [GetModelState]
        public ActionResult Item(int listId, string slug)
        {
            var item = listItemService.GetListItem(listId, slug, true);

            if (item == null || !item.Enabled)
            {
                return new HttpNotFoundResult();
            }

            var list = listService.GetById(listId);

            var sb = new StringBuilder();

            if (!string.IsNullOrEmpty(list.CssClass))
            {
                sb.AppendFormat("<div class=\"{0}\">", list.CssClass);
            }

            if (!string.IsNullOrEmpty(list.DetailTemplate))
            {
                var fields = listFieldService.GetFields(list.Id);
                TemplateHelper.BuildContent(list, fields, item, list.DetailTemplate, sb, Url);
            }

            // Comments
            if (item.Comments != null && item.Comments.Count > 0)
            {
                sb.Append("<div class=\"comments\">");
                sb.AppendFormat("<header><h5>{0} {1}</h5></header>", item.Comments.Count, T("Comments"));

                sb.Append("<ol>");

                foreach (var comment in item.Comments.OrderBy(x => x.CreatedDate))
                {
                    sb.Append("<li class=\"comment\">");

                    sb.Append("<div class=\"comment-meta\">");
                    sb.AppendFormat("<span class=\"comment-author\">{0}</span>", comment.Name);
                    sb.Append("<br />");
                    sb.AppendFormat("<span class=\"comment-date\">{0} {1}</span>", T("Commented on"), comment.CreatedDate.ToString("dd MMM yyyy"));
                    sb.Append("</div>");

                    sb.Append("<p>");
                    sb.Append(comment.Comments);
                    sb.Append("</p>");

                    sb.Append("</li>");
                }

                sb.Append("</ol>");

                sb.Append("</div>");
            }

            if (list.EnabledComments)
            {
                sb.Append("<div class=\"post-comment\">");
                sb.AppendFormat("<form class=\"form-horizontal\" action=\"{0}\" method=\"post\" id=\"fPostComment\">", Url.Action("AddComment"));
                sb.AppendFormat("<input type=\"hidden\" name=\"ListId\" value=\"{0}\" />", listId);
                sb.AppendFormat("<input type=\"hidden\" name=\"ListItemId\" value=\"{0}\" />", item.Id);
                sb.AppendFormat("<fieldset><legend>{0}</legend></fieldset>", T("Leave a comment"));
                sb.AppendFormat("<div class=\"control-group\"><label class=\"control-label\" for=\"Name\">{0}</label><div class=\"controls\"><input type=\"text\" id=\"Name\" name=\"Name\" value=\"{1}\" placeholder=\"{0}\" data-val=\"true\" data-val-required=\"{2}\"/><span data-valmsg-for=\"Name\" data-valmsg-replace=\"true\"></span></div></div>", T("Name"), TryGetValue("Name"), T(Constants.Messages.Validation.Required));
                sb.AppendFormat("<div class=\"control-group\"><label class=\"control-label\" for=\"Email\">{0}</label><div class=\"controls\"><input type=\"text\" id=\"Email\" name=\"Email\" value=\"{1}\" placeholder=\"{0}\" data-val=\"true\" data-val-required=\"{2}\" data-val-email=\"{3}\"/><span data-valmsg-for=\"Email\" data-valmsg-replace=\"true\"></span></div></div>", T("Email"), TryGetValue("Email"), T(Constants.Messages.Validation.Required), T(Constants.Messages.Validation.Email));
                sb.AppendFormat("<div class=\"control-group\"><label class=\"control-label\" for=\"Website\">{0}</label><div class=\"controls\"><input type=\"text\" id=\"Website\" name=\"Website\" value=\"{1}\" placeholder=\"{0}\" data-val=\"true\" data-val-url=\"{2}\"/><span data-valmsg-for=\"Website\" data-valmsg-replace=\"true\"></span></div></div>", T("Website"), TryGetValue("Website"), T(Constants.Messages.Validation.Url));
                sb.AppendFormat("<div class=\"control-group\"><label class=\"control-label\" for=\"Comments\">{0}</label><div class=\"controls\"><textarea type=\"text\" id=\"Comments\" name=\"Comments\" placeholder=\"{0}\" data-val=\"true\" data-val-required=\"{2}\">{1}</textarea><span data-valmsg-for=\"Comments\" data-valmsg-replace=\"true\"></span></div></div>", T("Comments"), TryGetValue("Comments"), T(Constants.Messages.Validation.Required));
                sb.AppendFormat("<div class=\"control-group\"><div class=\"controls\"><button type=\"submit\" class=\"btn btn-primary\">{0}</button></div></div>", T("Post Comment"));

                sb.Append("</form>");

                sb.Append("<script type=\"text/javascript\">");
                sb.Append("$(document).ready(function(){");
                sb.Append("$(\"#fPostComment\").validate();");
                sb.Append("});");
                sb.Append("</script>");
                sb.Append("</div>");
            }

            if (!string.IsNullOrEmpty(list.CssClass))
            {
                sb.Append("</div>");
            }

            return new ControlContentResult(sb.ToString())
                       {
                           Title = item.Title,
                           AdditionResources = () => new[] { ResourceType.JQueryValidate }
                       };
        }

        private string TryGetValue(string name)
        {
            if (ModelState.ContainsKey(name))
            {
                var state = ModelState[name];
                return state.Value.AttemptedValue;
            }
            return string.Empty;
        }

        [HttpPost, PassModelState]
        public ActionResult AddComment(int listId, ListItemCommentModel model)
        {
            var list = listService.GetById(listId);
            if (list == null || !list.EnabledComments)
            {
                return new HttpNotFoundResult();
            }

            if (ModelState.IsValid && IsValidUrl(model.Website))
            {
                var comment = new ListComment
                {
                    Name = model.Name,
                    Email = model.Email,
                    Website = model.Website,
                    Comments = model.Comments,
                    ListId = listId,
                    ListItemId = model.ListItemId,
                    CreatedDate = DateTime.UtcNow,
                    IsApproved = list.AutoModeratedComments,
                    IPAddress = ClientIPAddress
                };

                listItemCommentService.Insert(comment);
            }

            return Redirect(Request.UrlReferrer == null
                ? Url.Content("~/")
                : Request.UrlReferrer.ToString());
        }

        [Themed(false)]
        public ActionResult LoadMore(int listId, int? categoryId, int? widgetId, int pageSize, int pageIndex = 2)
        {
            var list = listService.GetById(listId);
            int totals;
            var items = categoryId.HasValue
                ? listItemService.GetListItemsByCategoryId(listId, categoryId.Value, list.Sorting, pageIndex, pageSize, out totals)
                : listItemService.GetListItems(listId, list.Sorting, pageIndex, pageSize, out totals);

            if (items.Count == 0)
            {
                return Content("");
            }

            var htmlTemplate = list.SummaryTemplate;
            //if (widgetId.HasValue)
            //{
            //}

            var fields = listFieldService.GetFields(listId);
            var sb = new StringBuilder();
            TemplateHelper.BuildContent(list, fields, items, htmlTemplate, sb, Url);

            var pagesCount = (int)Math.Ceiling((double)totals / pageSize);
            if (pageIndex < pagesCount)
            {
                sb.AppendFormat("<a class=\"load-more\" href=\"{0}/load-more?categoryId={1}&pageSize={2}&widgetId={3}&pageIndex={4}\">Load more...</a>", list.Url, categoryId, pageSize, widgetId, pageIndex + 1);
            }

            return Content(sb.ToString());
        }

        #region Full Calendar

        public ActionResult GetEvents(int listId)
        {
            var list = listService.GetById(listId);
            if (list == null || !list.Enabled)
            {
                return new HttpNotFoundResult();
            }

            var items = listItemService.GetListItems(listId, null);

            return Json(items.Select(x => ConvertToEvent(list, x)), JsonRequestBehavior.AllowGet);
        }

        private CalendarEvent ConvertToEvent(List list, ListItem item)
        {
            var values = string.IsNullOrEmpty(item.Values) ? new Dictionary<string, object>() : item.Values.SharpDeserialize<Dictionary<string, object>>();

            var calendarEvent = new CalendarEvent
            {
                title = item.Title,
                start = Convert.ToString(list.GetFieldValue(values, "Start")),
                end = Convert.ToString(list.GetFieldValue(values, "End")),
                url = Url.Action("Item", "Home", new { slug = item.Slug })
            };
            return calendarEvent;
        }

        [Serializable]
        private class CalendarEvent
        {
            // ReSharper disable InconsistentNaming

            public string title { [UsedImplicitly] get; set; }

            public string start { [UsedImplicitly] get; set; }

            public string end { [UsedImplicitly] get; set; }

            public string url { [UsedImplicitly] get; set; }

            // ReSharper restore InconsistentNaming
        }

        #endregion Full Calendar

        private bool IsValidUrl(string website)
        {
            if (string.IsNullOrEmpty(website))
            {
                return true;
            }

            Uri test;
            var result = Uri.TryCreate(website, UriKind.Absolute, out test) && (test.Scheme == "http" || test.Scheme == "https");
            if (!result)
            {
                ModelState.AddModelError("Website", "Please enter a valid URL.");
            }
            return result;
        }
    }
}