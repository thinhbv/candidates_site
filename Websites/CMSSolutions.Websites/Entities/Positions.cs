namespace CMSSolutions.Websites.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    using CMSSolutions.Data;
    using CMSSolutions.Data.Entity;
    using System.Runtime.Serialization;
    
    
    [DataContract()]
    public class Positions : BaseEntity<int>
    {
        
        [DataMember()]
        [DisplayName("pos_name")]
        public string pos_name { get; set; }
        
        [DataMember()]
        [DisplayName("created_date")]
        public System.DateTime created_date { get; set; }
        
        [DataMember()]
        [DisplayName("updated_date")]
        public System.Nullable<System.DateTime> updated_date { get; set; }

		[DataMember()]
		[DisplayName("content")]
		public string content { get; set; }

		[DataMember()]
		[DisplayName("start_date")]
		public DateTime start_date { get; set; }

		[DataMember()]
		[DisplayName("end_date")]
		public DateTime end_date { get; set; }
    }
    
    public class PositionsMapping : EntityTypeConfiguration<Positions>, IEntityTypeConfiguration
    {
        
        public PositionsMapping()
        {
            this.ToTable("Module_Positions");
            this.HasKey(m => m.Id);
            this.Property(m => m.pos_name).IsRequired().HasMaxLength(250);
            this.Property(m => m.created_date).IsRequired();
			this.Property(m => m.start_date).IsRequired();
			this.Property(m => m.end_date).IsRequired();
            this.Property(m => m.updated_date);
        }
    }
}
