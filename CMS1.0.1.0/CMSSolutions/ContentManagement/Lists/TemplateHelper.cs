using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using RazorEngine;
using RazorEngine.Templating;
using RazorEngine.Text;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.ContentManagement.Lists.Fields;
using CMSSolutions.Extensions;
using CMSSolutions.Web.Routing;

namespace CMSSolutions.ContentManagement.Lists
{
    internal static class TemplateHelper
    {
        public static void BuildContent(List list, IList<IListField> fields, IList<ListItem> items, string contentTemplate, StringBuilder stringBuilder, UrlHelper urlHelper)
        {
            if (string.IsNullOrEmpty(contentTemplate)) return;

            var templateName = string.Format("LIST_CONTENT_ITEMS_TEMPLATE_{0}", contentTemplate.GetHashCode());

            var model = new List<TemplateViewModel>();
            var httpContext = urlHelper.RequestContext.HttpContext;

            foreach (var item in items)
            {
                var localItem = item;
                var itemUrl = urlHelper.Content(string.Format("~/{0}/{1}", list.Url, item.Slug));

                IDictionary<string, object> values;

                if (!string.IsNullOrEmpty(item.Values))
                {
                    try
                    {
                        values = item.Values.SharpDeserialize<IDictionary<string, object>>();
                    }
                    catch (Exception)
                    {
                        values = new Dictionary<string, object>();
                    }
                }
                else
                {
                    values = new Dictionary<string, object>();
                }

                var obj = new TemplateViewModel();

                foreach (var pair in values)
                {
                    var field = fields.FirstOrDefault(x => x.Name == pair.Key);
                    if (field == null) continue;
                    var localValue = pair;
                    obj[pair.Key] = args => field.RenderField(localValue.Value, args);
                }

                obj["Title"] = args => localItem.Title;
                obj["MetaKeywords"] = args => localItem.MetaKeywords;
                obj["MetaDescription"] = args => localItem.MetaDescription;
                obj["ListItemUrl"] = args => itemUrl;
                obj["PictureUrl"] = args => string.IsNullOrEmpty(localItem.PictureUrl) ? args != null && args.Length > 0 ? Convert.ToString(args[0]) : "/Content/Images/notfound.png" : localItem.PictureUrl;

                obj["CreatedDate"] = args =>
                {
                    if (args != null && args.Length > 0)
                    {
                        return localItem.CreatedDate.ToString(Convert.ToString(args[0]));
                    }
                    return localItem.CreatedDate;
                };

                obj["ModifiedDate"] = args =>
                {
                    if (args != null && args.Length > 0)
                    {
                        return localItem.ModifiedDate.ToString(Convert.ToString(args[0]));
                    }
                    return localItem.ModifiedDate;
                };

                model.Add(obj);
            }

            var template = Razor.GetTemplate(contentTemplate, model, templateName);

            dynamic viewBag = new DynamicViewBag();
            viewBag.HttpContext = httpContext;
            stringBuilder.Append(template.Run(new ExecuteContext(viewBag)));
        }

        public static void BuildContent(List list, IList<IListField> fields, ListItem item, string contentTemplate, StringBuilder stringBuilder, UrlHelper urlHelper)
        {
            if (string.IsNullOrEmpty(contentTemplate)) return;

            var templateName = string.Format("LIST_CONTENT_ITEM_TEMPLATE_{0}", contentTemplate.GetHashCode());

            var itemUrl = urlHelper.Content(string.Format("~/{0}/{1}", list.Url, item.Slug));

            var values = item.Values.SharpDeserialize<IDictionary<string, object>>() ?? new Dictionary<string, object>();

            var model = new TemplateViewModel();

            foreach (var pair in values)
            {
                var field = fields.FirstOrDefault(x => x.Name == pair.Key);
                if (field == null) continue;
                var localValue = pair;
                model[pair.Key] = args => field.RenderField(localValue.Value, args);
            }

            model["Title"] = args => item.Title;
            model["MetaKeywords"] = args => item.MetaKeywords;
            model["MetaDescription"] = args => item.MetaDescription;
            model["ListItemUrl"] = args => itemUrl;
            model["PictureUrl"] = args => item.PictureUrl;

            model["CreatedDate"] = args =>
            {
                if (args != null && args.Length > 0)
                {
                    return item.CreatedDate.ToString(Convert.ToString(args[0]));
                }
                return item.CreatedDate;
            };

            model["ModifiedDate"] = args =>
            {
                if (args != null && args.Length > 0)
                {
                    return item.ModifiedDate.ToString(Convert.ToString(args[0]));
                }
                return item.ModifiedDate;
            };

            var template = Razor.GetTemplate(contentTemplate, model, templateName);
            stringBuilder.Append(template.Run(new ExecuteContext()));
        }

        public static void BuildPagination(StringBuilder stringBuilder, UrlHelper urlHelper, RouteValueDictionary baseRouteValues, int totals, int pageIndex, int pageSize)
        {
            if (totals <= 0)
            {
                return;
            }

            var pages = (int)Math.Ceiling((double)totals / pageSize);
            if (pages <= 1)
            {
                return;
            }

            var routeValues = new RouteValueDictionary(baseRouteValues);
            routeValues.Remove("pageIndex");
            routeValues.Remove("listId");

            stringBuilder.Append("<ul class=\"pagination\">");

            if (pageIndex > 2)
            {
                stringBuilder.AppendFormat("<li><a href=\"{1}\">{0}</a></li>", "First",
                                urlHelper.Action(null, null, routeValues.Merge(new { pageIndex = 1 })));
            }

            if (pageIndex > 1)
            {
                stringBuilder.AppendFormat("<li><a href=\"{1}\" rel=\"prev\">{0}</a></li>", "Prev",
                                urlHelper.Action(null, null, routeValues.Merge(new { pageIndex = pageIndex - 1 })));
            }

            for (var i = 1; i <= pages; i++)
            {
                if (pageIndex == i)
                {
                    stringBuilder.AppendFormat("<li class=\"current\"><a>{0}</a></li>", i);
                }
                else
                {
                    stringBuilder.AppendFormat("<li><a href=\"{1}\">{0}</a></li>", i,
                                    urlHelper.Action(null, null, routeValues.Merge(new { pageIndex = i })));
                }
            }

            if (pageIndex < pages)
            {
                stringBuilder.AppendFormat("<li><a href=\"{1}\" rel=\"next\">{0}</a></li>", "Next",
                                urlHelper.Action(null, null, routeValues.Merge(new { pageIndex = pageIndex + 1 })));
            }

            if (pageIndex < pages - 1)
            {
                stringBuilder.AppendFormat("<li><a href=\"{1}\">{0}</a></li>", "Last",
                                urlHelper.Action(null, null, routeValues.Merge(new { pageIndex = pages })));
            }

            stringBuilder.Append("</ul>");
        }

        private class TemplateViewModel : DynamicObject
        {
            private readonly IDictionary<string, Func<object[], object>> values;

            public TemplateViewModel()
            {
                values = new Dictionary<string, Func<object[], object>>();
            }

            public Func<object[], object> this[string name]
            {
                set { values[name] = value; }
            }

            public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
            {
                if (values.ContainsKey(binder.Name))
                {
                    result = values[binder.Name](args);

                    var htmlString = result as IHtmlString;

                    if (htmlString != null)
                    {
                        result = new RawString(htmlString.ToHtmlString());
                    }

                    return true;
                }

                result = null;
                return true;
            }
        }
    }
}