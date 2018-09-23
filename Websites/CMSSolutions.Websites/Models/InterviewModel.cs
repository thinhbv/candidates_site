namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class InterviewModel
    {
        
        [ControlHidden()]
        public int Id { get; set; }

		 [ControlHidden()]
		public int candidate_id { get; set; }

        [ControlNumeric(LabelText="Candidate Name", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public string candidate_name { get; set; }
        
        [ControlNumeric(LabelText="Round", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int round_id { get; set; }
        
        [ControlNumeric(LabelText="Position", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int position_id { get; set; }
        
        [ControlDatePicker(LabelText="Interview Plan Date", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<System.DateTime> interview_date_plan { get; set; }

        [ControlNumeric(LabelText="Interviewer", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int interviewer_id { get; set; }

		[ControlText(LabelText = "Evaluation", Type = ControlText.TextBox, Required = false, MaxLength = 1073741823, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 0)]
        public string evaluation { get; set; }
        
        [ControlNumeric(LabelText="Status", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int status { get; set; }

		[ControlText(LabelText = "Interviewer Result", Type = ControlText.TextBox, Required = false, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 0)]
        public string interview_result { get; set; }
        
        public static implicit operator InterviewModel(Interview entity)
        {
            return new InterviewModel
            {
                Id = entity.Id,
                candidate_id = entity.candidate_id,
                round_id = entity.round_id,
                position_id = entity.position_id,
                interview_date_plan = entity.interview_date_plan,
                interviewer_id = entity.interviewer_id,
                evaluation = entity.evaluation,
                status = entity.status,
                interview_result = entity.interview_result
            };
        }

    }
}
