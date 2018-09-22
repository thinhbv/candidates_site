using System.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Text;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using CMSSolutions.Extensions;
using CMSSolutions.Web.Mvc.Controls;
using CMSSolutions.Web.Mvc.Html;
using CMSSolutions.Web.Mvc.ControlForms;

namespace CMSSolutions.Web.Mvc
{
    public enum PageTarget
    {
        Default = 0,
        Blank,
        Parent,
        Self,
        Top
    }

    public static class HtmlHelperExtensions
    {
        #region Images

        public static MvcHtmlString Map(this HtmlHelper helper, string name, ImageMapHotSpot[] hotSpots)
        {
            return helper.Map(name, name, hotSpots);
        }

        public static MvcHtmlString Map(this HtmlHelper helper, string name, string id, ImageMapHotSpot[] hotSpots)
        {
            var map = new ImageMap
            {
                ID = id,
                Name = name,
                HotSpots = hotSpots
            };

            return new MvcHtmlString(map.ToString());
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string src, string alt)
        {
            return Image(helper, null, src, alt, null);
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string name, string src, string alt)
        {
            return Image(helper, name, src, alt, null);
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string name, string src, string alt, object htmlAttributes)
        {
            return helper.Image(name, src, alt, null, htmlAttributes);
        }

        public static MvcHtmlString Image(this HtmlHelper helper, string name, string src, string alt, string usemap, object htmlAttributes)
        {
            var urlHelper = new UrlHelper(helper.ViewContext.RequestContext);
            var builder = new TagBuilder("img");
            builder.GenerateId(name);
            builder.MergeAttribute("src", urlHelper.Content(src));

            if (!string.IsNullOrEmpty(alt))
            {
                builder.MergeAttribute("alt", alt);
            }
            if (!string.IsNullOrEmpty(name))
            {
                builder.MergeAttribute("name", name);
            }
            if (!string.IsNullOrEmpty(usemap))
            {
                builder.MergeAttribute("usemap", usemap);
            }

            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            return new MvcHtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString ImageLink(this HtmlHelper helper, string src, string alt, string href, PageTarget target = PageTarget.Default)
        {
            return helper.ImageLink(null, src, alt, href, null, null, target);
        }

        public static MvcHtmlString ImageLink(this HtmlHelper helper, string name, string src, string alt, string href, PageTarget target = PageTarget.Default)
        {
            return helper.ImageLink(name, src, alt, href, null, null, target);
        }

        public static MvcHtmlString ImageLink(this HtmlHelper helper, string name, string src, string alt, string href, object aHtmlAttributes, object imgHtmlAttributes, PageTarget target = PageTarget.Default)
        {
            var builder = new TagBuilder("a");
            builder.MergeAttribute("href", href);
            builder.GenerateId(name);

            if (!string.IsNullOrEmpty(name))
            {
                builder.MergeAttribute("name", name);
            }

            switch (target)
            {
                case PageTarget.Blank: builder.MergeAttribute("target", "_blank"); break;
                case PageTarget.Parent: builder.MergeAttribute("target", "_parent"); break;
                case PageTarget.Self: builder.MergeAttribute("target", "_self"); break;
                case PageTarget.Top: builder.MergeAttribute("target", "_top"); break;
            }

            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(aHtmlAttributes));

            var img = helper.Image(name + "Image", src, alt, imgHtmlAttributes);

            builder.InnerHtml = img.ToString();

            return MvcHtmlString.Create(builder.ToString());
        }

        #endregion Images

        #region Html Link

        public static MvcHtmlString EmailLink(this HtmlHelper helper, string emailAddress)
        {
            return helper.Link(string.Concat("mailto:", emailAddress));
        }

        public static MvcHtmlString Link(this HtmlHelper helper, string href, PageTarget target = PageTarget.Default)
        {
            return helper.Link(href, href, target);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, string linkText, string href, PageTarget target = PageTarget.Default)
        {
            return helper.Link(linkText, href, null, target);
        }

        public static MvcHtmlString Link(this HtmlHelper helper, string linkText, string href, object htmlAttributes, PageTarget target = PageTarget.Default)
        {
            var builder = new TagBuilder("a");
            builder.MergeAttribute("href", href);
            builder.InnerHtml = linkText;

            switch (target)
            {
                case PageTarget.Blank: builder.MergeAttribute("target", "_blank"); break;
                case PageTarget.Parent: builder.MergeAttribute("target", "_parent"); break;
                case PageTarget.Self: builder.MergeAttribute("target", "_self"); break;
                case PageTarget.Top: builder.MergeAttribute("target", "_top"); break;
            }

            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            return MvcHtmlString.Create(builder.ToString());
        }

