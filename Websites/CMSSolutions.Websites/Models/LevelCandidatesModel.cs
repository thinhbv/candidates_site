namespace CMSSolutions.Websites.Models
{
    using System;
    using CMSSolutions.Web.UI.ControlForms;
    using CMSSolutions.Websites.Entities;
    
    
    public class LevelCandidatesModel
    {
        
        [ControlHidden()]
        public int Id { get; set; }
        
        [ControlNumeric(LabelText="candidate_id", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int candidate_id { get; set; }
        
        [ControlNumeric(LabelText="language_id", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int language_id { get; set; }
        
        [ControlNumeric(LabelText="level_dev", Required=true, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public int level_dev { get; set; }
        
        [ControlDatePicker(LabelText="created_date", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<System.DateTime> created_date { get; set; }
        
        [ControlDatePicker(LabelText="updated_date", Required=false, ContainerCssClass=Constants.ContainerCssClassCol3, ContainerRowIndex=0)]
        public System.Nullable<System.DateTime> updated_date { get; set; }
        
        public static implicit operator LevelCandidatesModel(LevelCandidates entity)
        {
            return new LevelCandidatesModel
            {
                Id = entity.Id,
                candidate_id = entity.candidate_id,
                language_id = entity.language_id,
                level_dev = entity.level_dev,
                created_date = entity.created_date,
                updated_date = entity.updated_date
            };
        }

    }
}
