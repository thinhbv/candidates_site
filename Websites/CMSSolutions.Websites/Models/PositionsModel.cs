namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class PositionsModel
    {
        
        [ControlHidden()]
        public int Id { get; set; }

		[ControlText(LabelText = "Positions Name", Type = ControlText.TextBox, Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 0)]
        public string pos_name { get; set; }

        public static implicit operator PositionsModel(Positions entity)
        {
            return new PositionsModel
            {
                Id = entity.Id,
                pos_name = entity.pos_name
            };
        }

    }
}