        public static MvcHtmlString Link(this HtmlHelper helper, string linkText, string href, RouteValueDictionary htmlAttributes, PageTarget target = PageTarget.Default)
        {
            var builder = new TagBuilder("a");
            builder.MergeAttribute("href", href);
            builder.InnerHtml = linkText;

            switch (target)
            {
                case PageTarget.Blank: builder.MergeAttribute("target", "_blank"); break;
                case PageTarget.Parent: builder.MergeAttribute("target", "_parent"); break;
                case PageTarget.Self: builder.MergeAttribute("target", "_self"); break;
                case PageTarget.Top: builder.MergeAttribute("target", "_top"); break;
            }

            builder.MergeAttributes(htmlAttributes);

            return MvcHtmlString.Create(builder.ToString());
        }

        #endregion Html Link

        #region NumbersDropDown

        public static MvcHtmlString NumbersDropDown(this HtmlHelper html, string name, int min, int max)
        {
            return html.NumbersDropDown(name, min, max, min, null);
        }

        public static MvcHtmlString NumbersDropDown(this HtmlHelper html, string name, int min, int max, int selected)
        {
            return html.NumbersDropDown(name, min, max, selected, null);
        }

        public static MvcHtmlString NumbersDropDown(this HtmlHelper html, string name, int min, int max, int selected, object htmlAttributes)
        {
            var items = new List<SelectListItem>();

            for (int i = min; i <= max; i++)
            {
                var item = new SelectListItem
                {
                    Text = i.ToString(CultureInfo.InvariantCulture),
                    Value = i.ToString(CultureInfo.InvariantCulture),
                    Selected = i == selected
                };
                items.Add(item);
            }
            var selectList = new SelectList(items, "Value", "Text");

            return html.DropDownList(name, selectList, htmlAttributes);
        }

        public static MvcHtmlString NumbersDropDownFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int min, int max)
        {
            return html.NumbersDropDownFor(expression, min, max, null);
        }

