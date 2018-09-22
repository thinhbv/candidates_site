using System;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists.Models
{
    public class ListModel
    {
        [ControlHidden]
        public int Id { get; set; }

        [ControlText(Required = true)]
        public string Name { get; set; }

        [ControlSlug(MaxLength = 255)]
        public string Url { get; set; }

        [ControlChoice(ControlChoice.DropDownList, LabelText = "Sort By")]
        public string Sorting { get; set; }

        [ControlNumeric(MinimumValue = "0", Required = true, LabelText = "Page Size")]
        public int PageSize { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Enabled")]
        public bool Enabled { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Enable Meta Tags")]
        public bool EnabledMetaTags { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Enable Comments")]
        public bool EnabledComments { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Auto Moderated Comments")]
        public bool AutoModeratedComments { get; set; }

        [ControlText(MaxLength = 255, LabelText = "Css Class")]
        public string CssClass { get; set; }

        [ControlText(Type = ControlText.MultiText, LabelText = "Summary Template")]
        public string SummaryTemplate { get; set; }

        [ControlText(Type = ControlText.MultiText, LabelText = "Display Template")]
        public string DetailTemplate { get; set; }

        public static implicit operator ListModel(List other)
        {
            return new ListModel
            {
                Id = other.Id,
                Name = other.Name,
                Url = other.Url,
                Enabled = other.Enabled,
                SummaryTemplate = other.SummaryTemplate,
                DetailTemplate = other.DetailTemplate,
                Sorting = other.Sorting,
                PageSize = other.PageSize,
                EnabledMetaTags = other.EnabledMetaTags,
                CssClass = other.CssClass,
                EnabledComments = other.EnabledComments,
                AutoModeratedComments = other.AutoModeratedComments
            };
        }
    }
}