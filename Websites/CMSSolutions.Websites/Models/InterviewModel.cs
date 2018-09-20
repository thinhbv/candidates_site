namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class InterviewModel
    {
        
        [ControlHidden()]
        public int Id { get; set; }
        
        [ControlNumeric(LabelText="candidate_id", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int candidate_id { get; set; }
        
        [ControlNumeric(LabelText="round_id", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int round_id { get; set; }
        
        [ControlNumeric(LabelText="position_id", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int position_id { get; set; }
        
        [ControlDatePicker(LabelText="interview_date_plan", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<System.DateTime> interview_date_plan { get; set; }
        
        [ControlDatePicker(LabelText="interview_date", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<System.DateTime> interview_date { get; set; }
        
        [ControlNumeric(LabelText="interviewer_id", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int interviewer_id { get; set; }
        
        [ControlText(Type=ControlText.TextBox, Required=false, MaxLength=1073741823, ContainerCssClass=Constants.ContainerCssClassCol6, ContainerRowIndex=0)]
        public string evaluation { get; set; }
        
        [ControlNumeric(LabelText="status", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int status { get; set; }
        
        [ControlText(Type=ControlText.TextBox, Required=false, MaxLength=250, ContainerCssClass=Constants.ContainerCssClassCol6, ContainerRowIndex=0)]
        public string interview_result { get; set; }
        
        [ControlDatePicker(LabelText="created_date", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.DateTime created_date { get; set; }
        
        [ControlNumeric(LabelText="created_user_id", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int created_user_id { get; set; }
        
        [ControlDatePicker(LabelText="updated_date", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<System.DateTime> updated_date { get; set; }
        
        [ControlNumeric(LabelText="updated_user_id", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<int> updated_user_id { get; set; }
        
        public static implicit operator InterviewModel(Interview entity)
        {
            return new InterviewModel
            {
                Id = entity.Id,
                candidate_id = entity.candidate_id,
                round_id = entity.round_id,
                position_id = entity.position_id,
                interview_date_plan = entity.interview_date_plan,
                interview_date = entity.interview_date,
                interviewer_id = entity.interviewer_id,
                evaluation = entity.evaluation,
                status = entity.status,
                interview_result = entity.interview_result,
                created_date = entity.created_date,
                created_user_id = entity.created_user_id,
                updated_date = entity.updated_date,
                updated_user_id = entity.updated_user_id
            };
        }

    }
}
