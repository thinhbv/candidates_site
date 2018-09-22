using System;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists.Models
{
    public class ListFieldModel
    {
        [ControlHidden]
        public int Id { get; set; }

        [ControlText(Required = true)]
        public string Title { get; set; }

        [ControlText(Required = true)]
        public string Name { get; set; }

        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Field Type")]
        public string FieldType { get; set; }

        [ControlHidden]
        public int ListId { get; set; }

        [ControlNumeric]
        public int Position { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Required")]
        public bool Required { get; set; }

        public static implicit operator ListFieldModel(ListField other)
        {
            return new ListFieldModel
            {
                Id = other.Id,
                Name = other.Name,
                FieldType = other.FieldType,
                ListId = other.ListId,
                Position = other.Position,
                Required = other.Required
            };
        }
    }
}