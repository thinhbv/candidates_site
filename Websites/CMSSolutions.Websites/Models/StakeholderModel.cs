namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class StakeholderModel
    {
        
        [ControlHidden()]
        public int Id { get; set; }
        
        [ControlText(Type=ControlText.TextBox, Required=true, MaxLength=250, ContainerCssClass=Constants.ContainerCssClassCol6, ContainerRowIndex=0)]
        public string name { get; set; }
        
        [ControlText(Type=ControlText.TextBox, Required=true, MaxLength=50, ContainerCssClass=Constants.ContainerCssClassCol6, ContainerRowIndex=0)]
        public string mail_address { get; set; }
        
        [ControlDatePicker(LabelText="created_date", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.DateTime created_date { get; set; }
        
        [ControlDatePicker(LabelText="updated_date", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<System.DateTime> updated_date { get; set; }
        
        public static implicit operator StakeholderModel(Stakeholder entity)
        {
            return new StakeholderModel
            {
                Id = entity.Id,
                name = entity.name,
                mail_address = entity.mail_address,
                created_date = entity.created_date,
                updated_date = entity.updated_date
            };
        }

    }
}
