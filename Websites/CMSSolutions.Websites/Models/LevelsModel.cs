namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class LevelsModel
    {
        
        [ControlHidden()]
        public int Id { get; set; }
        
        [ControlText(LabelText="Level Name", Type=ControlText.TextBox, Required=true, MaxLength=250, ContainerCssClass=Constants.ContainerCssClassCol6, ContainerRowIndex=0)]
        public string name { get; set; }

		[ControlText(LabelText = "Notes", Required = false, Type = ControlText.MultiText, Rows = 2, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 1)]
		public string notes { get; set; }

        public static implicit operator LevelsModel(Levels entity)
        {
            return new LevelsModel
            {
                Id = entity.Id,
                name = entity.name,
                notes = entity.notes
            };
        }

    }
}
