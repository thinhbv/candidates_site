using System;
using CMSSolutions.ContentManagement.Aliases.Domain;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Aliases.Models
{
    public class AliasModel : BaseModel<int>
    {
        [ControlText(LabelText = "Alias Path")]
        public string Path { get; set; }

        [ControlText(LabelText = "Route Path")]
        public string Source { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "Enabled")]
        public bool IsEnabled { get; set; }

        public static implicit operator AliasModel(Alias other)
        {
            if (other == null)
            {
                return null;
            }

            return new AliasModel
            {
                Id = other.Id,
                Path = other.Path,
                Source = other.Source,
                IsEnabled = other.IsEnabled
            };
        }
    }
}