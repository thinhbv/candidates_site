using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.Collections.Generic;

namespace CMSSolutions.Web.Mvc.Html
{
    public static class HtmlHelperExtensions
    {
        #region Tree

        public static MvcHtmlString RenderTree<TModel>(this HtmlHelper helper, IEnumerable<TModel> rootItems, Func<TModel, IEnumerable<TModel>> selector, Func<TModel, string> builder)
        {
            var sb = new StringBuilder();

            GenerateTree(sb, rootItems, selector, builder);

            return new MvcHtmlString(sb.ToString());
        }

        private static void GenerateTree<T>(StringBuilder sb, IEnumerable<T> items, Func<T, IEnumerable<T>> selector, Func<T, string> builder)
        {
            sb.Append("<ul>");

            foreach (var item in items)
            {
                sb.Append("<li>");
                sb.Append(builder(item));

                var childItems = selector(item).ToList();
                if (childItems.Any())
                {
                    GenerateTree(sb, childItems, selector, builder);
                }

                sb.Append("</li>");
            }

            sb.Append("</ul>");
        }

        #endregion Tree

        #region Pagination

        public static MvcHtmlString Pagination<T>(this HtmlHelper htmlHelper, IPagedList<T> pagedList, Func<int, string> getUrl, string nextText = "&raquo;", string prevText = "&laquo;")
        {
            if (pagedList.PageCount < 2)
            {
                return null;
            }

            var sb = new StringBuilder();
            sb.Append("<ul class=\"pagination\">");

            if (pagedList.HasPreviousPage)
            {
                sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", getUrl(pagedList.PageIndex - 1), prevText);
            }

            for (var i = 1; i <= pagedList.PageCount; i++)
            {
                sb.AppendFormat(
                    i == pagedList.PageIndex
                        ? "<li class=\"active\"><a href=\"{0}\">{1}</a></li>"
                        : "<li><a href=\"{0}\">{1}</a></li>", getUrl(i), i);
            }

            if (pagedList.HasNextPage)
            {
                sb.AppendFormat("<li><a href=\"{0}\">{1}</a></li>", getUrl(pagedList.PageIndex + 1), nextText);
            }

            sb.Append("</ul>");

            return new MvcHtmlString(sb.ToString());
        }

        #endregion
    }
}