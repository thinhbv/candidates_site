namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class MailTemplatesModel
    {
        
        [ControlHidden()]
        public int Id { get; set; }

		[ControlText(LabelText = "Template Name", Type = ControlText.TextBox, Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 0)]
        public string name { get; set; }

		[ControlFileUpload(EnableFineUploader = true, Required = true, LabelText = "Choose Template File", ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 1, ShowThumbnail = false)]
		public string url_template { get; set; }
        
        public static implicit operator MailTemplatesModel(MailTemplates entity)
        {
            return new MailTemplatesModel
            {
                Id = entity.Id,
                name = entity.name,
                url_template = entity.url_template
            };
        }
    }
}
