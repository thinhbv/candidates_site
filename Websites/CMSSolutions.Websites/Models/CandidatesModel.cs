namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class CandidatesModel
    {
        
        [ControlHidden()]
        public int Id { get; set; }
        
        [ControlText(Type=ControlText.TextBox, Required=true, MaxLength=250, ContainerCssClass=Constants.ContainerCssClassCol6, ContainerRowIndex=0)]
        public string full_name { get; set; }
        
        [ControlDatePicker(LabelText="birthday", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<System.DateTime> birthday { get; set; }
        
        [ControlText(Type=ControlText.TextBox, Required=true, MaxLength=50, ContainerCssClass=Constants.ContainerCssClassCol6, ContainerRowIndex=0)]
        public string mail_address { get; set; }
        
        [ControlText(Type=ControlText.TextBox, Required=false, MaxLength=11, ContainerCssClass=Constants.ContainerCssClassCol6, ContainerRowIndex=0)]
        public string phone_number { get; set; }
        
        [ControlText(Type=ControlText.TextBox, Required=false, MaxLength=500, ContainerCssClass=Constants.ContainerCssClassCol6, ContainerRowIndex=0)]
        public string address { get; set; }
        
        [ControlDatePicker(LabelText="start_working_date", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<System.DateTime> start_working_date { get; set; }
        
        [ControlNumeric(LabelText="hr_user_id", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<int> hr_user_id { get; set; }
        
        [ControlText(Type=ControlText.TextBox, Required=true, MaxLength=500, ContainerCssClass=Constants.ContainerCssClassCol6, ContainerRowIndex=0)]
        public string cv_path { get; set; }
        
        [ControlNumeric(LabelText="created_user_id", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int created_user_id { get; set; }
        
        [ControlDatePicker(LabelText="created_date", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.DateTime created_date { get; set; }
        
        [ControlNumeric(LabelText="updated_user_id", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<int> updated_user_id { get; set; }
        
        [ControlDatePicker(LabelText="updated_date", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<System.DateTime> updated_date { get; set; }
        
        public static implicit operator CandidatesModel(Candidates entity)
        {
            return new CandidatesModel
            {
                Id = entity.Id,
                full_name = entity.full_name,
                birthday = entity.birthday,
                mail_address = entity.mail_address,
                phone_number = entity.phone_number,
                address = entity.address,
                start_working_date = entity.start_working_date,
                hr_user_id = entity.hr_user_id,
                cv_path = entity.cv_path,
                created_user_id = entity.created_user_id,
                created_date = entity.created_date,
                updated_user_id = entity.updated_user_id,
                updated_date = entity.updated_date
            };
        }

    }
}