        public static MvcHtmlString NumbersDropDownFor<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, int min, int max, object htmlAttributes)
        {
            var func = expression.Compile();
            var selectedValue = func(html.ViewData.Model);

            var items = new List<SelectListItem>();

            for (int i = min; i <= max; i++)
            {
                var item = new SelectListItem
                {
                    Text = i.ToString(CultureInfo.InvariantCulture),
                    Value = i.ToString(CultureInfo.InvariantCulture),
                    Selected = i.Equals(selectedValue)
                };
                items.Add(item);
            }
            var selectList = new SelectList(items, "Value", "Text");

            return html.DropDownListFor(expression, selectList, htmlAttributes);
        }

        #endregion NumbersDropDown

        #region Tables

        public static MvcHtmlString RenderAsTable<TModel, TValue>(this HtmlHelper<TModel> helper, IList<TValue> values, bool ignoreIfEmpty, int columns, string classes, Func<TValue, object> cellBuilder)
        {
            if (ignoreIfEmpty && values.Count == 0)
            {
                return null;
            }

            var rows = (int)Math.Ceiling((double)values.Count / columns);
            var columnWidth = (100d / columns).ToString("N2");

            var sb = new StringBuilder();
            sb.AppendFormat("<table class=\"{0}\">", classes);

            for (int r = 1; r <= rows; r++)
            {
                var start = (r * columns) - columns;
                var end = (r * columns) - 1;

                sb.Append("<tr>");

                if (rows == 1)
                {
                    for (var i = start; i <= end; i++)
                    {
                        if (i < values.Count)
                        {
                            sb.AppendFormat("<td style=\"width: {1}%;\">{0}</td>", cellBuilder(values[i]), columnWidth);
                        }
                    }
                }
                else
                {
                    for (var i = start; i <= end; i++)
                    {
                        sb.Append(i >= values.Count ? "<td></td>" : string.Format("<td style=\"width: {1}%;\">{0}</td>", cellBuilder(values[i]), columnWidth));
                    }
                }

                sb.Append("</tr>");
            }

            sb.Append("</table>");
            return new MvcHtmlString(sb.ToString());
        }

        #endregion Tables

        #region Ajax Form

        public static MvcAjaxForm BeginAjaxForm(this HtmlHelper htmlHelper, string formAction, object htmlAttributes = null)
        {
            return new MvcAjaxForm(htmlHelper, formAction, HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcAjaxForm BeginAjaxForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object htmlAttributes = null)
        {
            var routeValues = new RouteValueDictionary();
            if (htmlHelper.ViewContext.RouteData.Values.ContainsKey("namespaces"))
            {
                routeValues["namespaces"] = htmlHelper.ViewContext.RouteData.Values["namespaces"];
            }

            var formAction = UrlHelper.GenerateUrl(null, actionName, controllerName, routeValues, htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, true);
            return BeginAjaxForm(htmlHelper, formAction, htmlAttributes);
        }

        public static MvcAjaxForm BeginAjaxForm(this HtmlHelper htmlHelper, string actionName, string controllerName, FormMethod formMethod = FormMethod.Post, IDictionary<string, object> htmlAttributes = null)
        {
            var routeValues = new RouteValueDictionary();
            if (htmlHelper.ViewContext.RouteData.Values.ContainsKey("namespaces"))
            {
                routeValues["namespaces"] = htmlHelper.ViewContext.RouteData.Values["namespaces"];
            }

            var formAction = UrlHelper.GenerateUrl(null, actionName, controllerName, routeValues, htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, true);
            return new MvcAjaxForm(htmlHelper, formAction, htmlAttributes);
        }

        public static MvcAjaxForm BeginAjaxForm(this HtmlHelper htmlHelper, string actionName, string controllerName, object routeValues, object htmlAttributes)
        {
            var dictionary = new RouteValueDictionary(routeValues);
            if (!dictionary.ContainsKey("namespaces"))
            {
                if (htmlHelper.ViewContext.RouteData.Values.ContainsKey("namespaces"))
                {
                    dictionary["namespaces"] = htmlHelper.ViewContext.RouteData.Values["namespaces"];
                }
            }

            var formAction = UrlHelper.GenerateUrl(null, actionName, controllerName, dictionary, htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, true);
            return BeginAjaxForm(htmlHelper, formAction, htmlAttributes);
        }

        #endregion Ajax Form

        #region Pager

        //Example useage:
        //var pager = Html.Pager(Model.Listings.PageIndex, Model.Listings.PageCount, i => Url.Action("Index", new { pageIndex = i, pageSize = Model.Listings.PageSize }));
        //TODO: Make more options. maybe a fluent version
        public static MvcHtmlString Pager(this HtmlHelper helper, int pageIndex, int pageCount, Func<int, string> pageUrl)
        {
            if (pageCount <= 1)
            {
                return MvcHtmlString.Empty;
            }

            var ul = new TagBuilder("ul");
            ul.AddCssClass("pagination");

            if (pageIndex > 0)
            {
                #region First Page Link

                var li = new TagBuilder("li");
                var a = new TagBuilder("a");
                a.MergeAttribute("href", pageUrl(1));
                a.SetInnerText("<<");
                li.InnerHtml = a.ToString();
                ul.InnerHtml += li.ToString();

                #endregion First Page Link

                #region Previous Page Link

                li = new TagBuilder("li");
                a = new TagBuilder("a");
                a.MergeAttribute("href", pageUrl(pageIndex));
                a.SetInnerText("<");
                li.InnerHtml = a.ToString();
                ul.InnerHtml += li.ToString();

                #endregion Previous Page Link
            }

            #region Middle Pages

            int begin = Math.Max(1, pageIndex - 5);
            int end = Math.Min(begin + 10, pageCount);

            for (int i = begin; i <= end; i++)
            {
                var li = new TagBuilder("li");
                if (i == (pageIndex + 1))
                {
                    li.AddCssClass("active");
                }

                var a = new TagBuilder("a");
                a.MergeAttribute("href", pageUrl(i));
                a.SetInnerText(i.ToString(CultureInfo.InvariantCulture));

                li.InnerHtml = a.ToString();

                ul.InnerHtml += li.ToString();
            }

            #endregion Middle Pages

            if ((pageIndex + 1) < pageCount)
            {
                #region Next Page Link

                var li = new TagBuilder("li");
                var a = new TagBuilder("a");
                a.MergeAttribute("href", pageUrl(pageIndex + 2));
                a.SetInnerText(">");
                li.InnerHtml = a.ToString();
                ul.InnerHtml += li.ToString();

                #endregion Next Page Link

                #region Last Page Link

                li = new TagBuilder("li");
                a = new TagBuilder("a");
                a.MergeAttribute("href", pageUrl(pageCount));
                a.SetInnerText(">>");
                li.InnerHtml = a.ToString();
                ul.InnerHtml += li.ToString();

                #endregion Last Page Link
            }

            return MvcHtmlString.Create(ul.ToString());
        }

        //public static MvcHtmlString Pager<T>(this HtmlHelper htmlHelper, PagedList<T> pagedList, Func<int, string> generateUrl)
        //{
        //    var pages = pagedList.PageCount;

        //    if (pages <= 1)
        //    {
        //        return null;
        //    }

        //    var sb = new StringBuilder();
        //    sb.Append("<div class=\"pagination\">");
        //    sb.Append("<ul>");

        //    if (pagedList.PageIndex == 1)
        //    {
        //        sb.Append("<li class=\"disabled\"><a href=\"javascript:void(0)\">&lt;&lt;</a></li>");
        //    }
        //    else
        //    {
        //        sb.AppendFormat("<li><a href=\"{0}\">&lt;&lt;</a></li>", generateUrl(1));
        //    }

        //    for (var i = 1; i <= pages; i++)
        //    {
        //        if (i == pagedList.PageIndex)
        //        {
        //            sb.AppendFormat("<li class=\"active\"><a href=\"javascript:void(0)\">{0}</a></li>", i);
        //        }
        //        else
        //        {
        //            sb.AppendFormat("<li><a href=\"{1}\">{0}</a></li>", i, generateUrl(i));
        //        }
        //    }

        //    // Last
        //    if (pagedList.PageIndex == pages)
        //    {
        //        sb.Append("<li class=\"disabled\"><a href=\"javascript:void(0)\">&gt;&gt;</a></li>");
        //    }
        //    else
        //    {
        //        sb.AppendFormat("<li><a href=\"{0}\">&gt;&gt;</a></li>", generateUrl(pages));
        //    }

        //    sb.Append("</ul>");
        //    sb.Append("</div>");

        //    return new MvcHtmlString(sb.ToString());
        //}

        //public static MvcHtmlString Pager<T>(this HtmlHelper htmlHelper, PagedList<T> pagedList, bool ajaxEnabled = false)
        //{
        //    var pages = pagedList.PageCount;

        //    if (pages <= 1)
        //    {
        //        return null;
        //    }

        //    var urlHelper = new UrlHelper(htmlHelper.ViewContext.RequestContext, htmlHelper.RouteCollection);
        //    var action = htmlHelper.ViewContext.RouteData.GetRequiredString("action");
        //    var controller = htmlHelper.ViewContext.RouteData.GetRequiredString("controller");
        //    var emptyRouteValues = new RouteValueDictionary(htmlHelper.ViewContext.RouteData.Values);
        //    if (emptyRouteValues.ContainsKey("pageIndex"))
        //    {
        //        emptyRouteValues.Remove("pageIndex");
        //    }

        //    var sb = new StringBuilder();
        //    sb.Append("<div class=\"pagination\">");
        //    sb.Append("<ul>");

        //    if (pagedList.PageIndex == 1)
        //    {
        //        sb.Append("<li class=\"disabled\"><a href=\"javascript:void(0)\">&lt;&lt;</a></li>");
        //    }
        //    else
        //    {
        //        if (ajaxEnabled)
        //        {
        //            sb.AppendFormat("<li><button type=\"submit\" name=\"ChangePageIndex\" value=\"1\">&lt;&lt;</button></li>");
        //        }
        //        else
        //        {
        //            var url = urlHelper.Action(action, controller, new RouteValueDictionary(emptyRouteValues) { { "pageIndex", null } });
        //            sb.AppendFormat("<li><a href=\"{0}\">&lt;&lt;</a></li>", url);
        //        }
        //    }

        //    for (var i = 1; i <= pages; i++)
        //    {
        //        if (i == pagedList.PageIndex)
        //        {
        //            sb.AppendFormat("<li class=\"active\"><a href=\"javascript:void(0)\">{0}</a></li>", i);
        //        }
        //        else
        //        {
        //            if (ajaxEnabled)
        //            {
        //                sb.AppendFormat("<li><button type=\"submit\" name=\"ChangePageIndex\" value=\"{0}\">{0}</button></li>", i);
        //            }
        //            else
        //            {
        //                var url = urlHelper.Action(action, controller, i == 1 ?
        //                        new RouteValueDictionary(emptyRouteValues) { { "pageIndex", null } } :
        //                        new RouteValueDictionary(emptyRouteValues) { { "pageIndex", i } });
        //                sb.AppendFormat("<li><a href=\"{1}\">{0}</a></li>", i, url);
        //            }
        //        }
        //    }

        //    if (pagedList.PageIndex == pages)
        //    {
        //        sb.Append("<li class=\"disabled\"><a href=\"javascript:void(0)\">&gt;&gt;</a></li>");
        //    }
        //    else
        //    {
        //        if (ajaxEnabled)
        //        {
        //            sb.AppendFormat("<li><button type=\"submit\" name=\"ChangePageIndex\" value=\"{0}\">&gt;&gt;</button></li>", pages);
        //        }
        //        else
        //        {
        //            var url = urlHelper.Action(action, controller, new RouteValueDictionary(emptyRouteValues) { { "pageIndex", pages } });
        //            sb.AppendFormat("<li><a href=\"{0}\">&gt;&gt;</a></li>", url);
        //        }
        //    }

        //    sb.Append("</ul>");
        //    sb.Append("</div>");

        //    return new MvcHtmlString(sb.ToString());
        //}

        #endregion Pager

        #region Other

        public static MvcHtmlString FileUpload(this HtmlHelper html, string name, object htmlAttributes)
        {
            var builder = new TagBuilder("input");
            builder.MergeAttribute("type", "file");
            builder.GenerateId(name);

            if (!string.IsNullOrEmpty(name))
            {
                builder.MergeAttribute("name", name);
            }

            builder.MergeAttributes(HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));

            return new MvcHtmlString(builder.ToString(TagRenderMode.SelfClosing));
        }

        public static MvcHtmlString Multiple(this HtmlHelper html, params MvcHtmlString[] controls)
        {
            if (controls == null || controls.Length == 0)
            {
                return null;
            }

            var sb = new StringBuilder();
            foreach (var control in controls)
            {
                sb.Append(control);
            }

            return new MvcHtmlString(sb.ToString());
        }

        public static MvcHtmlString JsonDataSource<TEntity>(this HtmlHelper html, string name, TEntity item)
        {
            return new MvcHtmlString(string.Format("<script type=\"text/javascript\">var {0} = {1};</script>", name, item.ToJson()));
        }

        #endregion Other

        public static MvcHtmlString EnumDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> html, Expression<Func<TModel, TEnum>> expression, string emptyText = null, object htmlAttributes = null) where TEnum : struct
        {
            //string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            //string fullHtmlFieldName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);

            var func = expression.Compile();
            var selectedValue = func(html.ViewData.Model);

            var selectList = EnumExtensions.ToSelectList<TEnum>(selectedValue, emptyText);
            return html.DropDownListFor(expression, selectList, htmlAttributes);
        }

        public static MvcHtmlString EnumMultiDropDownListFor<TModel, TEnum>(this HtmlHelper<TModel> html, Expression<Func<TModel, IEnumerable<TEnum>>> expression, string emptyText = null, object htmlAttributes = null) where TEnum : struct
        {
            //string htmlFieldName = ExpressionHelper.GetExpressionText(expression);
            //string fullHtmlFieldName = html.ViewContext.ViewData.TemplateInfo.GetFullHtmlFieldName(htmlFieldName);

            var func = expression.Compile();
            var selectedValues = func(html.ViewData.Model);

            var parsedSelectedValues = Enumerable.Empty<long>();

            if (selectedValues != null)
            {
                parsedSelectedValues = selectedValues.Select(x => Convert.ToInt64(x));
            }

            var multiSelectList = EnumExtensions.ToMultiSelectList<TEnum>(parsedSelectedValues, emptyText);

            return html.ListBoxFor(expression, multiSelectList, htmlAttributes);
        }

        public static ControlForm<TModel> ControlForm<TModel>(this HtmlHelper htmlHelper) where TModel : class
        {
            return new ControlForm<TModel>(htmlHelper);
        }

        public static ControlGrid<TModel> ControlGrid<TModel>(this HtmlHelper htmlHelper) where TModel : class
        {
            return new ControlGrid<TModel>(htmlHelper);
        }
    }
}