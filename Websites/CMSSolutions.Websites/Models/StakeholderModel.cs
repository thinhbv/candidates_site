namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class StakeholderModel
    {
        
        [ControlHidden()]
        public int Id { get; set; }
        
        [ControlText(LabelText="Full Name", Type=ControlText.TextBox, Required=true, MaxLength=250, ContainerCssClass=Constants.ContainerCssClassCol6, ContainerRowIndex=0)]
        public string name { get; set; }

		[ControlText(LabelText = "Email Address", Type = ControlText.Email, Required = true, MaxLength = 50, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 0)]
        public string mail_address { get; set; }
        
        public static implicit operator StakeholderModel(Stakeholder entity)
        {
            return new StakeholderModel
            {
                Id = entity.Id,
                name = entity.name,
                mail_address = entity.mail_address
            };
        }

    }
}
