namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;

    public class LevelCandidatesModel
    {
        [ControlHidden()]
        public int Id { get; set; }

		[ControlHidden()]
		public int candidate_id { get; set; }

		[ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Skill", ContainerCssClass = "col-xs-8 col-md-6", ContainerRowIndex = 0)]
		public int language_id { get; set; }

		[ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Main Skill", ContainerCssClass = "col-xs-8 col-md-6", ContainerRowIndex = 1)]
		public int main_skill { get; set; }

		[ControlChoice(ControlChoice.DropDownList, Required = true, LabelText = "Level", ContainerCssClass = "col-xs-8 col-md-6", ContainerRowIndex = 2)]
		public int level_dev { get; set; }

		[ControlNumeric(LabelText = "Month of Experience", Required = true, MinimumValue = "1", MaximumValue = "500", ContainerCssClass = "col-xs-4 col-md-4", ContainerRowIndex = 3)]
		public int month { get; set; }

		[ControlText(LabelText = "Comment", Required = false, Type = ControlText.MultiText, Rows = 2, ContainerCssClass = Constants.ContainerCssClassCol12, ContainerRowIndex = 4)]
		public string notes { get; set; }

        public static implicit operator LevelCandidatesModel(LevelCandidates entity)
        {
            return new LevelCandidatesModel
            {
                Id = entity.Id,
                language_id = entity.language_id,
				main_skill= entity.main_skill,
                level_dev = entity.level_dev,
                month = entity.month,
                notes = entity.notes
            };
        }

    }
}
