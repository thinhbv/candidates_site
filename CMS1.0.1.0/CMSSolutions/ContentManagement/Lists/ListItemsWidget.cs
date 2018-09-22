using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.UI;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.ContentManagement.Lists.Services;
using CMSSolutions.ContentManagement.Widgets;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;
using CMSSolutions.Serialization;
using CMSSolutions.Web.UI;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists
{
    [Feature(Constants.Areas.Lists)]
    public class ListItemsWidget : WidgetBase
    {
        public ListItemsWidget()
        {
            T = NullLocalizer.Instance;
        }

        [ExcludeFromSerialization]
        public Localizer T { get; set; }

        public override string Name
        {
            get { return "List Items Widget"; }
        }

        [Display(Name = "List")]
        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "List", ContainerCssClass = "col-xs-3 col-md-3", ContainerRowIndex = 3)]
        public int ListId { get; set; }

        [Display(Name = "Category")]
        [ControlCascadingDropDown(ParentControl = "ListId", LabelText = "Category", ContainerCssClass = "col-xs-3 col-md-3", ContainerRowIndex = 3)]
        public int? CategoryId { get; set; }

        [ControlNumeric(Required = true, MinimumValue = "0", LabelText = "Number of Items", ContainerCssClass = "col-xs-3 col-md-3", ContainerRowIndex = 3)]
        public int NumberOfItems { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "Show Pagination", ContainerCssClass = "col-xs-3 col-md-3", ContainerRowIndex = 4)]
        public bool ShowPagination { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "Enable Lazy Loading", ContainerCssClass = "col-xs-3 col-md-3", ContainerRowIndex = 4)]
        public bool EnableLazyLoading { get; set; }

        [ControlText(Type = ControlText.MultiText, LabelText = "Html Template", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 4)]
        public string HtmlTemplate { get; set; }

        public override IEnumerable<ResourceType> GetAdditionalResources()
        {
            if (EnableLazyLoading)
            {
                yield return ResourceType.JScroll;
            }
        }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            if (ListId == 0)
            {
                return;
            }

            if (string.IsNullOrEmpty(HtmlTemplate))
            {
                return;
            }

            var workContext = workContextAccessor.GetContext(viewContext.HttpContext);
            var listService = workContext.Resolve<IListService>();
            var list = listService.GetById(ListId);

            if (list == null)
            {
                return;
            }

            var pageSize = NumberOfItems == 0 ? int.MaxValue : NumberOfItems;
            var listItemService = workContext.Resolve<IListItemService>();
            int totals;
            IList<ListItem> items;

            if (CategoryId.HasValue && CategoryId.Value != 0)
            {
                items = listItemService.GetListItemsByCategoryId(ListId, CategoryId.Value, list.Sorting, 1, pageSize, out totals);
            }
            else
            {
                items = listItemService.GetListItems(ListId, list.Sorting, 1, pageSize, out totals);
            }

            if (items.Count == 0)
            {
                return;
            }

            var urlHelper = new UrlHelper(viewContext.RequestContext);
            var fieldService = workContext.Resolve<IListFieldService>();
            var fields = fieldService.GetFields(ListId);
            var clientId = Guid.NewGuid().ToString("N").ToLowerInvariant();

            if (!string.IsNullOrEmpty(CssClass))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            }

            writer.AddAttribute(HtmlTextWriterAttribute.Id, "div-" + clientId);
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (ShowTitleOnPage)
            {
                writer.Write("<header><h3>{0}</h3></header>", Title);
            }

            var sb = new StringBuilder();
            TemplateHelper.BuildContent(list, fields, items, HtmlTemplate, sb, urlHelper);

            if (ShowPagination && pageSize < int.MaxValue)
            {
                var routes = new RouteValueDictionary
                                 {
                                     {"action", "Index"},
                                     {"controller", "Home"},
                                     {"area", Constants.Areas.Lists},
                                     {"listSlug", list.Url}
                                 };

                TemplateHelper.BuildPagination(sb, urlHelper, routes, totals, 1, NumberOfItems);
            }

            writer.Write(sb.ToString());

            if (EnableLazyLoading)
            {
                writer.Write("<a class=\"load-more\" href=\"{0}/load-more?categoryId={1}&pageSize={2}&widgetId={3}\">Load more...</a>", list.Url, CategoryId, pageSize, Id);

                writer.WriteLine("<script type=\"text/javascript\">");
                writer.Write("$(document).ready(function(){");
                writer.Write("$('#div-{0}').jscroll({{ nextSelector: 'a.load-more' }});", clientId);
                writer.Write("});");
                writer.WriteLine("</script>");
            }

            writer.RenderEndTag(); // div
        }

        public override ActionResult BuildEditor(Controller controller, WorkContext workContext, ControlFormResult<IWidget> form)
        {
            var listService = workContext.Resolve<IListService>();
            var list = listService.GetRecords().OrderBy(x => x.Name).ToDictionary(x => x.Id, x => x.Name);
            form.RegisterExternalDataSource("ListId", list);

            form.RegisterCascadingDropDownDataSource("CategoryId", new ControlCascadingDropDownOptions
                                                                       {
                                                                           SourceUrl = controller.Url.Action("EditorCallback"),
                                                                           Command = "GetCategories"
                                                                       });

            return form;
        }

        public override ActionResult EditorCallback(Controller controller, WorkContext workContext)
        {
            var command = controller.Request["command"];

            if (command == "GetCategories")
            {
                var categoryService = workContext.Resolve<IListCategoryService>();
                var listId = Convert.ToInt32(controller.Request.Form["ListId"]);
                var categories = categoryService.GetCategories(listId).Select(x => new
                {
                    Text = x.Name,
                    Value = Convert.ToString(x.Id)
                }).ToList();

                categories.Insert(0, new { Text = "", Value = "" });

                return new JsonResult
                           {
                               Data = categories
                           };
            }

            return base.EditorCallback(controller, workContext);
        }
    }
}