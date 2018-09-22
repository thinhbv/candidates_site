using System;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Pages.Models
{
    public class TranslationModel
    {
        [ControlHidden]
        public Guid Id { get; set; }

        [ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Language")]
        public string CultureCode { get; set; }
    }
}