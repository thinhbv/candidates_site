namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class PositionsModel
    {
        
        [ControlHidden()]
        public int Id { get; set; }

		[ControlDatePicker(LabelText = "Start Date", CssClass = CMSSolutions.Websites.Extensions.Constants.DateCustomCss, Required = true, ContainerCssClass = "col-xs-6 col-md-3", ContainerRowIndex = 0)]
		public DateTime start_date { get; set; }

		[ControlDatePicker(LabelText = "End Date", CssClass = CMSSolutions.Websites.Extensions.Constants.DateCustomCss, Required = true, ContainerCssClass = "col-xs-6 col-md-3", ContainerRowIndex = 0)]
		public DateTime end_date { get; set; }


		[ControlText(LabelText = "Recruitment Position", Type = ControlText.TextBox, Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 1)]
        public string pos_name { get; set; }

		[ControlText(LabelText = "Content", Required = true, Type = ControlText.RichText, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 2)]
		public string content { get; set; }

        public static implicit operator PositionsModel(Positions entity)
        {
            return new PositionsModel
            {
                Id = entity.Id,
                pos_name = entity.pos_name,
				content = entity.content,
				start_date = entity.start_date,
				end_date = entity.end_date
            };
        }
    }
}
