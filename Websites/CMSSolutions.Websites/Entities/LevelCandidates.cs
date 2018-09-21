namespace CMSSolutions.Websites.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    using CMSSolutions.Data;
    using CMSSolutions.Data.Entity;
    using System.Runtime.Serialization;
	using System.ComponentModel.DataAnnotations.Schema;
    
    
    [DataContract()]
    public class LevelCandidates : BaseEntity<int>
    {
        
        [DataMember()]
        [DisplayName("candidate_id")]
        public int candidate_id { get; set; }
        
        [DataMember()]
        [DisplayName("language_id")]
        public int language_id { get; set; }

		[NotMapped]
		[DisplayName("language_name")]
		public string language_name { get;set; }
        
        [DataMember()]
        [DisplayName("level_dev")]
        public int level_dev { get; set; }

		[NotMapped]
		[DisplayName("level_name")]
		public string level_name { get; set; }

        [DataMember()]
		[DisplayName("month")]
		public int month { get; set; }

		[DataMember()]
		[DisplayName("main_skill")]
		public int main_skill { get; set; }

		[NotMapped]
		[DisplayName(Constants.NotMapped)]
		public bool is_main
		{
			get 
			{
				if (main_skill == 1)
				{
					return true;
				}

				return false;
			}
		}

        [DataMember()]
        [DisplayName("created_date")]
        public System.Nullable<System.DateTime> created_date { get; set; }
        
        [DataMember()]
        [DisplayName("updated_date")]
        public System.Nullable<System.DateTime> updated_date { get; set; }

		[DataMember()]
		[DisplayName("notes")]
		public string notes { get; set; }
    }
    
    public class LevelCandidatesMapping : EntityTypeConfiguration<LevelCandidates>, IEntityTypeConfiguration
    {
        
        public LevelCandidatesMapping()
        {
            this.ToTable("Module_LevelCandidates");
            this.HasKey(m => m.Id);
            this.Property(m => m.candidate_id).IsRequired();
            this.Property(m => m.language_id).IsRequired();
            this.Property(m => m.level_dev).IsRequired();
			this.Property(m => m.month).IsRequired();
			this.Property(m => m.main_skill).IsRequired();
            this.Property(m => m.created_date);
            this.Property(m => m.updated_date);
        }
    }
}
