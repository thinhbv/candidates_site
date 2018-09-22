using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using CMSSolutions.Caching;
using CMSSolutions.Extensions;
using CMSSolutions.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlFormResult : ControlFormResult<dynamic>
    {
        private readonly IList<ControlFormAttribute> addedProperties;

        public ControlFormResult()
            : base(new object())
        {
            addedProperties = new List<ControlFormAttribute>();
        }

        public override void AddProperty(string name, ControlFormAttribute attribute, object value = null)
        {
            attribute.Name = name;
            attribute.Value = value;
            addedProperties.Add(attribute);
        }

        public override IEnumerable<ControlFormAttribute> GetProperties(RequestContext requestContext)
        {
            return addedProperties;
        }
    }

    public class ControlFormResult<TModel> : BaseControlFormResult where TModel : class
    {
        private readonly IDictionary<string, ControlFormAttribute> addedProperties;
        private readonly IDictionary<string, ControlAutoCompleteOptions<TModel>> autoCompleteDataSources;
        private readonly IDictionary<string, string> cascadingCheckboxDataSource;
        private readonly IDictionary<string, ControlCascadingDropDownOptions> cascadingDropDownDataSource;
        private readonly IList<string> excludedProperties;
        private readonly IDictionary<string, Func<TModel, IEnumerable<SelectListItem>>> externalDataSources;
        private readonly IDictionary<string, ControlFileUploadOptions> fileUploadOptions;
        private readonly IDictionary<string, GridLayout> gridLayouts;
        private readonly IList<ControlGroupedLayout<TModel>> groupedLayouts;
        private readonly TModel model;
        private readonly Type modelType;
        private readonly IDictionary<string, Action<dynamic>> onAttributesRenders;
        private readonly IList<string> readonlyProperties;
        private readonly IList<ControlTabbedLayout<TModel>> tabbedLayouts;

        public ControlFormResult(TModel model, Type modelType = null)
        {
            if (model == null)
            {
                throw new ArgumentNullException("model");
            }

            this.model = model;
            this.modelType = modelType ?? model.GetType();
            excludedProperties = new List<string>();
            readonlyProperties = new List<string>();
            gridLayouts = new Dictionary<string, GridLayout>();
            groupedLayouts = new List<ControlGroupedLayout<TModel>>();
            tabbedLayouts = new List<ControlTabbedLayout<TModel>>();
            externalDataSources = new Dictionary<string, Func<TModel, IEnumerable<SelectListItem>>>();
            autoCompleteDataSources = new Dictionary<string, ControlAutoCompleteOptions<TModel>>();
            cascadingDropDownDataSource = new Dictionary<string, ControlCascadingDropDownOptions>();
            cascadingCheckboxDataSource = new Dictionary<string, string>();
            fileUploadOptions = new Dictionary<string, ControlFileUploadOptions>();
            Actions = new List<ControlFormAction>();
            excludedProperties = new List<string>();
            addedProperties = new Dictionary<string, ControlFormAttribute>();
            onAttributesRenders = new Dictionary<string, Action<dynamic>>();

            ValidationSupport = true;
            SubmitButtonText = "Save";
            SubmitButtonCssClass = "btn btn-primary";
            CancelButtonText = "Cancel";
            CancelButtonCssClass = "btn btn-danger";
            ShowCancelButton = true;
            ShowSubmitButton = true;
            IsAjaxSupported = true;
            FormMethod = FormMethod.Post;
            ShowBoxHeader = true;
            IconHeader = "fa fa-lg fa-fw fa-pencil-square-o";
            IsFormCenter = false;
        }

        public IList<ControlFormAction> Actions { get; private set; }

        public string CancelButtonCssClass { get; set; }

        public string CancelButtonText { get; set; }

        public string CancelButtonIcon { get; set; }

        public string CancelButtonUrl { get; set; }

        public string ClientId { get; set; }

        public string CssClass { get; set; }

        public bool DisableBlockUI { get; set; }

        public bool EnableKnockoutJs { get; set; }

        public Func<TModel, ActionResult> FinishWizardStep { get; set; }

        public string FormActionsContainerCssClass { get; set; }

        public string FormActionsCssClass { get; set; }

        public string FormActionUrl { get; set; }

        public FormMethod FormMethod { get; set; }

        public TModel FormModel { get { return model; } }

        public string FormWrapperEndHtml { get; set; }

        public string FormWrapperStartHtml { get; set; }

        public string IconHeader { get; set; }

        public bool IsAjaxSupported { get; set; }

        public ControlFormLayout Layout { get; set; }

        public bool ShowBoxHeader { get; set; }

        public string OnClientSubmitButtonClick { get; set; }

        public bool ReadOnly { get; set; }

        public bool ShowCancelButton { get; set; }

        public bool ShowCloseButton { get; set; }

        public bool ShowSubmitButton { get; set; }

        public bool ShowValidationSummary { get; set; }

        public string SubmitButtonCssClass { get; set; }

        public string SubmitButtonText { get; set; }

        public string SubmitButtonCssClassIcon { get; set; }

        public string SubmitButtonValue { get; set; }

        public string UpdateActionName { get; set; }

        public Func<ControlFormResult<TModel>, TModel, int, bool> ValidateWizardStep { get; set; }

        public string ValidationSummaryMessage { get; set; }

        public bool ValidationSupport { get; set; }

        public bool IsFormCenter { get; set; }

        public string CssFormCenter { get; set; }

        public string SetJavascript { get; set; }

        public ControlFormAction AddAction(bool isSubmitButton = false, bool isAjaxSupport = true, bool addToTop = true)
        {
            var action = new ControlFormAction(isSubmitButton, isAjaxSupport);

            Actions.Add(action);

            return action;
        }

        public ControlGroupedLayout<TModel> AddGroupedLayout(string title, bool getExists = false)
        {
            if (getExists)
            {
                var exists = groupedLayouts.FirstOrDefault(x => x.Title == title);
                if (exists != null)
                {
                    return exists;
                }
            }

            var layout = new ControlGroupedLayout<TModel>(title);
            groupedLayouts.Add(layout);
            return layout;
        }

        public virtual void AddProperty(string name, ControlFormAttribute attribute, object value = null)
        {
            attribute.Name = name;
            attribute.Value = value;
            addedProperties[name] = attribute;
        }

        public ControlTabbedLayout<TModel> AddTabbedLayout(string title)
        {
            var layout = new ControlTabbedLayout<TModel>(title);
            tabbedLayouts.Add(layout);
            return layout;
        }

        public void AssignGridLayout<TValue>(Expression<Func<TModel, TValue>> expression, int col, int row, int colSpan = 1, int rowSpan = 1)
        {
            AssignGridLayout(ExpressionHelper.GetExpressionText(expression), col, row, colSpan, rowSpan);
        }

        public void AssignGridLayout(string property, int col, int row, int colSpan = 1, int rowSpan = 1)
        {
            if (colSpan < 1)
            {
                throw new ArgumentOutOfRangeException("colSpan");
            }

            gridLayouts.Add(property, new GridLayout(col, row, colSpan, rowSpan));
        }

        public void ExcludeProperty(string property)
        {
            excludedProperties.Add(property);
        }

        public void ExcludeProperty<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            excludedProperties.Add(ExpressionHelper.GetExpressionText(expression));
        }

        public override string GenerateControlFormUI(ControllerContext controllerContext)
        {
            if (FormProvider == null)
            {
                FormProvider = ControlFormProvider.DefaultFormProvider();
            }
            var formProvider = FormProvider;

            ViewData.Model = model;

            var viewContext = new ViewContext
            {
                HttpContext = controllerContext.HttpContext,
                Controller = controllerContext.Controller,
                RequestContext = controllerContext.RequestContext,
                ClientValidationEnabled = false,
                Writer = new FormProviderTextWriter(formProvider),
                ViewData = ViewData
            };

            var workContext = controllerContext.GetWorkContext();
            var htmlHelper = new HtmlHelper(viewContext, new ViewDataContainer(ViewData));

            if (string.IsNullOrEmpty(ClientId))
            {
                ClientId = "fControl_" + Guid.NewGuid().ToString("N").ToLowerInvariant();
            }

            if (IsFormCenter)
            {
                formProvider.WriteToOutput("<div class='" + CssFormCenter + "'></div>");
            }

            // Start div container
            formProvider.WriteToOutput("<div class=\"control-form-container\">");

            using (BeginForm(htmlHelper, IsAjaxSupported, ClientId))
            {
                if (!string.IsNullOrEmpty(FormWrapperStartHtml))
                {
                    formProvider.WriteToOutput(string.Format(FormWrapperStartHtml, Title, IconHeader));
                }

                // Buttons
                var htmlActions = new List<string>();

                if (ShowSubmitButton && !ReadOnly)
                {
                    var btnSaveTagBuilder = new TagBuilder("button");
                    btnSaveTagBuilder.InnerHtml = SubmitButtonText;
                    if (!string.IsNullOrEmpty(SubmitButtonCssClassIcon))
                    {
                        btnSaveTagBuilder.InnerHtml = string.Format("<i class=\"{0}\"></i> {1}", SubmitButtonCssClassIcon, SubmitButtonText);
                    }
                    btnSaveTagBuilder.MergeAttribute("type", "submit");
                    btnSaveTagBuilder.MergeAttribute("name", "Save");

                    if (!string.IsNullOrEmpty(SubmitButtonCssClass))
                    {
                        btnSaveTagBuilder.MergeAttribute("class", SubmitButtonCssClass);
                    }

                    if (!string.IsNullOrEmpty(OnClientSubmitButtonClick))
                    {
                        btnSaveTagBuilder.MergeAttribute("onclick", OnClientSubmitButtonClick);
                    }

                    if (!string.IsNullOrEmpty(SubmitButtonValue))
                    {
                        btnSaveTagBuilder.MergeAttribute("value", SubmitButtonValue);
                    }

                    var btnSave = btnSaveTagBuilder.ToString(TagRenderMode.Normal);

                    htmlActions.Add(btnSave);
                }

                htmlActions.AddRange(Actions.Select(x => x.Create(formProvider)));

                if (ShowCancelButton)
                {
                    string btnCancel;
                    if (!string.IsNullOrEmpty(CancelButtonUrl))
                    {
                        if (!string.IsNullOrEmpty(CancelButtonIcon))
                        {
                            btnCancel = string.Format(
                                "<a class=\"{1}\" href=\"{2}\"><i class=\"{3}\"></i> {0}</a>",
                                CancelButtonText,
                                CancelButtonCssClass,
                                CancelButtonUrl, 
                                CancelButtonIcon);
                        }
                        else
                        {
                            btnCancel = string.Format(
                                "<a class=\"{1}\" href=\"{2}\">{0}</a>",
                                CancelButtonText,
                                CancelButtonCssClass,
                                CancelButtonUrl);
                        }
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(CancelButtonIcon))
                        {
                            const string btnCancelClick = "if(self != top){ parent.jQuery.fancybox.close(); }else{ history.back(); }";
                            btnCancel = string.Format(
                                "<button type=\"button\" name=\"Cancel\" class=\"{1}\" onclick=\"{2}\"><i class=\"{3}\"></i>  {0}</button>",
                                CancelButtonText,
                                CancelButtonCssClass,
                                btnCancelClick,
                                CancelButtonIcon);
                        }
                        else
                        {
                            const string btnCancelClick = "if(self != top){ parent.jQuery.fancybox.close(); }else{ history.back(); }";
                            btnCancel = string.Format(
                                "<button type=\"button\" name=\"Cancel\" class=\"{1}\" onclick=\"{2}\">{0}</button>",
                                CancelButtonText,
                                CancelButtonCssClass,
                                btnCancelClick);
                        }
                    }

                    htmlActions.Add(btnCancel);
                }

                var properties = GetProperties(controllerContext.RequestContext).ToList();

                if (!properties.Any())
                {
                    return formProvider.GetHtmlString();
                }

                foreach (var property in properties)
                {
                    property.ReadOnly = readonlyProperties.Contains(property.Name);

                    if (onAttributesRenders.ContainsKey(property.Name))
                    {
                        var action = onAttributesRenders[property.Name];
                        action(property);
                    }
                }

                switch (Layout)
                {
                    #region ControlFormLayout.Grid

                    case ControlFormLayout.Grid:
                        {
                            var gridLayoutColumns = Math.Max(gridLayouts.Max(x => x.Value.ColumnSpan), gridLayouts.Max(x => x.Value.Column) + 1);
                            var gridLayoutRows = gridLayouts.Max(x => x.Value.Row) + 1;

                            if (ShowBoxHeader)
                            {
                                formProvider.WriteToOutput("<div class=\"box\">");
                                formProvider.WriteToOutput(string.Format("<div class=\"box-header\">{1}<h5>{0}</h5></div>", Title, ShowCloseButton ? "<i class=\"cx-icon cx-icon-close\" onclick=\"window.parent.fancyboxResult = null; parent.jQuery.fancybox.close();\" style=\"cursor:pointer\"></i>" : ""));
                                formProvider.WriteToOutput("<div class=\"box-content\">");
                            }  

                            if (ShowValidationSummary)
                            {
                                formProvider.WriteToOutput(string.Format("<div data-valmsg-summary=\"true\" class=\"validation-summary\"><span>{0}</span><ul></ul></div>", ValidationSummaryMessage));
                            }

                            formProvider.WriteToOutput("<table style=\"width: 100%;\">");

                            var columnWith = 100 / gridLayoutColumns;
                            formProvider.WriteToOutput("<colgroup>");
                            for (int i = 0; i < gridLayoutColumns; i++)
                            {
                                formProvider.WriteToOutput(string.Format("<col style=\"width: {0}%\">", columnWith));
                            }
                            formProvider.WriteToOutput("</colgroup>");

                            for (var r = 0; r < gridLayoutRows; r++)
                            {
                                var controlsInRow = gridLayouts.Where(x => x.Value.Row == r).ToList();
                                if (controlsInRow.Count == 0)
                                {
                                    continue;
                                }

                                formProvider.WriteToOutput("<tr>");
                                var maxColSpan = 1;

                                for (var c = 0; c < gridLayoutColumns; c++)
                                {
                                    if (maxColSpan > 1)
                                    {
                                        if (c + 1 <= maxColSpan)
                                        {
                                            continue;
                                        }
                                    }

                                    // Calc row span
                                    var cells = gridLayouts.Values.Where(x => DetectSpanCellss(x, r, c)).ToList();
                                    if (cells.Count > 0)
                                    {
                                        continue;
                                    }

                                    var col = c;
                                    var controlsInCol = controlsInRow.Where(x => x.Value.Column == col).ToList();
                                    if (controlsInCol.Count == 0)
                                    {
                                        formProvider.WriteToOutput("<td>");
                                        formProvider.WriteToOutput("</td>");
                                        continue;
                                    }

                                    maxColSpan = controlsInCol.Max(x => x.Value.ColumnSpan);

                                    formProvider.WriteToOutput("<td style=\"vertical-align: top;\"");

                                    if (maxColSpan > 1)
                                    {
                                        formProvider.WriteToOutput(string.Format(" colspan=\"{0}\"", maxColSpan));
                                    }

                                    var rowSpan = controlsInCol.Max(x => x.Value.RowSpan);
                                    if (rowSpan > 1)
                                    {
                                        formProvider.WriteToOutput(string.Format(" rowspan=\"{0}\"", rowSpan));
                                    }

                                    formProvider.WriteToOutput(">");

                                    foreach (var control in controlsInCol)
                                    {
                                        var property = properties.FirstOrDefault(x => x.Name == control.Key);
                                        if (property == null)
                                        {
                                            continue;
                                        }

                                        if (property is ControlHiddenAttribute)
                                        {
                                            formProvider.WriteToOutput(property.GenerateControlUI(this, workContext, htmlHelper));
                                        }
                                        else
                                        {
                                            if (!string.IsNullOrEmpty(property.ControlSpan))
                                            {
                                                continue;
                                            }

                                            var spanControls = properties.Where(x => x.ControlSpan == property.Name).Select(x => x.GenerateControlUI(this, workContext, htmlHelper)).ToList();
                                            spanControls.Insert(0, property.GenerateControlUI(this, workContext, htmlHelper));

                                            formProvider.WriteToOutput(property, spanControls.ToArray());
                                        }
                                    }

                                    formProvider.WriteToOutput("</td>");
                                }

                                formProvider.WriteToOutput("</tr>");
                            }

                            formProvider.WriteToOutput("</table>");

                            formProvider.WriteToOutput(null, htmlActions.ToArray());
                            if (ShowBoxHeader)
                            {
                                formProvider.WriteToOutput("</div></div>"); 
                            }   
                        }
                        break;

                    #endregion ControlFormLayout.Grid

                    #region ControlFormLayout.Grouped

                    case ControlFormLayout.Grouped:
                        {
                            foreach (var groupedLayout in groupedLayouts)
                            {       
                                if (ShowBoxHeader)
                                {
                                    formProvider.WriteToOutput(string.Format("<div class=\"box {0}\">", groupedLayout.CssClass));
                                    formProvider.WriteToOutput(string.Format("<div class=\"box-header\"><h5>{0}</h5></div>", groupedLayout.Title));
                                    formProvider.WriteToOutput(groupedLayout.EnableScrollbar
                                                               ? "<div class=\"box-content\" style=\"overflow: auto;\">"
                                                               : "<div class=\"box-content\">");
                                }   

                                var groupedLayoutColumns = groupedLayout.Column;

                                if (groupedLayoutColumns == 0)
                                {
                                    groupedLayoutColumns = 1;
                                }

                                var groupedLayoutRows = (int)Math.Ceiling((double)groupedLayout.Properties.Count / groupedLayoutColumns);

                                if (groupedLayout.EnableGrid)
                                {
                                    formProvider.WriteToOutput("<table style=\"width: 100%;\">");

                                    var columnWith = 100 / groupedLayoutColumns;

                                    formProvider.WriteToOutput("<colgroup>");

                                    for (int i = 0; i < groupedLayoutColumns; i++)
                                    {
                                        formProvider.WriteToOutput(string.Format("<col style=\"width: {0}%\">", columnWith));
                                    }

                                    formProvider.WriteToOutput("</colgroup>");

                                    var index = 0;

                                    for (var i = 0; i < groupedLayoutRows; i++)
                                    {
                                        formProvider.WriteToOutput("<tr>");

                                        for (var j = 0; j < groupedLayoutColumns; j++)
                                        {
                                            if (index == groupedLayout.Properties.Count)
                                            {
                                                continue;
                                            }
                                            var propertyName = groupedLayout.Properties[index];

                                            if (excludedProperties.Contains(propertyName))
                                            {
                                                formProvider.WriteToOutput("<td>");
                                                formProvider.WriteToOutput("</td>");
                                                continue;
                                            }

                                            var property = properties.First(x => x.Name == propertyName);

                                            if (!string.IsNullOrEmpty(property.ControlSpan))
                                            {
                                                continue;
                                            }

                                            var spanControls = properties.Where(x => x.ControlSpan == propertyName).Select(x => x.GenerateControlUI(this, workContext, htmlHelper)).ToList();
                                            spanControls.Insert(0, property.GenerateControlUI(this, workContext, htmlHelper));

                                            if (property is ControlHiddenAttribute)
                                            {
                                                formProvider.WriteToOutput(property, spanControls.ToArray());
                                                index++;
                                                j--;
                                                continue;
                                            }

                                            formProvider.WriteToOutput("<td>");

                                            formProvider.WriteToOutput(property, spanControls.ToArray());

                                            formProvider.WriteToOutput("</td>");
                                            index++;
                                        }

                                        formProvider.WriteToOutput("</tr>");
                                    }

                                    formProvider.WriteToOutput("</table>");
                                }
                                else
                                {
                                    foreach (var propertyName in groupedLayout.Properties)
                                    {
                                        if (excludedProperties.Contains(propertyName))
                                        {
                                            continue;
                                        }

                                        var property = properties.First(x => x.Name == propertyName);

                                        if (!string.IsNullOrEmpty(property.ControlSpan))
                                        {
                                            continue;
                                        }

                                        var spanControls = properties.Where(x => x.ControlSpan == propertyName).Select(x => x.GenerateControlUI(this, workContext, htmlHelper)).ToList();
                                        spanControls.Insert(0, property.GenerateControlUI(this, workContext, htmlHelper));

                                        formProvider.WriteToOutput(property, spanControls.ToArray());
                                    }
                                }

                                if (ShowBoxHeader)
                                {
                                    formProvider.WriteToOutput("</div></div>");
                                }  
                            }

                            formProvider.WriteToOutput(null, htmlActions.ToArray());
                        }
                        break;

                    #endregion ControlFormLayout.Grouped

                    #region ControlFormLayout.Tab

                    case ControlFormLayout.Tab:
                        {
                            formProvider.WriteToOutput("<div class=\"box\">");

                            formProvider.WriteToOutput(string.Format("<div class=\"box-header\">"));
                            formProvider.WriteToOutput(string.Format("<ul class=\"nav nav-tabs\">"));

                            var tabIndex = 0;
                            foreach (var tabbedLayout in tabbedLayouts)
                            {
                                formProvider.WriteToOutput(tabIndex == 0
                                    ? string.Format("<li class=\"active\"><a data-toggle=\"tab\" href=\"#{1}_Tab{2}\">{0}</a></li>",
                                        tabbedLayout.Title,
                                        ClientId,
                                        tabIndex)
                                    : string.Format("<li><a data-toggle=\"tab\" href=\"#{1}_Tab{2}\">{0}</a></li>",
                                        tabbedLayout.Title,
                                        ClientId,
                                        tabIndex));

                                tabIndex++;
                            }

                            formProvider.WriteToOutput("</ul>");
                            formProvider.WriteToOutput("</div>");

                            formProvider.WriteToOutput("<div class=\"box-content\">");
                            formProvider.WriteToOutput("<div class=\"tab-content\">");

                            tabIndex = 0;
                            foreach (var tabbedLayout in tabbedLayouts)
                            {
                                formProvider.WriteToOutput(tabIndex == 0
                                    ? string.Format("<div id=\"{0}_Tab{1}\" class=\"tab-pane active\">", ClientId, tabIndex)
                                    : string.Format("<div id=\"{0}_Tab{1}\" class=\"tab-pane\">", ClientId, tabIndex));
                                tabIndex++;

                                foreach (var item in tabbedLayout.Groups)
                                {
                                    var propertiesInGroup = properties.Where(x => item.Properties.Contains(x.Name)).ToList();
                                    if (propertiesInGroup.Count == 0)
                                    {
                                        continue;
                                    }

                                    if (propertiesInGroup.All(x => x.ContainerRowIndex == -100))
                                    {
                                        var index = 0;
                                        foreach (var attribute in propertiesInGroup.Where(attribute => !(attribute is ControlHiddenAttribute)))
                                        {
                                            attribute.ContainerRowIndex = index;
                                            index++;
                                        }
                                    }

                                    if (item.Title != null)
                                    {
                                        if (ShowBoxHeader)
                                        {
                                            formProvider.WriteToOutput("<div class=\"box\">");
                                            formProvider.WriteToOutput(string.Format("<div class=\"box-header\"><h2>{0}</h2></div>", item.Title));
                                            formProvider.WriteToOutput("<div class=\"box-content\">");
                                        }   
                                    }

                                    // Render hidden fields
                                    foreach (var attribute in propertiesInGroup.Where(attribute => (attribute is ControlHiddenAttribute)))
                                    {
                                        formProvider.WriteToOutput(attribute.GenerateControlUI(this, workContext, htmlHelper));
                                    }

                                    var max = propertiesInGroup.Max(x => x.ContainerRowIndex);
                                    var min = propertiesInGroup.Min(x => x.ContainerRowIndex);
                                    for (var i = min; i <= max; i++)
                                    {
                                        var propertiesInRow = propertiesInGroup.Where(x => x.ContainerRowIndex == i).ToList();
                                        if (propertiesInRow.Count == 0)
                                        {
                                            continue;
                                        }

                                        formProvider.WriteToOutput("<div class=\"row\">");

                                        foreach (var property in propertiesInRow)
                                        {
                                            if (excludedProperties.Contains(property.Name))
                                            {
                                                continue;
                                            }
                                            formProvider.WriteToOutput(property, property.GenerateControlUI(this, workContext, htmlHelper));
                                        }

                                        formProvider.WriteToOutput("</div>");
                                    }

                                    if (item.Title != null)
                                    {
                                        if (ShowBoxHeader)
                                        {
                                            formProvider.WriteToOutput("</div></div>");
                                        }                                              
                                    }
                                }

                                formProvider.WriteToOutput("</div>");
                            }

                            formProvider.WriteToOutput("<div class=\"row\">");
                            formProvider.WriteActions(FormActionsContainerCssClass, FormActionsCssClass, htmlActions.ToArray());
                            formProvider.WriteToOutput("</div>");

                            formProvider.WriteToOutput("</div>");
                            formProvider.WriteToOutput("</div>");
                            formProvider.WriteToOutput("</div>");
                        }
                        break;

                    #endregion ControlFormLayout.Tab

                    #region ControlFormLayout.Wizard

                    case ControlFormLayout.Wizard:
                        {
                            Actions.Clear();
                            var currentStep = GetCurrentWizardStep(controllerContext);

                            if (currentStep > 0)
                            {
                                Actions.Add(new ControlFormAction(true, false).HasText("Back").HasName("__CurrentStep").HasValue(Convert.ToString(currentStep - 1)).HasButtonStyle(ButtonStyle.Default));
                            }

                            if (currentStep < groupedLayouts.Count - 1)
                            {
                                Actions.Add(new ControlFormAction(true, true).HasText("Next").HasName("__CurrentStep").HasValue(Convert.ToString(currentStep + 1)).HasButtonStyle(ButtonStyle.Primary));
                            }

                            if (currentStep == groupedLayouts.Count - 1)
                            {
                                // Finish
                                Actions.Add(new ControlFormAction(true, true).HasText(SubmitButtonText).HasName("__CurrentStep").HasValue(Convert.ToString(currentStep + 1)).HasButtonStyle(ButtonStyle.Primary));
                            }

                            //htmlActions.AddRange(Actions.Select(action => action.Create(formProvider)));

                            var index = 0;
                            foreach (var groupedLayout in groupedLayouts)
                            {
                                if (currentStep != index)
                                {
                                    foreach (var propertyName in groupedLayout.Properties)
                                    {
                                        var value = GetPropertyValue(model, propertyName);
                                        if (value != null)
                                        {
                                            formProvider.WriteToOutput(string.Format("<input type=\"hidden\" name=\"ControlWizard_{0}\" value=\"{1}\" />", propertyName, HttpUtility.HtmlEncode(value.SharpSerialize())));
                                        }
                                    }

                                    index++;
                                    continue;
                                }  

                                if (ShowBoxHeader)
                                {
                                    formProvider.WriteToOutput("<div class=\"box\">");
                                    formProvider.WriteToOutput(string.Format("<div class=\"box-header\"><h5>{0}</h5></div>", groupedLayout.Title));
                                    formProvider.WriteToOutput("<div class=\"box-content\">");
                                }  

                                // Auto assign grid row index
                                if (properties.Where(pair => !excludedProperties.Contains(pair.Name)).All(x => x.ContainerRowIndex == -1))
                                {
                                    var rowIndex = 0;
                                    foreach (var property in properties)
                                    {
                                        if (property is ControlHiddenAttribute)
                                        {
                                            continue;
                                        }
                                        property.ContainerRowIndex = rowIndex;
                                        rowIndex++;
                                    }
                                }

                                var includedProperties = groupedLayout.Properties.Where(x => !excludedProperties.Contains(x));

                                // Render hidden fields
                                foreach (var property in properties.Where(x => includedProperties.Contains(x.Name) && x is ControlHiddenAttribute))
                                {
                                    formProvider.WriteToOutput(property.GenerateControlUI(this, workContext, htmlHelper));
                                }

                                var max = properties.Max(x => x.ContainerRowIndex);
                                for (var i = 0; i <= max; i++)
                                {
                                    var propertiesInRow = properties.Where(x =>
                                        x.ContainerRowIndex == i &&
                                        includedProperties.Contains(x.Name))
                                       .ToList();

                                    if (!propertiesInRow.Any())
                                    {
                                        continue;
                                    }

                                    formProvider.WriteToOutput("<div class=\"row\">");

                                    foreach (var property in propertiesInRow)
                                    {
                                        formProvider.WriteToOutput(property, property.GenerateControlUI(this, workContext, htmlHelper));
                                    }

                                    formProvider.WriteToOutput("</div>");
                                }

                                if (ShowBoxHeader)
                                {
                                    formProvider.WriteToOutput("</div></div>");
                                } 

                                index++;
                            }

                            // In wizard, we overwrite other actions
                            htmlActions = Actions.Select(x => x.Create(formProvider)).ToList();

                            formProvider.WriteToOutput("<div class=\"row\">");
                            formProvider.WriteActions(FormActionsContainerCssClass, FormActionsCssClass, htmlActions.ToArray());
                            formProvider.WriteToOutput("</div>");
                        }
                        break;

                    #endregion ControlFormLayout.Wizard

                    #region ControlFormLayout.Table

                    case ControlFormLayout.Table:
                        {   
                            if (ShowBoxHeader)
                            {
                                formProvider.WriteToOutput("<div class=\"box\">");
                                formProvider.WriteToOutput(string.Format("<div class=\"box-header\"><h5>{0}</h5></div>", Title));
                                formProvider.WriteToOutput("<div class=\"box-content\">");
                            }

                            // Hidden fields
                            foreach (var property in properties.Where(pair => !excludedProperties.Contains(pair.Name) && pair is ControlHiddenAttribute))
                            {
                                formProvider.WriteToOutput(property.GenerateControlUI(this, workContext, htmlHelper));
                            }

                            formProvider.WriteToOutput("<table class=\"table table-bordered\">");

                            formProvider.WriteToOutput("<tbody>");

                            foreach (var property in properties.Where(pair => !excludedProperties.Contains(pair.Name) && !(pair is ControlHiddenAttribute)))
                            {
                                if (!string.IsNullOrEmpty(property.ControlSpan))
                                {
                                    continue;
                                }

                                var spanControls = properties.Where(x => x.ControlSpan == property.Name).Select(x => x.GenerateControlUI(this, workContext, htmlHelper)).ToList();
                                spanControls.Insert(0, property.GenerateControlUI(this, workContext, htmlHelper));

                                formProvider.WriteToOutput("<tr>");

                                formProvider.WriteToOutput(string.Format("<td>{0}</td>", property.LabelText));

                                formProvider.WriteToOutput("<td>");
                                formProvider.WriteToOutput(spanControls.ToArray());
                                formProvider.WriteToOutput("</td>");

                                formProvider.WriteToOutput("</tr>");
                            }

                            formProvider.WriteToOutput("</tbody>");

                            formProvider.WriteToOutput("</table>");

                            formProvider.WriteToOutput(null, htmlActions.ToArray());

                            if (ShowBoxHeader)
                            {
                                formProvider.WriteToOutput("</div></div>");
                            }

                            break;
                        }

                    #endregion ControlFormLayout.Table

                    #region ControlFormLayout.Flat

                    case ControlFormLayout.Flat:
                        { 
                            if (ShowBoxHeader)
                            {
                                formProvider.WriteToOutput("<div class=\"box\">");
                                formProvider.WriteToOutput(string.Format("<div class=\"box-header\"><h2>{0}</h2><div class=\"pull-right box-header-controls\">{1}</div></div>", Title, ShowCloseButton ? "<i class=\"cx-icon cx-icon-close\" onclick=\"window.parent.fancyboxResult = null; parent.jQuery.fancybox.close();\" style=\"cursor:pointer\"></i>" : ""));
                                formProvider.WriteToOutput("<div class=\"box-content\">");
                            } 

                            if (!string.IsNullOrEmpty(Description))
                            {
                                formProvider.WriteToOutput("<div class=\"lead\">" + Description + "</div>");
                            }

                            if (ShowValidationSummary)
                            {
                                if (string.IsNullOrEmpty(ValidationSummaryMessage))
                                {
                                    formProvider.WriteToOutput("<div data-valmsg-summary=\"true\" class=\"validation-summary\"><ul></ul></div>");
                                }
                                else
                                {
                                    formProvider.WriteToOutput(string.Format("<div data-valmsg-summary=\"true\" class=\"validation-summary\"><span>{0}</span><ul></ul></div>", ValidationSummaryMessage));
                                }
                            }

                            // Auto assign grid row index
                            if (properties.Where(pair => !excludedProperties.Contains(pair.Name)).All(x => x.ContainerRowIndex == -1))
                            {
                                var rowIndex = 0;
                                foreach (var property in properties)
                                {
                                    if (property is ControlHiddenAttribute)
                                    {
                                        continue;
                                    }
                                    property.ContainerRowIndex = rowIndex;
                                    rowIndex++;
                                }
                            }

                            // Render hidden fields
                            foreach (var property in properties.Where(x => !excludedProperties.Contains(x.Name) && x is ControlHiddenAttribute))
                            {
                                formProvider.WriteToOutput(property.GenerateControlUI(this, workContext, htmlHelper));
                            }

                            var max = properties.Max(x => x.ContainerRowIndex);

                            if (max != -100)
                            {
                                for (var i = 0; i <= max; i++)
                                {
                                    var propertiesInRow =
                                        properties.Where(
                                            x => x.ContainerRowIndex == i && !excludedProperties.Contains(x.Name))
                                            .ToList();
                                    if (!propertiesInRow.Any())
                                    {
                                        continue;
                                    }

                                    formProvider.WriteToOutput("<div class=\"row\">");

                                    foreach (var property in propertiesInRow)
                                    {
                                        formProvider.WriteToOutput(property,
                                            property.GenerateControlUI(this, workContext, htmlHelper));
                                    }

                                    formProvider.WriteToOutput("</div>");
                                }
                            }
                            else
                            {
                                foreach (var property in properties.Where(x => !excludedProperties.Contains(x.Name) && !(x is ControlHiddenAttribute)))
                                {
                                    formProvider.WriteToOutput("<div class=\"row\">");

                                    formProvider.WriteToOutput(property,
                                        property.GenerateControlUI(this, workContext, htmlHelper));

                                    formProvider.WriteToOutput("</div>");
                                }
                            }


                            formProvider.WriteToOutput("<div class=\"row row-actions\"><hr/>");
                            formProvider.WriteActions(FormActionsContainerCssClass, FormActionsCssClass, htmlActions.ToArray());
                            formProvider.WriteToOutput("</div>");
                            if (ShowBoxHeader)           
                            {
                                formProvider.WriteToOutput("</div></div>");
                            } 
                        }
                        break;

                    #endregion ControlFormLayout.Flat

                    default:
                        throw new ArgumentOutOfRangeException();
                }

                foreach (var hiddenValue in HiddenValues)
                {
                    formProvider.WriteToOutput(string.Format("<input type=\"hidden\" id=\"{0}\" name=\"{0}\" value=\"{1}\"/>", hiddenValue.Key, HttpUtility.HtmlEncode(hiddenValue.Value)));
                }

                if (!string.IsNullOrEmpty(FormWrapperEndHtml))
                {
                    formProvider.WriteToOutput(FormWrapperEndHtml);
                }
            }

            var scriptRegister = new ScriptRegister(workContext);

            if (!DisableBlockUI)
            {
                // Block UI
                formProvider.WriteToOutput("<div class=\"blockUI\" style=\"display:none; z-index: 100; border: none; margin: 0; padding: 0; width: 100%; height: 100%; top: 0; left: 0; background-color: #000000; opacity: 0.05; filter: alpha(opacity = 5); cursor: wait; position: absolute;\"></div>");

                // Block Msg
                formProvider.WriteToOutput("<div class=\"blockUIMsg\" style=\"display:none;\">Processing...</div>");

                if (IsAjaxSupported)
                {
                    scriptRegister.IncludeInline("$(document).bind(\"ajaxSend\", function(){ $(\".blockUI, .blockUIMsg\").show(); }).bind(\"ajaxComplete\", function(){ $(\".blockUI, .blockUIMsg\").hide(); });");
                }
                else
                {
                    scriptRegister.IncludeInline(string.Format("$('#{0}').on(\"submit\", function(){{ var isValid = $('#{0}').valid(); if(isValid){{ $(\".blockUI, .blockUIMsg\").show(); }} }});", ClientId));
                }
            }

            // End div container
            formProvider.WriteToOutput("</div>");

            if (!string.IsNullOrEmpty(SetJavascript))
            {
                formProvider.WriteToOutput(SetJavascript);
            }

            if (IsFormCenter)
            {
                formProvider.WriteToOutput("<div class='" + CssFormCenter + "'></div>");
            }

            if (EnableKnockoutJs)
            {
                scriptRegister.IncludeInline(string.Format("var data = $('#{0}').serializeObject();", ClientId));
                scriptRegister.IncludeInline("var viewModel = ko.mapping.fromJS(data);");
                scriptRegister.IncludeInline(string.Format("ko.applyBindings(viewModel, document.getElementById('{0}'));", ClientId));
            }

            if (Layout == ControlFormLayout.Tab)
            {
                scriptRegister.IncludeInline(string.Format("function {1}_ValidateTabs(){{ var validationInfo = $('#{1}').data('unobtrusiveValidation'); for(var i = 0; i < {0}; i++){{ $('a[href=#{1}_Tab' + i + ']').tab('show'); var isValid = validationInfo.validate(); if(!isValid){{ return false; }} }} }}", groupedLayouts.Count, ClientId));
            }

            return formProvider.GetHtmlString();
        }

        public override IEnumerable<ResourceType> GetAdditionalResources(ControllerContext context)
        {
            if (ValidationSupport)
            {
                yield return ResourceType.JQueryValidate;
            }

            foreach (var addition in GetProperties(context.RequestContext).SelectMany(property => property.GetAdditionalResources()))
            {
                yield return addition;
            }

            if (EnableKnockoutJs)
            {
                yield return ResourceType.KnockoutJs;
            }
        }

        public virtual ControlAutoCompleteOptions GetAutoCompleteDataSource(string property)
        {
            if (!autoCompleteDataSources.ContainsKey(property))
            {
                throw new ArgumentOutOfRangeException("property", "You must register a auto complete options for: " + property);
            }
            var dataSource = autoCompleteDataSources[property];
            return dataSource;
        }

        public virtual string GetCascadingCheckBoxDataSource(string property)
        {
            if (cascadingCheckboxDataSource.ContainsKey(property))
            {
                return cascadingCheckboxDataSource[property];
            }

            throw new NotSupportedException(string.Format("You must register a cascading dropdown data source for '{0}'.", property));
        }

        public virtual ControlCascadingDropDownOptions GetCascadingDropDownDataSource(string property)
        {
            if (cascadingDropDownDataSource.ContainsKey(property))
            {
                return cascadingDropDownDataSource[property];
            }

            throw new NotSupportedException(string.Format("You must register a cascading dropdown data source for '{0}'.", property));
        }

        public virtual IList<SelectListItem> GetExternalDataSource(string property)
        {
            if (!externalDataSources.ContainsKey(property))
            {
                return null;
            }

            var dataSource = externalDataSources[property];
            return dataSource.Invoke(model).ToList();
        }

        public virtual ControlFileUploadOptions GetFileUploadOptions(string property)
        {
            if (fileUploadOptions.ContainsKey(property))
            {
                return fileUploadOptions[property];
            }

            return new ControlFileUploadOptions();
        }

        public virtual IEnumerable<ControlFormAttribute> GetProperties(RequestContext requestContext)
        {
            var workContext = requestContext.GetWorkContext();
            var cacheManager = workContext.Resolve<IStaticCacheManager>();

            var attributes = cacheManager.Get("ControlForms_Properties_" + modelType.FullName, ctx =>
            {
                var propertyInfos = modelType.GetProperties(BindingFlags.Instance | BindingFlags.Public);
                var result = new List<ControlFormAttribute>();

                foreach (var propertyInfo in propertyInfos)
                {
                    var attrs = propertyInfo.GetCustomAttributes(typeof(ControlFormAttribute), false);
                    if (attrs.Length > 0)
                    {
                        var attribute = (ControlFormAttribute)attrs[0];
                        attribute.Name = propertyInfo.Name;
                        attribute.PropertyName = propertyInfo.Name;
                        attribute.PropertyType = propertyInfo.PropertyType;
                        attribute.PropertyInfo = propertyInfo;

                        if (attribute.LabelText == null)
                        {
                            attribute.LabelText = propertyInfo.Name;
                        }

                        result.Add(attribute);
                    }
                }
                return result.OrderBy(x => x.Order).ToList();
            });

            var properties = new List<ControlFormAttribute>();

            foreach (var attribute in attributes)
            {
                var property = attribute.ShallowCopy();
                property.Value = GetPropertyValue(model, property.Name);
                properties.Add(property);
            }

            return properties.Concat(addedProperties.Values).OrderBy(x => x.Order);
        }

        public object GetPropertyValue(object obj, string property)
        {
            if (obj == null)
            {
                return null;
            }

            var type = obj.GetType();
            var propertyInfo = type.GetProperty(property);
            if (propertyInfo != null)
            {
                return propertyInfo.GetValue(obj);
            }

            var provider = obj as IDynamicMetaObjectProvider;
            if (provider != null)
            {
                dynamic dynamicObject = provider;
                return dynamicObject[property];
            }

            return null;
        }

        public void MakeReadOnlyProperty(string property)
        {
            readonlyProperties.Add(property);
        }

        public void MakeReadOnlyProperty<TValue>(Expression<Func<TModel, TValue>> expression)
        {
            readonlyProperties.Add(ExpressionHelper.GetExpressionText(expression));
        }

        public override bool OverrideExecuteResult(ControllerContext context)
        {
            if (Layout == ControlFormLayout.Wizard)
            {
                GetProperties(context.RequestContext);
                var currentStep = GetCurrentWizardStep(context);
                if (currentStep >= groupedLayouts.Count)
                {
                    context.Controller.ValueProvider = new ControlWizardValueProvider(context.Controller.ValueProvider, model);
                    var invoker = new ControllerActionInvoker();
                    invoker.InvokeAction(context, UpdateActionName);
                    return true;
                }
            }
            return base.OverrideExecuteResult(context);
        }

        public void RegisterAutoCompleteDataSource<TValue>(Expression<Func<TModel, TValue>> expression, ControlAutoCompleteOptions<TModel> options)
        {
            autoCompleteDataSources.Add(ExpressionHelper.GetExpressionText(expression), options);
        }

        public void RegisterAutoCompleteDataSource<TValue>(Expression<Func<TModel, TValue>> expression, string sourceUrl)
        {
            autoCompleteDataSources.Add(ExpressionHelper.GetExpressionText(expression), new ControlAutoCompleteOptions<TModel> { SourceUrl = sourceUrl });
        }

        public void RegisterAutoCompleteDataSource<TValue>(Expression<Func<TModel, TValue>> expression, string sourceUrl, Func<TModel, string> textSelector)
        {
            autoCompleteDataSources.Add(ExpressionHelper.GetExpressionText(expression), new ControlAutoCompleteOptions<TModel> { SourceUrl = sourceUrl, TextSelector = textSelector });
        }

        public void RegisterCascadingCheckboxDataSource(string property, string sourceUrl)
        {
            cascadingCheckboxDataSource.Add(property, sourceUrl);
        }

        public void RegisterCascadingDropDownDataSource<TValue>(Expression<Func<TModel, TValue>> expression, string sourceUrl)
        {
            cascadingDropDownDataSource.Add(ExpressionHelper.GetExpressionText(expression), new ControlCascadingDropDownOptions { SourceUrl = sourceUrl });
        }

        public void RegisterCascadingDropDownDataSource<TValue>(Expression<Func<TModel, TValue>> expression, ControlCascadingDropDownOptions options)
        {
            cascadingDropDownDataSource.Add(ExpressionHelper.GetExpressionText(expression), options);
        }

        public void RegisterCascadingDropDownDataSource(string property, string sourceUrl)
        {
            cascadingDropDownDataSource.Add(property, new ControlCascadingDropDownOptions { SourceUrl = sourceUrl });
        }

        public void RegisterCascadingDropDownDataSource(string property, ControlCascadingDropDownOptions options)
        {
            cascadingDropDownDataSource.Add(property, options);
        }

        public void RegisterExternalDataSource<TProperty>(Expression<Func<TModel, TProperty>> expression, Func<TModel, IEnumerable<SelectListItem>> items)
        {
            var str = ExpressionHelper.GetExpressionText(expression);
            externalDataSources.Add(str, items);
        }

        public void RegisterExternalDataSource<TProperty>(Expression<Func<TModel, TProperty>> expression, IEnumerable<SelectListItem> items)
        {
            var str = ExpressionHelper.GetExpressionText(expression);
            externalDataSources.Add(str, x => items);
        }

        public void RegisterExternalDataSource<TProperty>(Expression<Func<TModel, TProperty>> expression, params string[] values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            var func = new Func<TModel, List<SelectListItem>>(item => values.Select(value => new SelectListItem
            {
                Text = value,
                Value = value
            }).ToList());
            externalDataSources.Add(ExpressionHelper.GetExpressionText(expression), func);
        }

        public void RegisterExternalDataSource<TProperty, TKey>(Expression<Func<TModel, TProperty>> expression, IDictionary<TKey, string> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            var func = new Func<TModel, List<SelectListItem>>(item => values.Select(value => new SelectListItem
            {
                Text = value.Value,
                Value = Convert.ToString(value.Key)
            }).ToList());
            externalDataSources.Add(ExpressionHelper.GetExpressionText(expression), func);
        }

        public void RegisterExternalDataSource(string property, Func<TModel, IEnumerable<SelectListItem>> items)
        {
            externalDataSources.Add(property, items);
        }

        public void RegisterExternalDataSource(string property, params string[] values)
        {
            RegisterExternalDataSource(property, values.ToDictionary(k => k, v => v));
        }

        public void RegisterExternalDataSource(string property, IEnumerable<string> values)
        {
            RegisterExternalDataSource(property, values.ToDictionary(k => k, v => v));
        }

        public void RegisterExternalDataSource<TKey>(string property, IDictionary<TKey, string> values)
        {
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            var func = new Func<TModel, List<SelectListItem>>(item => values.Select(value => new SelectListItem
            {
                Text = value.Value,
                Value = Convert.ToString(value.Key)
            }).ToList());
            externalDataSources.Add(property, func);
        }

        public void RegisterExternalDataSource<TKey>(string property, Dictionary<TKey, string> dataSourceValues)
        {
            externalDataSources[property] = m => new List<SelectListItem>(dataSourceValues.Select(x => new SelectListItem { Text = x.Value, Value = Convert.ToString(x.Key) }));
        }

        public void RegisterExternalDataSource(string property, IEnumerable<SelectListItem> items)
        {
            externalDataSources[property] = m => items;
        }

        public void RegisterFileUploadOptions<TValue>(Expression<Func<TModel, TValue>> expression, ControlFileUploadOptions options)
        {
            fileUploadOptions.Add(ExpressionHelper.GetExpressionText(expression), options);
        }

        public void RegisterFileUploadOptions(string property, string uploadUrl)
        {
            fileUploadOptions.Add(property, new ControlFileUploadOptions { UploadUrl = uploadUrl });
        }

        public void RegisterFileUploadOptions(string property, ControlFileUploadOptions options)
        {
            fileUploadOptions.Add(property, options);
        }

        public virtual void SetPropertyValue(RequestContext requestContext, string property, object value)
        {
            var attributes = GetProperties(requestContext);
            var propertyInfo = attributes.FirstOrDefault(x => x.Name == property);
            if (propertyInfo != null)
            {
                propertyInfo.PropertyInfo.SetValue(model, value);
            }
        }

        private static bool DetectSpanCellss(GridLayout gridLayout, int row, int column)
        {
            if (gridLayout.Column == 0 && gridLayout.Row == 0)
            {
                return false;
            }

            if (gridLayout.ColumnSpan == 1 && gridLayout.RowSpan == 1)
            {
                return false;
            }

            if (gridLayout.Column == column && gridLayout.Row == row)
            {
                return false;
            }

            if (gridLayout.Row > row)
            {
                return false;
            }

            if (gridLayout.Column > column)
            {
                return false;
            }

            if (gridLayout.Row + gridLayout.RowSpan > row && gridLayout.Column + gridLayout.ColumnSpan > column)
            {
                return true;
            }

            return false;
        }

        private int GetCurrentWizardStep(ControllerContext controllerContext)
        {
            var httpContext = controllerContext.HttpContext;

            if (httpContext.Items["__ControlWizard_CurrentStep"] != null)
            {
                return (int)httpContext.Items["__ControlWizard_CurrentStep"];
            }

            var currentStep = Convert.ToInt32(httpContext.Request.Form["__CurrentStep"]);

            // Bind data from form to model
            if (httpContext.Request.HttpMethod == "POST")
            {
                foreach (var key in httpContext.Request.Form.AllKeys.Where(x => x.StartsWith("ControlWizard_")))
                {
                    var value = httpContext.Request.Form[key];
                    value = value.HtmlEncode();
                    value = value.HtmlDecode();
                    SetPropertyValue(controllerContext.RequestContext, key.Replace("ControlWizard_", ""), value.SharpDeserialize<object>());
                }

                TryUpdateModel(model, controllerContext);
            }

            if (currentStep > 0 && ValidateWizardStep != null)
            {
                var isValid = ValidateWizardStep(this, model, currentStep);
                if (!isValid)
                {
                    currentStep--;
                    while (currentStep > 0 && !ValidateWizardStep(this, model, currentStep))
                    {
                        currentStep--;
                    }
                }
            }

            httpContext.Items["__ControlWizard_CurrentStep"] = currentStep;

            return currentStep;
        }

        #region Dynamic Control Attribute Properties

        public void OnRenderAttribute<TControlAttribute>(Expression<Func<TModel, dynamic>> expression, Action<TControlAttribute> action)
            where TControlAttribute : ControlFormAttribute
        {
            var str = ExpressionHelper.GetExpressionText(expression);
            var internalAction = new Action<dynamic>(obj => action(obj));
            onAttributesRenders.Add(str, internalAction);
        }

        public void OnRenderAttribute<TControlAttribute>(string property, Action<TControlAttribute> action)
            where TControlAttribute : ControlFormAttribute
        {
            var str = property;
            var internalAction = new Action<dynamic>(obj => action(obj));
            onAttributesRenders.Add(str, internalAction);
        }

        #endregion Dynamic Control Attribute Properties

        #region Helpers

        public virtual MvcHtmlString CreateLabelFor(HtmlHelper htmlHelper, ControlFormAttribute attribute)
        {
            return htmlHelper.Label(attribute.Name, attribute.LabelText, new { @class = "control-label" });
        }

        private static void TryUpdateModel(TModel model, ControllerContext controllerContext)
        {
            var binder = new DefaultModelBinder();
            var bindingContext = new ModelBindingContext
                                     {
                                         ModelMetadata = ModelMetadataProviders.Current.GetMetadataForType(() => model, typeof(TModel)),
                                         ModelState = controllerContext.Controller.ViewData.ModelState,
                                         ValueProvider = controllerContext.Controller.ValueProvider
                                     };

            binder.BindModel(controllerContext, bindingContext);
        }

        private IDisposable BeginForm(HtmlHelper htmlHelper, bool supportAjax, string formId)
        {
            if (Layout == ControlFormLayout.Wizard)
            {
                UpdateActionName = null;
            }

            var htmlAttributes = new RouteValueDictionary();

            if (!string.IsNullOrEmpty(CssClass))
            {
                htmlAttributes.Add("class", CssClass);
            }

            if (string.IsNullOrEmpty(UpdateActionName))
            {
                htmlAttributes.Add("action", htmlHelper.ViewContext.HttpContext.Request.RawUrl);
            }

            if (!string.IsNullOrEmpty(FormActionUrl))
            {
                htmlAttributes["action"] = FormActionUrl;
            }

            htmlAttributes.Add("id", formId);

            if (Layout == ControlFormLayout.Tab)
            {
                htmlAttributes.Add("data-ajax-begin", formId + "_ValidateTabs");
            }

            if (!supportAjax)
            {
                htmlAttributes.Add("enctype", "multipart/form-data");
            }

            return supportAjax
                ? htmlHelper.BeginAjaxForm(UpdateActionName, null, FormMethod, htmlAttributes)
                : htmlHelper.BeginForm(UpdateActionName, null, FormMethod, htmlAttributes);
        }

        private class GridLayout
        {
            public GridLayout(int column, int row, int columnSpan = 1, int rowSpan = 1)
            {
                Column = column;
                Row = row;
                ColumnSpan = columnSpan;
                RowSpan = rowSpan;
            }

            public int Column { get; private set; }

            public int ColumnSpan { get; private set; }

            public int Row { get; private set; }

            public int RowSpan { get; private set; }
        }

        private class ControlWizardValueProvider : IValueProvider
        {
            private readonly object model;
            private readonly IValueProvider valueProvider;

            public ControlWizardValueProvider(IValueProvider valueProvider, object model)
            {
                this.valueProvider = valueProvider;
                this.model = model;
            }

            public bool ContainsPrefix(string prefix)
            {
                return prefix == "model" || valueProvider.ContainsPrefix(prefix);
            }

            public ValueProviderResult GetValue(string key)
            {
                return key == "model"
                    ? new ValueProviderResult(model, null, CultureInfo.InvariantCulture)
                    : valueProvider.GetValue(key);
            }
        }

        #endregion Helpers
    }
}