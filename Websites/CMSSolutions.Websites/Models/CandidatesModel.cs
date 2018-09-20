namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class CandidatesModel
    {
        [ControlHidden()]
        public int Id { get; set; }
        
        [ControlText(LabelText="Full Name", Type=ControlText.TextBox, Required=true, MaxLength=250, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public string full_name { get; set; }

		[ControlText(LabelText = "Email", Type = ControlText.Email, Required = true, MaxLength = 50, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
		public string mail_address { get; set; }

		[ControlText(LabelText = "Phone Number", Type = ControlText.TextBox, Required = false, MaxLength = 11, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
		public string phone_number { get; set; }

		[ControlDatePicker(LabelText = "Birthday", CssClass = CMSSolutions.Websites.Extensions.Constants.DateCustomCss, Required = false, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 0)]
        public string birthday { get; set; }

		[ControlText(LabelText = "Address", Type = ControlText.TextBox, Required = false, MaxLength = 500, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 1)]
		public string address { get; set; }

		[ControlFileUpload(EnableFineUploader = true, Required = true, LabelText = "Choose CV File", ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 2, ShowThumbnail = false)]
        public string cv_path { get; set; }

		[ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "HR Recipient", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex =2)]
		public int hr_user_id { get; set; }

		[ControlDatePicker(LabelText = "Start Working Date", CssClass = CMSSolutions.Websites.Extensions.Constants.DateCustomCss, Required = false, ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 2)]
		public string start_working_date { get; set; }

		[ControlText(LabelText = "Notes", Required = true, Type = ControlText.MultiText, Rows = 2, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 3)]
		public string notes { get; set; }

        public static implicit operator CandidatesModel(Candidates entity)
        {
            var model = new CandidatesModel
            {
                Id = entity.Id,
                full_name = entity.full_name,
                mail_address = entity.mail_address,
                phone_number = entity.phone_number,
                address = entity.address,
                hr_user_id = entity.hr_user_id,
                cv_path = entity.cv_path,
				notes = entity.notes
            };

			if (entity.birthday != null)
			{
				model.birthday = entity.birthday.Value.ToString(Extensions.Constants.DateTimeFomat);
			}

			if (entity.start_working_date != null)
			{
				model.start_working_date = entity.birthday.Value.ToString(Extensions.Constants.DateTimeFomat);
			}

			return model;
        }

    }
}
