using System;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Widgets.Models
{
    public class WidgetModel
    {
        public int Id { get; set; }

        [ControlHidden]
        public int? PageId { get; set; }

        [ControlText(Required = true, MaxLength = 255, ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 0)]
        public string Title { get; set; }

        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Widget Type", CssClass = "uniform", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 0)]
        public string WidgetType { get; set; }

        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Zone", CssClass = "uniform", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 1)]
        public int ZoneId { get; set; }

        [ControlNumeric(Required = true, ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 1)]
        public int Order { get; set; }

        [ControlChoice(ControlChoice.CheckBox)]
        public bool Enabled { get; set; }

        public string DisplayCondition { get; set; }
    }
}