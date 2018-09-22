using System;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists.Models
{
    public class ListCategoryModel
    {
        [ControlHidden]
        public int Id { get; set; }

        [ControlText(Required = true)]
        public string Name { get; set; }

        [ControlSlug(MaxLength = 255)]
        public string Url { get; set; }

        public string FullUrl { get; set; }

        [ControlNumeric(Required = true)]
        public int Position { get; set; }

        [ControlChoice(ControlChoice.DropDownList, LabelText = "Parent")]
        public int? ParentId { get; set; }

        [ControlHidden]
        public int ListId { get; set; }

        public static implicit operator ListCategoryModel(ListCategory other)
        {
            return new ListCategoryModel
            {
                Id = other.Id,
                Name = other.Name,
                Url = other.Url,
                FullUrl = other.FullUrl,
                Position = other.Position,
                ListId = other.ListId,
                ParentId = other.ParentId
            };
        }
    }
}