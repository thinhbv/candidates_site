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
    public class Questions : BaseEntity<int>
    {

		[DataMember()]
		[DisplayName("level_id")]
		public int level_id { get; set; }

        [DataMember()]
		[DisplayName("language_id")]
        public int language_id { get; set; }

		[NotMapped]
		[DisplayName("language_name")]
		public string language_name { get; set; }
        
        [DataMember()]
		[DisplayName("content")]
        public string content { get; set; }

		[DataMember()]
		[DisplayName("types")]
		public string types { get; set; }

		[NotMapped]
		[DisplayName("type_name")]
		public string type_name { get; set; }
        
        [DataMember()]
        [DisplayName("creator")]
        public int creator { get; set; }
        
        [DataMember()]
        [DisplayName("created_date")]
        public System.DateTime created_date { get; set; }
        
        [DataMember()]
        [DisplayName("updated_date")]
        public System.Nullable<System.DateTime> updated_date { get; set; }
    }
    
    public class QuestionsMapping : EntityTypeConfiguration<Questions>, IEntityTypeConfiguration
    {
        
        public QuestionsMapping()
        {
            this.ToTable("Module_Questions");
            this.HasKey(m => m.Id);
            this.Property(m => m.language_id).IsRequired();
            this.Property(m => m.content).IsRequired().HasMaxLength(250);
            this.Property(m => m.creator).IsRequired();
            this.Property(m => m.created_date).IsRequired();
            this.Property(m => m.updated_date);
        }
    }
}
