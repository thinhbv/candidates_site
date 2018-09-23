namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class QuestionsModel
    {
        
        [ControlHidden()]
        public int Id { get; set; }

		[ControlChoice(ControlChoice.DropDownList, LabelText = "Language", CssClass = Extensions.Constants.CssControlCustom, Required = true, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 0)]
        public int language_id { get; set; }

		[ControlChoice(ControlChoice.DropDownList, LabelText = "Position", AllowMultiple = true, CssClass = Extensions.Constants.CssControlCustom, EnableChosen = true, Required = true, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 2)]
		public string[] types { get; set; }

		[ControlText(LabelText = "Content Question", Type = ControlText.MultiText, Rows = 2, Required = true, MaxLength = 250, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 0)]
        public string content { get; set; }
        
        public static implicit operator QuestionsModel(Questions entity)
        {
			string[] temp;
			if (entity.types.Contains(","))
			{
				temp = entity.types.Split(Char.Parse(","));
			}
			else
			{
				temp = new string[] {entity.types};
			}
            return new QuestionsModel
            {
                Id = entity.Id,
                language_id = entity.language_id,
                content = entity.content,
				types = temp,
            };
        }

    }
}
