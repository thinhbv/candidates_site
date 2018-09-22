namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class PositionsModel
    {
        
        [ControlHidden()]
        public int Id { get; set; }

		[ControlText(LabelText = "Position Name", Type = ControlText.TextBox, Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 0)]
        public string pos_name { get; set; }

		[ControlDatePicker(LabelText = "Start Date", CssClass = CMSSolutions.Websites.Extensions.Constants.DateCustomCss, Required = true, ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 1)]
		public DateTime start_date { get; set; }

		[ControlDatePicker(LabelText = "End Date", CssClass = CMSSolutions.Websites.Extensions.Constants.DateCustomCss, Required = true, ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 1)]
		public DateTime end_date { get; set; }

		[ControlText(LabelText = "Content", Required = true, Type = ControlText.MultiText, Rows=2, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 2)]
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
