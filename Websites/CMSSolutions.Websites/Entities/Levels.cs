namespace CMSSolutions.Websites.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    using CMSSolutions.Data;
    using CMSSolutions.Data.Entity;
    using System.Runtime.Serialization;
    
    
    [DataContract()]
    public class Levels : BaseEntity<int>
    {
        
        [DataMember()]
        [DisplayName("name")]
        public string name { get; set; }
        
        [DataMember()]
        [DisplayName("notes")]
        public string notes { get; set; }
        
        [DataMember()]
        [DisplayName("created_date")]
        public System.Nullable<System.DateTime> created_date { get; set; }
        
        [DataMember()]
        [DisplayName("updated_date")]
        public System.Nullable<System.DateTime> updated_date { get; set; }
    }
    
    public class LevelsMapping : EntityTypeConfiguration<Levels>, IEntityTypeConfiguration
    {
        
        public LevelsMapping()
        {
            this.ToTable("Module_Levels");
            this.HasKey(m => m.Id);
            this.Property(m => m.name).IsRequired().HasMaxLength(250);
            this.Property(m => m.notes).HasMaxLength(1073741823);
            this.Property(m => m.created_date);
            this.Property(m => m.updated_date);
        }
    }
}
