using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using CMSSolutions.Data;
using CMSSolutions.Serialization;
using CMSSolutions.Web.UI;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Widgets
{
    public abstract class WidgetBase : BaseEntity<int>, IWidget
    {
        #region IWidget Members

        [ControlHidden, ExcludeFromSerialization]
        public override int Id { get; set; }

        [ExcludeFromSerialization]
        public abstract string Name { get; }

        [ControlChoice(ControlChoice.CheckBox, Order = -9999, ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = -10, RenderLabelControl = false, AppendText = "Localized for this language")]
        public bool Localized { get; set; }

        [ExcludeFromSerialization]
        [ControlText(Required = true, LabelText = "Title", MaxLength = 255, Order = -9998, ContainerCssClass = "col-xs-6 col-sm-6", ContainerRowIndex = -9)]
        public string Title { get; set; }

        [Display(Name = "Show Title On Page")]
        [ControlChoice(ControlChoice.CheckBox, LabelText = "&nbsp;", AppendText = "Show Title On Page", Order = -9997, ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = -9)]
        public bool ShowTitleOnPage { get; set; }

        public virtual bool HasTitle { get { return true; } }

        [ExcludeFromSerialization]
        [Display(Name = "Display Condition")]
        [ControlText(Order = -9994, LabelText = "Display Condition", ContainerCssClass = "col-xs-6 col-sm-6", ContainerRowIndex = -8)]
        public string DisplayCondition { get; set; }

        [ExcludeFromSerialization]
        [ControlChoice(ControlChoice.CheckBox, Order = -9993, ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = -8, LabelText = "&nbsp;", AppendText = "Enabled")]
        public bool Enabled { get; set; }

        [Display(Name = "Zone")]
        [ExcludeFromSerialization]
        [ControlChoice(ControlChoice.DropDownList, Required = true, Order = -9996, LabelText = "Zone", CssClass = "uniform", ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = -7)]
        public int ZoneId { get; set; }

        [ExcludeFromSerialization]
        public int? PageId { get; set; }

        [ExcludeFromSerialization]
        [ControlNumeric(Required = true, LabelText = "Order", Order = -9995, ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = -7)]
        public int Order { get; set; }

        [ControlText(MaxLength = 255, LabelText = "Css Class", Order = -9992, ContainerCssClass = "col-xs-3 col-sm-3", ContainerRowIndex = -7)]
        public string CssClass { get; set; }

        [ExcludeFromSerialization]
        public bool IsMoveable { get; set; }

        [ExcludeFromSerialization]
        public string CultureCode { get; set; }

        [ExcludeFromSerialization]
        public int? RefId { get; set; }

        public virtual IEnumerable<ResourceType> GetAdditionalResources()
        {
            return Enumerable.Empty<ResourceType>();
        }

        public abstract void BuildDisplay(HtmlTextWriter writer, ViewContext viewContext, IWorkContextAccessor workContextAccessor);

        public virtual ActionResult DisplayCallback(Controller controller, WorkContext workContext)
        {
            return null;
        }

        public string GetDisplayCallbackUrl(UrlHelper urlHelper)
        {
            return urlHelper.Action("DisplayCallback", "Widget", new {area = Constants.Areas.Widgets, widgetId = Id});
        }

        public virtual string BuildDisplay(ViewContext viewContext, IWorkContextAccessor workContextAccessor)
        {
            using (var stringWriter = new StringWriter())
            using (var htmlTextWriter = new HtmlTextWriter(stringWriter))
            {
                BuildDisplay(htmlTextWriter, viewContext, workContextAccessor);
                return stringWriter.ToString();
            }
        }

        public virtual ActionResult BuildEditor(Controller controller, WorkContext workContext, ControlFormResult<IWidget> controlForm)
        {
            return controlForm;
        }

        public virtual ActionResult EditorCallback(Controller controller, WorkContext workContext)
        {
            return null;
        }

        public virtual void OnSaving(WorkContext workContext)
        {
        }

        IWidget IWidget.ShallowCopy()
        {
            return (IWidget) MemberwiseClone();
        }

        #endregion IWidget Members

        #region Helpers

        protected string ViewContent(ViewContext viewContext, string viewName, object model = null)
        {
            var controllerContext = new ControllerContext
            {
                RouteData = viewContext.RouteData,
                HttpContext = viewContext.HttpContext,
            };

            viewContext.ViewData.Model = model;

            var result = ViewEngines.Engines.FindPartialView(controllerContext, viewName);

            if (result != null && result.View != null)
            {
                using (var writer = new StringWriter())
                {
                    result.View.Render(viewContext, writer);
                    return writer.ToString();
                }
            }

            return null;
        }

        #endregion Helpers
    }
}