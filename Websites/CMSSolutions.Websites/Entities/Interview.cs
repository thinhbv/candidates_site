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
    public class Interview : BaseEntity<int>
    {
        
        [DataMember()]
        [DisplayName("candidate_id")]
        public int candidate_id { get; set; }
        
        [DataMember()]
        [DisplayName("round_id")]
        public int round_id { get; set; }
        
        [DataMember()]
        [DisplayName("position_id")]
        public int position_id { get; set; }

		[NotMapped]
		[DisplayName("position_name")]
		public string position_name { get; set; }
        
        [DataMember()]
        [DisplayName("interview_date_plan")]
        public System.Nullable<System.DateTime> interview_date_plan { get; set; }
        
        [DataMember()]
        [DisplayName("interview_date")]
        public System.Nullable<System.DateTime> interview_date { get; set; }
        
        [DataMember()]
        [DisplayName("interviewer_id")]
        public int interviewer_id { get; set; }

		[NotMapped]
		[DisplayName("interviewer_name")]
		public string interviewer_name { get; set; }
        
        [DataMember()]
        [DisplayName("evaluation")]
        public string evaluation { get; set; }
        
        [DataMember()]
        [DisplayName("status")]
        public int status { get; set; }
        
        [DataMember()]
        [DisplayName("interview_result")]
        public string interview_result { get; set; }
        
        [DataMember()]
        [DisplayName("created_date")]
        public System.DateTime created_date { get; set; }
        
        [DataMember()]
        [DisplayName("created_user_id")]
        public int created_user_id { get; set; }
        
        [DataMember()]
        [DisplayName("updated_date")]
        public System.Nullable<System.DateTime> updated_date { get; set; }
        
        [DataMember()]
        [DisplayName("updated_user_id")]
        public System.Nullable<int> updated_user_id { get; set; }
    }
    
    public class InterviewMapping : EntityTypeConfiguration<Interview>, IEntityTypeConfiguration
    {
        
        public InterviewMapping()
        {
            this.ToTable("Module_Interview");
            this.HasKey(m => m.Id);
            this.Property(m => m.candidate_id).IsRequired();
            this.Property(m => m.round_id).IsRequired();
            this.Property(m => m.position_id).IsRequired();
            this.Property(m => m.interview_date_plan);
            this.Property(m => m.interview_date);
            this.Property(m => m.interviewer_id).IsRequired();
            this.Property(m => m.evaluation);
            this.Property(m => m.status).IsRequired();
            this.Property(m => m.interview_result).HasMaxLength(250);
            this.Property(m => m.created_date).IsRequired();
            this.Property(m => m.created_user_id).IsRequired();
            this.Property(m => m.updated_date);
            this.Property(m => m.updated_user_id);
        }
    }
}
