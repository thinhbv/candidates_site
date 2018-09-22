using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.ContentManagement.Lists.Services;
using CMSSolutions.ContentManagement.Widgets;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists
{
    [Feature(Constants.Areas.Lists)]
    public class ListCategoriesWidget : WidgetBase
    {
        public override string Name
        {
            get { return "List Categories Widget"; }
        }

        [Display(Name = "List")]
        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "List", ContainerCssClass = "col-xs-3 col-md-3", ContainerRowIndex = 3)]
        public int ListId { get; set; }

        public override void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            if (ListId == 0)
            {
                return;
            }

            var workContext = workContextAccessor.GetContext(viewContext.HttpContext);
            var listService = workContext.Resolve<IListService>();
            var urlHelper = workContext.Resolve<UrlHelper>();

            var list = listService.GetById(ListId);
            if (list == null)
            {
                return;
            }

            var categoryService = workContext.Resolve<IListCategoryService>();
            var categories = categoryService.GetCategories(ListId);

            if (categories.Count == 0)
            {
                return;
            }

            if (!string.IsNullOrEmpty(CssClass))
            {
                writer.AddAttribute(HtmlTextWriterAttribute.Class, CssClass);
            }
            writer.RenderBeginTag(HtmlTextWriterTag.Div);

            if (ShowTitleOnPage)
            {
                writer.Write("<header><h3>{0}</h3></header>", Title);
            }

            RenderCategories(writer, urlHelper, categories.Where(x => x.ParentId == null), categories, list.Url);

            writer.RenderEndTag(); // div
        }

        private static void RenderCategories(HtmlTextWriter writer, UrlHelper urlHelper, IEnumerable<ListCategory> categories, IList<ListCategory> allCategories, string listUrl)
        {
            writer.Write("<ul>");
            foreach (var category in categories)
            {
                writer.Write("<li>");
                writer.Write("<a href=\"{0}\">{1}</a>", urlHelper.Content(string.Format("~/{0}/{1}", listUrl, category.FullUrl)), category.Name);
                var childCategories = allCategories.Where(x => x.ParentId == category.Id).ToList();
                if (childCategories.Count > 0)
                {
                    RenderCategories(writer, urlHelper, childCategories, allCategories, listUrl);
                }
                writer.Write("</li>");
            }
            writer.Write("</ul>");
        }

        public override ActionResult BuildEditor(Controller controller, WorkContext workContext, ControlFormResult<IWidget> form)
        {
            var listService = workContext.Resolve<IListService>();
            var list = listService.GetRecords().OrderBy(x => x.Name).ToDictionary(x => x.Id, x => x.Name);
            form.RegisterExternalDataSource("ListId", list);

            return form;
        }
    }
}