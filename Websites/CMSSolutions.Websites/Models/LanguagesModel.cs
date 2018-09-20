namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;

    public class LanguagesModel
    {
        [ControlHidden()]
        public int Id { get; set; }

		[ControlText(LabelText = "Language Name", Type = ControlText.TextBox, Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 0)]
        public string name { get; set; }
        
        public static implicit operator LanguagesModel(Languages entity)
        {
            return new LanguagesModel
            {
                Id = entity.Id,
                name = entity.name
            };
        }
    }
}
