using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CMSSolutions.Data;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.UI.JQueryBuilder;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlGridFormResult<TModel> : BaseControlFormResult
    {
        #region Private Members

        private readonly IList<ControlGridFormColumn<TModel>> columns;
        private readonly IList<ControlFormAction> actions;
        private readonly IList<ControlGridFormRowAction<TModel>> rowActions;
        private readonly IList<string> reloadEvents;
        private readonly IDictionary<string, string> customrVars;
        private ControlSubGridForm subGridForm;
        private string clientId;
        private bool enableTreeGrid;
        private Func<TModel, dynamic> treeGridParentId;
        private Func<TModel, bool> treeGridHasChildren;

        #endregion Private Members

        #region Constructors

        public ControlGridFormResult()
        {
            columns = new List<ControlGridFormColumn<TModel>>();
            actions = new List<ControlFormAction>();
            rowActions = new List<ControlGridFormRowAction<TModel>>();
            reloadEvents = new List<string>();
            customrVars = new Dictionary<string, string>();
            IsAjaxSupported = true;
            DefaultPageSize = 10;
            EnableSorting = true;
            ActionsHeaderText = "Actions";
            RecordsInfoPosition = "right";

            if (typeof(BaseEntity).IsAssignableFrom(typeof(TModel)))
            {
                // ReSharper disable PossibleNullReferenceException
                GetModelId = model => (model as BaseEntity).GetIdValue();   
                // ReSharper restore PossibleNullReferenceException
            }
            else
            {
                GetModelId = model => model.GetHashCode();    
            }
            IconHeader = "fa fa-lg fa-fw fa-table";
            ShowCaption = false;
        }

        #endregion Constructors

        #region Properties

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

        public IList<ControlFormAction> Actions { get { return actions; } }

        public IList<ControlGridFormRowAction<TModel>> RowActions { get { return rowActions; } }

        public int? ActionsColumnWidth { get; set; }

        public IList<string> ReloadEvents { get { return reloadEvents; } }

        public string CssClass { get; set; }

        public string UpdateActionName { get; set; }

        public bool IsAjaxSupported { get; set; }

        public bool EnableSearch { get; set; }

        public bool EnableSorting { get; set; }

        public Func<ControlGridFormRequest, ControlGridAjaxData<TModel>> FetchAjaxSource { get; set; }

        public bool EnablePaginate { get; set; }

        public bool EnablePageSizeChange { get; set; }

        public int DefaultPageSize { get; set; }

        public IList<ControlGridFormColumn<TModel>> Columns { get { return columns; } }

        public bool HideActionsColumn { get; set; }

        public Func<TModel, object> GetModelId { get; set; }

        public string ActionsHeaderText { get; set; }

        public JArray RowsList { get; set; }

        public bool ShowFooterRow { get; set; }

        public bool EnableShowHideGrid { get; set; }

        public bool EnableCheckboxes { get; set; }

        public string RecordsInfoPosition { get; set; }

        public bool HidePagerWhenEmpty { get; set; }

        public string GridWrapperStartHtml { get; set; }

        public string GridWrapperEndHtml { get; set; }

        public string GetRecordsUrl { get; set; }

        public string IconHeader { get; set; }

        public bool ShowCaption { get; set; }

        #endregion Properties

        #region Public Methods

        #region Sub Grid

        public ControlSubGridForm<TSubModel, TModelKey> EnableSubGrid<TSubModel, TModelKey>()
        {
            if (enableTreeGrid)
            {
                throw new ArgumentException("Cannot enable both tree grid and sub grid.");
            }

            if (subGridForm != null)
            {
                throw new ArgumentException("Sub Grid already enabled.");
            }

            var controlSubGridForm = new ControlSubGridForm<TSubModel, TModelKey>();
            subGridForm = controlSubGridForm;
            return controlSubGridForm;
        }

        #endregion Sub Grid

        #region Tree Grid

        public void EnableTreeGrid<TValue>(Expression<Func<TModel, TValue>> expression, Func<TModel, bool> hasChildren = null)
        {
            if (subGridForm != null)
            {
                throw new ArgumentException("Cannot enable both tree grid and sub grid.");
            }

            enableTreeGrid = true;
            var func = expression.Compile();
            treeGridParentId = x => func(x);

            if (hasChildren == null)
            {
                treeGridHasChildren = model => true;
            }
            else
            {
                treeGridHasChildren = hasChildren;
            }
        }

        #endregion Tree Grid

        public ControlGridFormColumn<TModel> AddColumn<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            return AddColumn(expression, null);
        }

        public ControlGridFormColumn<TModel> AddColumn<TValue>(Expression<Func<TModel, TValue>> expression, string headerText)
        {
            var column = new ControlGridFormColumn<TModel>();
            column.SetValueGetter(expression.Compile());

            if (!string.IsNullOrEmpty(headerText))
            {
                column.HeaderText = headerText;
            }
            else
            {
                var modelMetadata = ModelMetadata.FromLambdaExpression(expression, new ViewDataDictionary<TModel>());
                column.HeaderText = modelMetadata.DisplayName ?? modelMetadata.PropertyName;
            }
            column.PropertyName = Utils.GetFullPropertyName(expression);
            column.PropertyType = typeof (TValue);

            columns.Add(column);
            return column;
        }

        public ControlGridFormColumn<TModel> AddColumn(string columnName)
        {
            var column = new ControlGridFormColumn<TModel> { PropertyName = columnName };
            columns.Add(column);
            return column;
        }

        public ControlFormAction AddAction(bool isSubmitButton = false, bool isAjaxSupport = true)
        {
            var action = new ControlFormAction(isSubmitButton, isAjaxSupport);
            actions.Add(action);
            return action;
        }

        public ControlFormAction AddAction(ControlFormAction action)
        {
            actions.Add(action);
            return action;
        }

        public ControlGridFormRowAction<TModel> AddRowAction(bool isSubmitButton = false, bool isAjaxSupport = true)
        {
            var action = new ControlGridFormRowAction<TModel>(isSubmitButton, isAjaxSupport);
            rowActions.Add(action);
            return action;
        }

        public void AddRowAction(ControlGridFormRowAction<TModel> action)
        {
            rowActions.Add(action);
        }

        public void AddReloadEvent(string eventName)
        {
            reloadEvents.Add(eventName);
        }

        public void AddCustomVar(string name, object value, bool isFunction = false)
        {
            if (value == null)
            {
                throw new NullReferenceException();
            }

            if (isFunction || !(value is string))
            {
                if (isFunction)
                {
                    customrVars.Add(name, "function(){ return " + value + " }");
                }
                else
                {
                    customrVars.Add(name, value.ToString());
                }
            }
            else
            {
                customrVars.Add(name, JQueryUtility.EncodeJsString(value.ToString()));
            }
        }

        public override IEnumerable<ResourceType> GetAdditionalResources(ControllerContext context)
        {
            yield return ResourceType.JQueryUI;
            yield return ResourceType.JQueryValidate;
            yield return ResourceType.FancyBox;
        }

        public override bool OverrideExecuteResult(ControllerContext context)
        {
            if (context.HttpContext.Request.IsAjaxRequest())
            {
                var formProvider = FormProvider ?? ControlFormProvider.DefaultFormProvider();

                // Return data only
                var request = ControlGridFormRequest.Create(context.HttpContext.Request);
                if (request.PageSize <= 0)
                {
                    request.PageSize = DefaultPageSize;
                }

                var response = context.HttpContext.Response;
                response.ContentType = "application/json";
                response.ContentEncoding = Encoding.UTF8;

                if (context.HttpContext.Request.QueryString["subGrid"] == "1")
                {
                    var id = context.HttpContext.Request.Form["id"];
                    var data = subGridForm.GetSubGridData(id);
                    WriteJsonData(response, request, data,
                        data.Count(),
                        formProvider,
                        subGridForm.Columns,
                        subGridForm.GetRowActions(),
                        x => subGridForm.GetModelId(x), true, false, null, null);
                }
                else
                {
                    var data = FetchAjaxSource(request);
                    WriteJsonData(response, request, data,
                        data.TotalRecords > 0 ? data.TotalRecords : data.Count,
                        formProvider,
                        columns.Select(x => (ControlGridFormColumn)x).ToList(),
                        rowActions.Count > 0 && !HideActionsColumn ? rowActions.Select(x => (IControlGridFormRowAction)x).ToList() : new List<IControlGridFormRowAction>(),
                        GetModelId, false, enableTreeGrid, treeGridParentId, treeGridHasChildren);
                }

                return true;
            }
            return base.OverrideExecuteResult(context);
        }

        private static void WriteJsonData<TModelRecord>(HttpResponseBase response, ControlGridFormRequest request, ControlGridAjaxData<TModelRecord> data, int totalRecords,
            ControlFormProvider formProvider, IList<ControlGridFormColumn> columns, ICollection<IControlGridFormRowAction> rowActions, Func<TModelRecord, object> getModelId, bool isSubGrid, bool isTreeGrid, Func<TModelRecord, dynamic> getParentId, Func<TModelRecord, bool> hasChildren)
        {
            using (var writer = new JsonTextWriter(response.Output) { Formatting = Formatting.None })
            {
                writer.WriteStartObject();

                writer.WritePropertyName("page");
                writer.WriteValue(request.PageIndex);

                writer.WritePropertyName("records");
                writer.WriteValue(totalRecords);

                writer.WritePropertyName("total");
                writer.WriteValue((int)Math.Ceiling((totalRecords * 1d) / request.PageSize));
                
                if (data.Callbacks.Count > 0)
                {
                    writer.WritePropertyName("callback");
                    writer.WriteValue(string.Join("", data.Callbacks));    
                }

                writer.WritePropertyName("rows");

                writer.WriteStartArray();

                var needWriteValueDelimiter = false;
                foreach (TModelRecord item in data)
                {
                    var jsonObject = new JObject { { "_id", Convert.ToString(getModelId(item)) } };

                    foreach (var column in columns)
                    {
                        jsonObject.Add(column.PropertyName, Convert.ToString(column.BuildHtml(item)));
                    }

                    if (isTreeGrid)
                    {
                        jsonObject.Add("_level", request.NodeLevel + 1);
                        jsonObject.Add("_parentId", getParentId(item));
                        jsonObject.Add("_isLeaf", !hasChildren(item));
                        jsonObject.Add("_isExpanded", false);
                    }

                    if (rowActions.Count > 0)
                    {
                        var sb = new StringBuilder();
                        sb.Append("<table style=\"margin-left: auto; margin-right: auto; border: none; padding: 0;\">");
                        sb.Append("<tr>");

                        foreach (var action in rowActions)
                        {
                            if (!action.IsVisible(item))
                            {
                                continue;
                            }

                            var enabled = action.IsEnable(item);
                            var attributes = new RouteValueDictionary(action.GetAttributes(item));

                            sb.Append("<td style=\"border: none; background-color: transparent; padding: 0 2px;\">");
                            if (action.IsSubmitButton)
                            {
                                var value = action.GetValue(item);

                                var cssClass =
                                    (formProvider.GetButtonSizeCssClass(action.ButtonSize) + " " +
                                     formProvider.GetButtonStyleCssClass(action.ButtonStyle) + " " + action.CssClass).Trim();

                                if (!string.IsNullOrEmpty(cssClass))
                                {
                                    attributes.Add("class", cssClass);
                                }

                                if (!enabled)
                                {
                                    attributes.Add("disabled", "disabled");
                                }

                                if (!string.IsNullOrEmpty(action.ClientClickCode))
                                {
                                    attributes.Add("onclick", action.ClientClickCode);
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(action.ConfirmMessage))
                                    {
                                        attributes.Add("onclick", string.Format("return confirm('{0}');", action.ConfirmMessage));
                                    }
                                }

                                attributes.Add("id", "btn" + Guid.NewGuid().ToString("N").ToLowerInvariant());
                                attributes.Add("style", "white-space: nowrap;");

                                var tagBuilder = new TagBuilder("button");
                                tagBuilder.MergeAttribute("type", "submit");
                                tagBuilder.MergeAttribute("name", action.Name);
                                tagBuilder.InnerHtml = action.Text;
                                tagBuilder.MergeAttribute("value", Convert.ToString(value));
                                tagBuilder.MergeAttributes(attributes, true);
                                sb.Append(tagBuilder.ToString(TagRenderMode.Normal));
                            }
                            else
                            {
                                var href = action.GetUrl(item);

                                var cssClass =
                                    (formProvider.GetButtonSizeCssClass(action.ButtonSize) + " " +
                                     formProvider.GetButtonStyleCssClass(action.ButtonStyle) + " " + action.CssClass).Trim();

                                if (!string.IsNullOrEmpty(cssClass))
                                {
                                    if (enabled)
                                    {
                                        attributes.Add("class", cssClass);
                                    }
                                    else
                                    {
                                        attributes.Add("class", cssClass + " disabled");
                                    }
                                }
                                else
                                {
                                    if (!enabled)
                                    {
                                        attributes.Add("class", "disabled");
                                    }
                                }

                                attributes.Add("style", "white-space: nowrap;");

                                if (action.IsShowModalDialog && enabled)
                                {
                                    attributes.Add("data-toggle", "fancybox");
                                    attributes.Add("data-fancybox-type", "iframe");
                                    attributes.Add("data-fancybox-width", action.ModalDialogWidth);
                                    attributes.Add("data-fancybox-height", action.ModalDialogHeight);
                                }

                                var tagBuilder = new TagBuilder("a");
                                if (enabled)
                                {
                                    tagBuilder.MergeAttribute("href", href ?? "javascript:void(0)");
                                }
                                else
                                {
                                    tagBuilder.MergeAttribute("href", "javascript:void(0)");
                                }
                                tagBuilder.InnerHtml = action.Text;
                                tagBuilder.MergeAttributes(attributes, true);
                                sb.Append(tagBuilder.ToString(TagRenderMode.Normal));
                            }
                            sb.Append("</td>");
                        }

                        sb.Append("</tr>");
                        sb.Append("</table>");

                        jsonObject.Add("_RowActions", sb.ToString());
                    }
                    else
                    {
                        jsonObject.Add("_RowActions", null);
                    }

                    if (needWriteValueDelimiter)
                    {
                        writer.WriteRaw(",");
                    }

                    writer.WriteRaw(jsonObject.ToString());
                    needWriteValueDelimiter = true;
                }

                if (isSubGrid && data.UserData.ContainsKey("_RowActions"))
                {
                    if (needWriteValueDelimiter)
                    {
                        writer.WriteRaw(",");
                    }

                    writer.WriteStartObject();
                    writer.WritePropertyName("_RowActions");
                    writer.WriteValue(data.UserData["_RowActions"]);
                    writer.WriteEndObject();

                    data.UserData.Remove("_RowActions");
                }

                writer.WriteEndArray();

                if (data.UserData.Count > 0)
                {
                    writer.WritePropertyName("userdata");

                    writer.WriteStartObject();

                    foreach (var item in data.UserData)
                    {
                        writer.WritePropertyName(item.Key);
                        writer.WriteValue(item.Value);
                    }

                    writer.WriteEndObject();
                }

                writer.WriteEndObject();
                writer.Flush();
            }
        }

        public override string GenerateControlFormUI(ControllerContext controllerContext)
        {
            var viewData = controllerContext.Controller.ViewData;
            viewData.Model = null;

            if (FormProvider == null)
            {
                FormProvider = ControlFormProvider.DefaultFormProvider();
            }
            var formProvider = FormProvider;

            var viewContext = new ViewContext
            {
                HttpContext = controllerContext.HttpContext,
                Controller = controllerContext.Controller,
                RequestContext = controllerContext.RequestContext,
                ClientValidationEnabled = false,
                Writer = new FormProviderTextWriter(formProvider),
                ViewData = viewData
            };

            var htmlHelper = new HtmlHelper<TModel>(viewContext, new ViewDataContainer(viewData));

            if (!string.IsNullOrEmpty(GridWrapperStartHtml))
            {
                formProvider.WriteToOutput(string.Format(GridWrapperStartHtml, Title, IconHeader));
            }

            // Start div container
            formProvider.WriteToOutput(string.Format("<div class=\"control-grid-container\" id=\"{0}_Container\">", ClientId));

            using (BeginForm(htmlHelper, UpdateActionName, IsAjaxSupported))
            {
                formProvider.WriteActions(actions);

                formProvider.WriteToOutput(string.Format("<table class=\"{0}\" id=\"{1}\"></table>", CssClass, ClientId));

                formProvider.WriteToOutput(string.Format("<div id=\"{0}_Pager\"></div>", ClientId));

                // Hidden values
                foreach (var hiddenValue in HiddenValues)
                {
                    formProvider.WriteToOutput(string.Format("<input type=\"hidden\" name=\"{0}\" id=\"{0}\" value=\"{1}\"/>", hiddenValue.Key, HttpUtility.HtmlEncode(hiddenValue.Value)));
                }

                #region jqGrid

                var dataTableOptions = new JObject
                    {
                        {"rowNum", EnablePaginate ? DefaultPageSize : int.MaxValue},
                        {"autowidth", true},
                        {"viewrecords", true},
                        {"loadonce", false},
                        {"userDataOnFooter", true},
                        {"hidegrid", EnableShowHideGrid},
                        {"height", "100%"},
                        {"recordpos", RecordsInfoPosition},
                        {"multiselect", EnableCheckboxes},
                        {"loadComplete", new JRaw(string.Format("function(data){{ if({1} && data.records === 0) {{ $('#{0}_Pager_center').hide(); }} else {{ $('#{0}_Pager_center').show(); }} var width = $('#{0}_Container').width(); $('#{0}').setGridWidth(width); if(data.callback){{ eval(data.callback); }} }}", ClientId, HidePagerWhenEmpty ? "true" : "false"))}
                    };

                if (ShowCaption)
                {
                    dataTableOptions.Add(new JObject {"caption", Title});
                }

                if (EnablePaginate)
                {
                    dataTableOptions.Add("pager", string.Format("#{0}_Pager", ClientId));
                }

                if (RowsList != null)
                {
                    dataTableOptions.Add("rowList", RowsList);
                }

                if (ShowFooterRow)
                {
                    dataTableOptions.Add("footerrow", true);
                }

                var colNames = new JArray();
                var colModel = new JArray();

                foreach (var column in columns)
                {
                    colNames.Add(column.HeaderText);
                    var options = new JObject(
                        new JProperty("name", column.PropertyName),
                        new JProperty("index", column.PropertyName),
                        new JProperty("align", column.Align),
                        new JProperty("sortable", EnableSorting && column.Sortable));

                    if (column.Width.HasValue)
                    {
                        options.Add(new JProperty("width", column.Width));
                        options.Add(new JProperty("fixed", true));
                    }

                    if (!string.IsNullOrEmpty(column.CssClass))
                    {
                        options.Add(new JProperty("classes", column.CssClass));
                    }

                    if (column.Filterable)
                    {
                        options.Add(new JProperty("search", true));
                        var typeCode = Type.GetTypeCode(column.PropertyType);

                        switch (typeCode)
                        {
                            case TypeCode.Boolean:
                                options.Add(new JProperty("stype", "select"));
                                options.Add(new JProperty("editoptions", new JObject(new JProperty("value", ":All;true:Yes;false:No"))));
                            break;
                            case TypeCode.String:
                                options.Add(new JProperty("stype", "text"));
                                options.Add(new JProperty("searchoptions", new JObject(new JProperty("sopt", new JArray("cn")))));
                            break;
                            default:
                                throw new NotSupportedException();
                        }
                    }
                    else
                    {
                        options.Add(new JProperty("search", false));
                    }

                    colModel.Add(options);
                }

                if (rowActions.Count > 0)
                {
                    if (!HideActionsColumn)
                    {
                        colNames.Add(ActionsHeaderText);
                        var options = new JObject(
                            new JProperty("name", "_RowActions"),
                            new JProperty("align", "center"),
                            new JProperty("index", "_RowActions"),
                            new JProperty("cellattr", new JRaw("function(){ return 'title=\"\"'; }")),
                            new JProperty("search", false),
                            new JProperty("sortable", false));

                        if (ActionsColumnWidth.HasValue)
                        {
                            options.Add(new JProperty("width", ActionsColumnWidth.Value));
                            options.Add(new JProperty("fixed", true));
                        }

                        colModel.Add(options);
                    }
                }

                dataTableOptions.Add("colNames", colNames);
                dataTableOptions.Add("colModel", colModel);

                if (customrVars.Count > 0)
                {
                    var postData = new JObject();

                    foreach (var customrVar in customrVars)
                    {
                        postData.Add(customrVar.Key, new JRaw(customrVar.Value));
                    }

                    dataTableOptions.Add("postData", postData);
                }

                dataTableOptions.Add("datatype", "json");
                dataTableOptions.Add("jsonReader", new JObject(new JProperty("id", "_id"), new JProperty("subgrid", new JObject(new JProperty("repeatitems", false)))));
                dataTableOptions.Add("mtype", "POST");
                dataTableOptions.Add("url", string.IsNullOrEmpty(GetRecordsUrl)
                                         ? controllerContext.HttpContext.Request.RawUrl
                                         : GetRecordsUrl);

                // Sub Grid
                if (subGridForm != null)
                {
                    var subGridNames = new JArray(subGridForm.Columns.Select(x => x.HeaderText));
                    var subGridWidths = new JArray(subGridForm.Columns.Select(x => x.Width.HasValue ? x.Width.Value : 100));
                    var subGridAligns = new JArray(subGridForm.Columns.Select(x => x.Align));
                    var subGridMappings = new JArray(subGridForm.Columns.Select(x => x.PropertyName));

                    var subRowActions = subGridForm.GetRowActions();
                    if (subRowActions.Count > 0)
                    {
                        subGridNames.Add(subGridForm.ActionsColumnText);
                        subGridWidths.Add(subGridForm.ActionsColumnWidth);
                        subGridAligns.Add("center");
                        subGridMappings.Add("_RowActions");
                    }

                    var subGridModel = new JObject
                                       {
                                           {"name", subGridNames},
                                           {"width", subGridWidths},
                                           {"align", subGridAligns},
                                           {"mapping", subGridMappings}
                                       };

                    var queryString = string.Join(string.Empty, controllerContext.HttpContext.Request.RawUrl.Split('?').Skip(1));
                    var queryStrings = HttpUtility.ParseQueryString(queryString);
                    queryStrings["subGrid"] = "1";

                    dataTableOptions.Add("subGrid", true);
                    // ReSharper disable PossibleNullReferenceException
                    dataTableOptions.Add("subGridUrl", controllerContext.HttpContext.Request.Url.GetLeftPart(UriPartial.Path) + "?" + string.Join("&", queryStrings.AllKeys.Select(x => x + "=" + HttpUtility.UrlEncode(queryStrings[x]))));
                    // ReSharper restore PossibleNullReferenceException
                    dataTableOptions.Add("subGridModel", new JArray(subGridModel));

                    if (subGridForm.Width.HasValue)
                    {
                        dataTableOptions.Add("subGridWidth", subGridForm.Width.Value);
                    }

                    if (subGridForm.AjaxOptions != null)
                    {
                        dataTableOptions.Add("ajaxSubgridOptions", subGridForm.AjaxOptions);
                    }
                }

                // Tree grid
                if (enableTreeGrid)
                {
                    var treeReader = new JObject
                    {
                        { "level_field", "_level" },
                        { "parent_id_field", "_parentId" },
                        { "leaf_field", "_isLeaf" },
                        { "expanded_field", "_isExpanded" }
                    };

                    dataTableOptions.Add("treeGrid", true);
                    dataTableOptions.Add("treeGridModel", "adjacency");
                    dataTableOptions.Add("treeReader", treeReader);
                }

                var workContext = controllerContext.GetWorkContext();
                var scriptRegister = new ScriptRegister(workContext);

                scriptRegister.IncludeInline(string.Format("$(document).ready(function(){{ $('#{0}').jqGrid({1}); }});", ClientId, dataTableOptions.ToString(Formatting.None)));

                if (EnableSearch && columns.Any(x => x.Filterable))
                {
                    scriptRegister.IncludeInline(string.Format("$(document).ready(function(){{ $('#{0}').jqGrid('filterToolbar', {{ stringResult: true }}); }});", ClientId));
                }

                if (ReloadEvents.Count > 0)
                {
                    scriptRegister.IncludeInline(string.Format("$(document).ready(function(){{ $('body').bind('SystemMessageEvent', function(event){{ var events = [{1}]; if(events.indexOf(event.SystemMessage) > -1){{ $('#{0}').jqGrid().trigger('reloadGrid'); }} }}); }});", ClientId, string.Join(", ", ReloadEvents.Select(x => "'" + x + "'"))));
                }

                // Resize window event
                scriptRegister.IncludeInline(string.Format("$(document).ready(function(){{ $(window).resize(function(){{ var width = $('#{0}_Container').width(); $('#{0}').setGridWidth(width); }}); }});", ClientId));

                #endregion jqGrid
            }

            // End div container
            formProvider.WriteToOutput("</div>");

            if (!string.IsNullOrEmpty(GridWrapperEndHtml))
            {
                formProvider.WriteToOutput(GridWrapperEndHtml);
            }

            return formProvider.GetHtmlString();
        }

        #endregion Public Methods

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

        private class EmptyForm : IDisposable
        {
            public void Dispose()
            {
            }
        }

        #endregion Nested Types
    }
}