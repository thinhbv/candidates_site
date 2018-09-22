using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.UI.JQueryBuilder;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlRepeaterFormResult<TModel> : BaseControlFormResult
    {
        #region Private Members

        private readonly IEnumerable<TModel> items;
        private readonly IList<ControlFormAction> actions;
        private readonly IDictionary<string, string> attachedSystemMessages;
        private readonly IDictionary<string, string> customrVars;

        #endregion Private Members

        #region Constructors

        public ControlRepeaterFormResult()
            : this(new List<TModel>())
        {
        }

        public ControlRepeaterFormResult(IEnumerable<TModel> items)
        {
            if (items == null)
            {
                throw new ArgumentNullException("items");
            }

            this.items = items;
            actions = new List<ControlFormAction>();
            attachedSystemMessages = new Dictionary<string, string>();
            customrVars = new Dictionary<string, string>();
            IsAjaxSupported = true;
            DefaultPageSize = 10;
        }

        #endregion Constructors

        #region Properties

        private string clientId;

        public string ClientId
        {
            get
            {
                if (string.IsNullOrEmpty(clientId))
                {
                    clientId = "tblDataTable_" + Guid.NewGuid().ToString("N").ToLowerInvariant();
                }
                return clientId;
            }
            set { clientId = value; }
        }

        public string CssClass { get; set; }

        public string UpdateActionName { get; set; }

        public bool IsAjaxSupported { get; set; }

        public bool EnableSearch { get; set; }

        public bool EnableSorting { get; set; }

        public bool EnableAjaxSource { get; set; }

        public Func<ControlGridFormRequest, ControlGridAjaxData<TModel>> FetchAjaxSource { get; set; }

        public bool EnablePaginate { get; set; }

        public bool EnablePageSizeChange { get; set; }

        public int DefaultPageSize { get; set; }

        public bool HideActionsColumn { get; set; }

        /// <summary>
        /// Razor Engine template
        /// </summary>
        public string TemplateView { get; set; }

        #endregion Properties

        #region Public Methods

        public ControlFormAction AddAction(bool isSubmitButton = false, bool isAjaxSupport = true)
        {
            var action = new ControlFormAction(isSubmitButton, isAjaxSupport);
            actions.Add(action);
            return action;
        }

        public void AddAction(ControlFormAction action)
        {
            actions.Add(action);
        }

        public void AddReloadEvent(string message, string action)
        {
            attachedSystemMessages[message] = action;
        }

        public void AddCustomVar(string name, object value, bool isFunction = false)
        {
            if (value == null)
            {
                throw new NullReferenceException();
            }

            if (isFunction || !(value is string))
            {
                customrVars.Add(name, value.ToString());
            }
            else
            {
                customrVars.Add(name, JQueryUtility.EncodeJsString(value.ToString()));
            }
        }

        public override IEnumerable<ResourceType> GetAdditionalResources(ControllerContext context)
        {
            yield return ResourceType.JQueryValidate;
        }

        public override bool OverrideExecuteResult(ControllerContext context)
        {
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                // Return data only
                var request = ControlGridFormRequest.Create(context.HttpContext.Request);
                if (request.PageSize <= 0)
                {
                    request.PageSize = DefaultPageSize;
                }
                var data = FetchAjaxSource(request);

                var response = context.HttpContext.Response;
                response.ContentType = "application/json";
                response.ContentEncoding = Encoding.UTF8;

                using (var writer = new JsonTextWriter(response.Output) { Formatting = Formatting.None })
                {
                    writer.WriteStartObject();

                    writer.WritePropertyName("iTotalRecords");
                    var totalRecords = data.TotalRecords > 0 ? data.TotalRecords : data.Count;
                    writer.WriteValue(totalRecords);

                    writer.WritePropertyName("aaData");
                    writer.WriteStartArray();

                    var needWriteValueDelimiter = false;
                    foreach (var item in data)
                    {
                        var jsonObject = new JObject();

                        //foreach (var column in columns)
                        //{
                        //    if (column.IsCheckbox)
                        //    {
                        //        throw new NotImplementedException();
                        //    }
                        //    else
                        //    {
                        //        jsonObject.Add(column.PropertyName, Convert.ToString(column.HtmlBuilder(item)));
                        //    }
                        //}

                        //if (rowActions.Count > 0 && !HideActionsColumn)
                        //{
                        //    var sb = new StringBuilder();
                        //    sb.Append("<table style=\"margin-left: auto; margin-right: auto; border: none; padding: 0;\">");
                        //    sb.Append("<tr>");

                        //    var rowActionIndex = -1;
                        //    foreach (var action in rowActions)
                        //    {
                        //        rowActionIndex++;

                        //        if (action.VisibleWhenFunc != null && !action.VisibleWhenFunc(item))
                        //        {
                        //            continue;
                        //        }

                        //        sb.Append("<td style=\"border: none; background-color: transparent; padding: 0 2px;\">");
                        //        if (action.IsSubmitButton)
                        //        {
                        //            var value = action.ValueSelector != null ? action.ValueSelector(item) : null;
                        //            var attributes = new RouteValueDictionary();
                        //            if (!string.IsNullOrEmpty(action.CssClass))
                        //            {
                        //                attributes.Add("class", action.CssClass);
                        //            }

                        //            if (!string.IsNullOrEmpty(action.ConfirmMessage))
                        //            {
                        //                attributes.Add("onclick", string.Format("return confirm('{0}');", action.ConfirmMessage));
                        //            }

                        //            attributes.Add("id", "btn" + Guid.NewGuid().ToString("N").ToLowerInvariant());
                        //            attributes.Add("style", "white-space: nowrap;");

                        //            var tagBuilder = new TagBuilder("button");
                        //            tagBuilder.MergeAttribute("type", "submit");
                        //            tagBuilder.MergeAttribute("name", action.Name);
                        //            tagBuilder.InnerHtml = action.Text;
                        //            tagBuilder.MergeAttribute("value", value);
                        //            tagBuilder.MergeAttributes(attributes);
                        //            sb.Append(tagBuilder.ToString(TagRenderMode.Normal));
                        //        }
                        //        else
                        //        {
                        //            var href = action.UrlBuilder != null ? action.UrlBuilder(item) : null;
                        //            var attributes = new RouteValueDictionary();
                        //            if (!string.IsNullOrEmpty(action.CssClass))
                        //            {
                        //                attributes.Add("class", action.CssClass);
                        //            }

                        //            attributes.Add("style", "white-space: nowrap;");

                        //            if (action.IsShowModalDialog)
                        //            {
                        //                attributes.Add("data-toggle", "modal");
                        //                attributes.Add("data-target", "#dlgRowAction_" + rowActionIndex);
                        //            }

                        //            var tagBuilder = new TagBuilder("a");
                        //            tagBuilder.MergeAttribute("href", href ?? "javascript:void(0)");
                        //            tagBuilder.InnerHtml = action.Text;
                        //            tagBuilder.MergeAttributes(attributes);
                        //            sb.Append(tagBuilder.ToString(TagRenderMode.Normal));
                        //        }
                        //        sb.Append("</td>");
                        //    }

                        //    sb.Append("</tr>");
                        //    sb.Append("</table>");

                        //    jsonObject.Add("_RowActions", sb.ToString());
                        //}
                        //else
                        //{
                        //    jsonObject.Add("_RowActions", null);
                        //}

                        if (needWriteValueDelimiter)
                        {
                            writer.WriteRaw(",");
                        }

                        writer.WriteRaw(jsonObject.ToString());
                        needWriteValueDelimiter = true;
                    }

                    writer.WriteEndArray();

                    writer.WriteEndObject();
                    writer.Flush();
                }

                return true;
            }
            return base.OverrideExecuteResult(context);
        }

        public override string GenerateControlFormUI(ControllerContext controllerContext)
        {
            var viewData = controllerContext.Controller.ViewData;
            viewData.Model = null;

            var viewContext = new ViewContext
            {
                HttpContext = controllerContext.HttpContext,
                Controller = controllerContext.Controller,
                RequestContext = controllerContext.RequestContext,
                ClientValidationEnabled = false,
                ViewData = viewData
            };

            var htmlHelper = new HtmlHelper<TModel>(viewContext, new ViewDataContainer(viewData));
            if (FormProvider == null)
            {
                FormProvider = ControlFormProvider.DefaultFormProvider();
            }
            var formProvider = FormProvider;

            // Start div container
            formProvider.WriteToOutput("<div class=\"control-grid-container\">");

            using (BeginForm(htmlHelper, UpdateActionName, IsAjaxSupported))
            {
                if (actions.Count > 0)
                {
                    formProvider.WriteActions(actions);
                }

                formProvider.WriteToOutput("<div class=\"box\">");

                formProvider.WriteToOutput(string.Format("<div class=\"box-header\"><h5>{0}</h5></div>", Title));
                formProvider.WriteToOutput("<div class=\"box-content nopadding\">");

                formProvider.WriteToOutput(string.Format("<table class=\"{0}\" id=\"{1}\">", CssClass, ClientId));

                formProvider.WriteToOutput("<tbody>");

                foreach (var item in items)
                {
                    formProvider.WriteToOutput("<tr class=\"table-row\">");

                    string result = RenderRazorViewToString(controllerContext, TemplateView, item);
                    formProvider.WriteToOutput(result);

                    formProvider.WriteToOutput("</tr>");
                }

                formProvider.WriteToOutput("</tbody>");

                formProvider.WriteToOutput("</table>");

                #region Ajax Stuff

                if (EnableAjaxSource)
                {
                    var dataTableOptions = new JObject
                    {
                        {"bPaginate", EnablePaginate},
                        {"bInfo", EnablePaginate},
                        {"bLengthChange", EnablePageSizeChange},
                        {"iDisplayLength", DefaultPageSize},
                        {"bSort", EnableSorting},
                        {"bSortCellsTop", true},
                        {"bAutoWidth", false}
                    };

                    if (EnablePaginate || EnableSearch)
                    {
                        dataTableOptions.Add("sDom", "lrt<'ui-toolbar ui-widget-header ui-corner-bl ui-corner-br ui-helper-clearfix'fip>");
                    }
                    else
                    {
                        dataTableOptions.Add("sDom", "lrt");
                    }

                    if (EnablePaginate)
                    {
                        dataTableOptions.Add("sPaginationType", "bootstrap");
                    }

                    var aoColumnDefs = new JArray
                    {
                        new JObject(
                            new JProperty("aTargets", new JArray("disabled-sortable")),
                            new JProperty("bSortable", false))
                    };

                    //var columnIndex = 0;

                    //foreach (var column in columns)
                    //{
                    //    if (column.PropertyName.Contains("."))
                    //    {
                    //        aoColumnDefs.Add(new JObject(
                    //            new JProperty("aTargets", new JArray(columnIndex)),
                    //            new JProperty("mData", column.PropertyName),
                    //            new JProperty("sDefaultContent", ""),
                    //            new JProperty("mRender", new JRaw(string.Format("function(data, type, full){{ return full['{0}']; }}", column.PropertyName)))));
                    //    }
                    //    else
                    //    {
                    //        aoColumnDefs.Add(new JObject(
                    //            new JProperty("aTargets", new JArray(columnIndex)),
                    //            new JProperty("mData", column.PropertyName)));
                    //    }

                    //    columnIndex++;
                    //}

                    //if (rowActions.Count > 0)
                    //{
                    //    if (HideActionsColumn)
                    //    {
                    //        aoColumnDefs.Add(new JObject(
                    //        new JProperty("aTargets", new JArray(columnIndex)),
                    //        new JProperty("mData", "_RowActions"),
                    //        new JProperty("bVisible", false)));
                    //    }
                    //    else
                    //    {
                    //        aoColumnDefs.Add(new JObject(
                    //            new JProperty("aTargets", new JArray(columnIndex)),
                    //            new JProperty("mData", "_RowActions")));
                    //    }
                    //}

                    dataTableOptions.Add("aoColumnDefs", aoColumnDefs);

                    dataTableOptions.Add("aaSorting", new JArray());

                    if (!EnableSearch)
                    {
                        dataTableOptions.Add("bFilter", false);
                    }

                    //                    dataTableOptions.Add("fnServerParams", new JRaw(string.Format(
                    //                        @"function (aoData) {{
                    //                            aoData.push( {{ ""name"": ""searchingColumns"", ""value"": ""{0}"" }} );
                    //                            aoData.push( {{ ""name"": ""filteringColumns"", ""value"": ""{1}"" }} );
                    //                            {2}
                    //                        }}", string.Join(",", columns.Where(x => x.Searchable).Select(x => x.PropertyName)),
                    //                           string.Join(",", columns.Where(x => x.Filterable).Select(x => x.PropertyName)),
                    //                           string.Join("", customrVars.Select(x => string.Format(@"aoData.push( {{ ""name"": ""{0}"", ""value"": {1} }} );", x.Key, x.Value))))));

                    dataTableOptions.Add("bProcessing", true);
                    dataTableOptions.Add("sServerMethod", "POST");
                    dataTableOptions.Add("bServerSide", true);
                    dataTableOptions.Add("sAjaxSource", controllerContext.HttpContext.Request.RawUrl);

                    formProvider.WriteToOutput("<script type=\"text/javascript\">");
                    formProvider.WriteToOutput(string.Format("var {0};", ClientId));
                    formProvider.WriteToOutput(string.Format("$(document).ready(function(){{ " +
                        "{0} = $('#{0}').dataTable({1}); window.{0} = {0};" +
                        "{0}.fnSetFilteringDelay();" +
                        "$('body').bind('SystemMessageEvent', function(event){{ doSystemMessages_{0}(event.SystemMessage); }}); " +
                        "}});", ClientId, dataTableOptions.ToString(Formatting.None)));

                    if (attachedSystemMessages.Count > 0)
                    {
                        formProvider.WriteToOutput(string.Format("function doSystemMessages_{0}(message){{", ClientId));
                        formProvider.WriteToOutput(string.Format("var dataTable = $('#{0}').dataTable();", ClientId));
                        formProvider.WriteToOutput("switch(message){");

                        foreach (var message in attachedSystemMessages)
                        {
                            formProvider.WriteToOutput(string.Format("case '{0}':", message.Key));
                            formProvider.WriteToOutput(message.Value);
                            formProvider.WriteToOutput("break;");
                        }

                        formProvider.WriteToOutput("}}");
                    }

                    //                    if (columns.Any(x => x.Filterable))
                    //                    {
                    //                        // Hook column filter
                    //                        formProvider.WriteToOutput(
                    //@"var delay = (function(){
                    //    var timer = 0;
                    //    return function(callback, ms) {
                    //        clearTimeout (timer);
                    //        timer = setTimeout(callback, ms);
                    //    };
                    //})();");
                    //                        formProvider.WriteToOutput(string.Format(
                    //                            "$('#{0} thead input').keyup(function(){{ var $this = this; delay(function(){{ var value = $this.value; {0}.fnFilter(value, {0}.oApi._fnVisibleToColumnIndex({0}.fnSettings(), $('#{0} thead input').index($this))); }}, 300); }});",
                    //                            ClientId));
                    //                    }

                    formProvider.WriteToOutput("</script>");
                }

                #endregion Ajax Stuff

                formProvider.WriteToOutput("</div></div>");

                // Hidden values
                foreach (var hiddenValue in HiddenValues)
                {
                    formProvider.WriteToOutput(string.Format("<input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"{1}\"/>", hiddenValue.Key, HttpUtility.HtmlEncode(hiddenValue.Value)));
                }
            }

            // End div container
            formProvider.WriteToOutput("</div>");

            return formProvider.GetHtmlString();
        }

        #endregion Public Methods

        private string RenderRazorViewToString(ControllerContext controllerContext, string viewName, object model)
        {
            ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controllerContext, viewName);
                var viewContext = new ViewContext(controllerContext, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(controllerContext, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }

        #region Private Methods

        private static IDisposable BeginForm(HtmlHelper htmlHelper, string actionName, bool supportAjax)
        {
            if (string.IsNullOrEmpty(actionName))
            {
                return new EmptyForm();
            }

            return supportAjax
                ? htmlHelper.BeginAjaxForm(actionName, null, new { style = "margin: 0;" })
                : htmlHelper.BeginForm(actionName, null, FormMethod.Post, new { style = "margin: 0;" });
        }

        #endregion Private Methods

        #region Nested Types

        public class ControlGridFormColumn
        {
            internal string HeaderText { get; set; }

            internal string DisplayFormat { get; set; }

            internal bool IsCheckbox { get; set; }

            public string PropertyName { get; set; }

            internal string Width { get; set; }

            internal bool Sortable { get; set; }

            internal bool Filterable { get; set; }

            internal bool Searchable { get; set; }

            internal bool IsNoWrapText { get; set; }

            internal Func<TModel, string> HtmlBuilder { get; private set; }

            internal Func<TModel, dynamic> ValueGetter { get; private set; }

            internal ControlGridFormColumn()
            {
                Sortable = true;
            }

            internal void SetValueGetter<TValue>(Func<TModel, TValue> expression)
            {
                ValueGetter = model => expression(model);

                HtmlBuilder = model =>
                {
                    var value = ValueGetter(model);
                    if (value == null)
                    {
                        return string.Empty;
                    }

                    var formattedValue = !string.IsNullOrEmpty(DisplayFormat) ? string.Format(DisplayFormat, value) : Convert.ToString(value);
                    if (IsNoWrapText)
                    {
                        formattedValue = string.Format("<span style=\"white-space: nowrap;\">{0}</span>", formattedValue);
                    }
                    return formattedValue;
                };
            }

            public ControlGridFormColumn HasDisplayFormat(string value)
            {
                DisplayFormat = value;
                return this;
            }

            public ControlGridFormColumn HasHeaderText(string value)
            {
                HeaderText = value;
                return this;
            }

            public ControlGridFormColumn EnableFilter(bool value = true)
            {
                Filterable = value;
                return this;
            }

            public ControlGridFormColumn RenderAsCheckbox(Func<TModel, bool> isChecked = null)
            {
                IsCheckbox = true;
                HtmlBuilder = model =>
                {
                    var value = ValueGetter(model);
                    var checkedValue = isChecked != null && isChecked(model);
                    if (checkedValue)
                    {
                        return string.Format("<input type=\"checkbox\" checked=\"checked\" name=\"{0}\" value=\"{1}\">", PropertyName, value);
                    }
                    return string.Format("<input type=\"checkbox\" name=\"{0}\" value=\"{1}\">", PropertyName, value);
                };
                return this;
            }

            public ControlGridFormColumn HasWidth(string value)
            {
                Width = value;
                return this;
            }

            public ControlGridFormColumn RenderAsHtml(Func<TModel, string> builder)
            {
                HtmlBuilder = builder;
                return this;
            }

            public ControlGridFormColumn RenderAsLink(Func<TModel, string> href, bool targetBlank = true)
            {
                if (targetBlank)
                {
                    HtmlBuilder = model => string.Format("<a href=\"{0}\" target=\"_blank\">{0}</a>", href(model));
                }
                else
                {
                    HtmlBuilder = model => string.Format("<a href=\"{0}\">{0}</a>", href(model));
                }
                return this;
            }

            public ControlGridFormColumn RenderAsLink(Func<TModel, string> text, Func<TModel, string> href, bool targetBlank = true)
            {
                if (targetBlank)
                {
                    HtmlBuilder = model => string.Format("<a href=\"{0}\" target=\"_blank\">{1}</a>", href(model), text(model));
                }
                else
                {
                    HtmlBuilder = model => string.Format("<a href=\"{0}\">{1}</a>", href(model), text(model));
                }
                return this;
            }

            public ControlGridFormColumn RenderAsImage(Func<TModel, string> src)
            {
                HtmlBuilder = model => string.Format("<img src=\"{0}\" />", src(model));
                return this;
            }

            public ControlGridFormColumn RenderAsStatusImage(string trueImageSrc, string falseImageSrc)
            {
                HtmlBuilder = model =>
                {
                    var value = Convert.ToBoolean(ValueGetter(model));
                    return string.Format("<img src=\"{0}\" />", value ? trueImageSrc : falseImageSrc);
                };
                return this;
            }

            public ControlGridFormColumn RenderAsStatusImage(bool reverse = false, bool showTrueOnly = false)
            {
                const string success =
                    "data:image/gif;base64,R0lGODlhEAAQAOYAAARsE5zgd0/AJtfsyi6wFoS1hACZALLWsmbIQx+oCwCICDmMO7/ov/r2/Pnq+gBtJjq1HIXMeqfJqu/37xaFF9Hh0XnTTCydJ2bMMw2jAKflg5Xcd0CmQRNzGVC+O+rm7EW7IgByMojYXi6gKgCFEiWjF9/v2wB9IRyUHGzGU4zFjDq1IQBmMweeAPXw9gCND4i/jiqqIBupCH7TWQB8Kf///1rFLEe8ISetC+nz3C+oJBF6GCStEUK1IbjfuIa7hjGoKB6RHXPKVgBsOpvde1TDKTqUOkKrQo/LjwiODk3AJwuNCCmlKTOvHiKqCf/3/0C5G5bbeABxNonYYQCGGxmtCD60K////wAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAACH5BAUUAFcALAAAAAAQABAAAAe5gFeCg4SFhkhHIyMcKoaCDExCATMIHjEoPoUMQBoiGEU3EAQJS5mDTBoWGJ8gEAkRPBQTgkgpU6s2SiBOJk8DST+CHBs2IEUCIDgmNTVRLwuCIzMlMDxQysxELVQ7ghceEg4VTcs12gY03Vcclh8uT0/mLS0GUtBXKjoyVi4N8vQnHhQYFERGBg85zhkgMaTDLEE+lszLYKDiiSEADhTyQUHBixcnQjzooNHQhB8LdnRYUOCho5eDAgEAOw==";
                const string block =
                    "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAADSklEQVQ4jV2QS0xcZQCFv//eO3eGeTGDIHQYyqNNWtoi0sZHQjSxSdPU0ocNlVhNF7LvwsSdJhrjwoUmNLFxpYs2xhBgUQxpQkxNGjGxpQVaHg4CIhMeHcrAMHfmzn39Lui4cH3yfTnniD5VASCs+xAliy88WfeVqryTaG68Gqqp7gDIZzan0kvLNz92vcFPFLFq+3W2LZuM6yH+J3j/1ZNv3Dp04SJay0GIhAHAMHAWF5gbGuK3u/d6bb/+fVmgnlEEALqqfnb2g57rTV1dCD2AsbTA0oP7PJ15Ajs5gpEItS93kKyKXph5MhMyXW+0IOV/DU6f7Tp9J3G4FcM0+ePBOMMTj9emjcJwAZxXKkPne08cTzYkk8TqalmenaZ/5JfujOsNajtScqyl/o5P9bGYmuf+1CO+WUhf7gwFBwDuxWNM+gMnZMFIGtks29tZglqAhsb6gcziitCAt17wB5lJzWO5Dn0L6cvAwDlp81EoADBRnahuN3M5Zv0BVmybA1tbNOg6vyrikqbrWs/u6hprwJLjGcDA1eDeL8DElOa1y79SpKsquT6e/iHu2G+f0tTaZs2Hp6lXlB1FtP5tlti0LFZKpZ/bdK0MP0yrsr20u8uyX+fLp9v9wIdxV95el5JnpklAcEjLS+ycIkg5NtKVhEM6OO5DQ3gdwnYoVQS4kTf7EaLnoOuCpobSmkbItdgBTzFd71HCpxJzPOpi0XeB36Ou7MB18RSVH41SP9DT6SmojkQGK65EPY+oqpBxvUnFsN1bnqLSpgiadU2cVNXXk8B+FEaK7k9Az7V99eVZ1w5XBmm3SvhVlTlX3lSAyXHTWn8pUkEi84xwqciLiX2MOsrQ0WjkvTPhWBn+9LWW/X1tm1mO6D7GLNcBRsWbe2Frdzw8c1Hx+Md1cerqSeUM7EJxOqhoIhKLHKkK+glmNqgplRjx4POc2QmMlS+f/XY7f0mLR4bOR0IIz+R4Uz16LHpUqDq2uUthdYNiQGPYdPh61+gFxgDUxucGQzB3t2gNpG33XFUoWFkTjRGIxUHzkc/lGd/KcWN9K/udYZ7yC25bz7nyBDYEFCRIqLWg6YCgO+HzHZPAmmWn5mEwAH8WYSMiIC/3uH8BnK93p318sLgAAAAASUVORK5CYII=";

                HtmlBuilder = model =>
                {
                    var value = Convert.ToBoolean(ValueGetter(model));
                    if (showTrueOnly)
                    {
                        var trueImageSrc = reverse ? block : success;
                        return value ? string.Format("<img src=\"{0}\" />", trueImageSrc) : null;
                    }
                    else
                    {
                        var trueImageSrc = reverse ? block : success;
                        var falseImageSrc = reverse ? success : block;
                        return string.Format("<img src=\"{0}\" />", value ? trueImageSrc : falseImageSrc);
                    }
                };
                return this;
            }

            public ControlGridFormColumn EnableSorting(bool value = true)
            {
                Sortable = value;
                return this;
            }

            public ControlGridFormColumn NoWrapText()
            {
                IsNoWrapText = true;
                return this;
            }
        }

        private class EmptyForm : IDisposable
        {
            public void Dispose()
            {
            }
        }

        #endregion Nested Types
    }
}