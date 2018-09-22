namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class ScheduleInterviewModel
    {
        
        [ControlHidden()]
        public int Id { get; set; }
        
        [ControlNumeric(LabelText="pos_id", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int pos_id { get; set; }
        
        [ControlNumeric(LabelText="candidate_id", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int candidate_id { get; set; }
        
        [ControlDatePicker(LabelText="interview_date", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.DateTime interview_date { get; set; }
        
        [ControlDatePicker(LabelText="created_date", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.DateTime created_date { get; set; }
        
        [ControlDatePicker(LabelText="updated_date", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<System.DateTime> updated_date { get; set; }
        
        public static implicit operator ScheduleInterviewModel(ScheduleInterview entity)
        {
            return new ScheduleInterviewModel
            {
                Id = entity.Id,
                pos_id = entity.pos_id,
                candidate_id = entity.candidate_id,
                interview_date = entity.interview_date,
                created_date = entity.created_date,
                updated_date = entity.updated_date
            };
        }

    }
}
