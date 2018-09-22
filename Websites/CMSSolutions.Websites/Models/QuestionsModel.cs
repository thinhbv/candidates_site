namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class QuestionsModel
    {
        
        [ControlHidden()]
        public int Id { get; set; }
        
        [ControlNumeric(LabelText="language_id", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int language_id { get; set; }
        
        [ControlText(Type=ControlText.TextBox, Required=true, MaxLength=250, ContainerCssClass=Constants.ContainerCssClassCol6, ContainerRowIndex=0)]
        public string content { get; set; }
        
        [ControlNumeric(LabelText="creator", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int creator { get; set; }
        
        [ControlDatePicker(LabelText="created_date", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.DateTime created_date { get; set; }
        
        [ControlDatePicker(LabelText="updated_date", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<System.DateTime> updated_date { get; set; }
        
        public static implicit operator QuestionsModel(Questions entity)
        {
            return new QuestionsModel
            {
                Id = entity.Id,
                language_id = entity.language_id,
                content = entity.content,
                creator = entity.creator,
                created_date = entity.created_date,
                updated_date = entity.updated_date
            };
        }

    }
}
