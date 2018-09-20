namespace CMSSolutions.Websites.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    using CMSSolutions.Data;
    using CMSSolutions.Data.Entity;
    using System.Runtime.Serialization;
    
    
    [DataContract()]
    public class LevelCandidates : BaseEntity<int>
    {
        
        [DataMember()]
        [DisplayName("candidate_id")]
        public int candidate_id { get; set; }
        
        [DataMember()]
        [DisplayName("language_id")]
        public int language_id { get; set; }
        
        [DataMember()]
        [DisplayName("level_dev")]
        public int level_dev { get; set; }
        
        [DataMember()]
        [DisplayName("created_date")]
        public System.Nullable<System.DateTime> created_date { get; set; }
        
        [DataMember()]
        [DisplayName("updated_date")]
        public System.Nullable<System.DateTime> updated_date { get; set; }
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
            this.Property(m => m.created_date);
            this.Property(m => m.updated_date);
        }
    }
}
