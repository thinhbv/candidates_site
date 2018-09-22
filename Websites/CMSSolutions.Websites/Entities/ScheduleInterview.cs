namespace CMSSolutions.Websites.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    using CMSSolutions.Data;
    using CMSSolutions.Data.Entity;
    using System.Runtime.Serialization;
    
    
    [DataContract()]
    public class ScheduleInterview : BaseEntity<int>
    {
        
        [DataMember()]
        [DisplayName("pos_id")]
        public int pos_id { get; set; }
        
        [DataMember()]
        [DisplayName("candidate_id")]
        public int candidate_id { get; set; }
        
        [DataMember()]
        [DisplayName("interview_date")]
        public System.DateTime interview_date { get; set; }
        
        [DataMember()]
        [DisplayName("created_date")]
        public System.DateTime created_date { get; set; }
        
        [DataMember()]
        [DisplayName("updated_date")]
        public System.Nullable<System.DateTime> updated_date { get; set; }
		
		[DataMember()]
		[DisplayName("list_questions")]
		public string list_questions { get; set; }
    }
    
    public class ScheduleInterviewMapping : EntityTypeConfiguration<ScheduleInterview>, IEntityTypeConfiguration
    {
        
        public ScheduleInterviewMapping()
        {
            this.ToTable("Module_ScheduleInterview");
            this.HasKey(m => m.Id);
            this.Property(m => m.pos_id).IsRequired();
            this.Property(m => m.candidate_id).IsRequired();
            this.Property(m => m.interview_date).IsRequired();
            this.Property(m => m.created_date).IsRequired();
			this.Property(m => m.list_questions).IsRequired();
            this.Property(m => m.updated_date);
        }
    }
}
