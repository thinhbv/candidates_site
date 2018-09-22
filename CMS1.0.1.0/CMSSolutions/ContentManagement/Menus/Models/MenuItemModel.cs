using System;
using CMSSolutions.ContentManagement.Menus.Domain;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Menus.Models
{
    public class MenuItemModel
    {
        [ControlHidden]
        public int Id { get; set; }

        [ControlHidden]
        public int MenuId { get; set; }

        [ControlText(Required = true, MaxLength = 255, ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 0)]
        public string Text { get; set; }

        [ControlText(Required = true, MaxLength = 255, ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 0)]
        public string Url { get; set; }

        public bool IsExternalUrl { get; set; }

        [ControlText(MaxLength = 255, ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 1)]
        public string Description { get; set; }

        [ControlChoice(ControlChoice.DropDownList, LabelText = "Parent", CssClass = "uniform", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 1)]
        public int? ParentId { get; set; }

        [ControlText(MaxLength = 255, LabelText = "Css Class", ContainerCssClass = "col-xs-4 col-md-4", ContainerRowIndex = 2)]
        public string CssClass { get; set; }

        [ControlNumeric(Required = true, ContainerCssClass = "col-xs-4 col-md-4", ContainerRowIndex = 2)]
        public int Position { get; set; }

        [ControlChoice(ControlChoice.CheckBox, ContainerCssClass = "col-xs-4 col-md-4", ContainerRowIndex = 2, LabelText = "", AppendText = "Enabled")]
        public bool Enabled { get; set; }

        public int Rank { get; set; }

        public static implicit operator MenuItemModel(MenuItem other)
        {
            return new MenuItemModel
            {
                Id = other.Id,
                MenuId = other.MenuId,
                Position = other.Position,
                Text = other.Text,
                Description = other.Description,
                Url = other.Url,
                IsExternalUrl = other.IsExternalUrl,
                CssClass = other.CssClass,
                ParentId = other.ParentId,
                Enabled = other.Enabled
            };
        }
    }
}