namespace CMSSolutions.Websites.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    using CMSSolutions.Data;
    using CMSSolutions.Data.Entity;
    using System.Runtime.Serialization;
    
    
    [DataContract()]
    public class Candidates : BaseEntity<int>
    {
        
        [DataMember()]
        [DisplayName("full_name")]
        public string full_name { get; set; }
        
        [DataMember()]
        [DisplayName("birthday")]
        public System.Nullable<System.DateTime> birthday { get; set; }
        
        [DataMember()]
        [DisplayName("mail_address")]
        public string mail_address { get; set; }
        
        [DataMember()]
        [DisplayName("phone_number")]
        public string phone_number { get; set; }
        
        [DataMember()]
        [DisplayName("address")]
        public string address { get; set; }
        
        [DataMember()]
        [DisplayName("start_working_date")]
        public System.Nullable<System.DateTime> start_working_date { get; set; }
        
        [DataMember()]
        [DisplayName("hr_user_id")]
        public System.Nullable<int> hr_user_id { get; set; }
        
        [DataMember()]
        [DisplayName("cv_path")]
        public string cv_path { get; set; }
        
        [DataMember()]
        [DisplayName("created_user_id")]
        public int created_user_id { get; set; }
        
        [DataMember()]
        [DisplayName("created_date")]
        public System.DateTime created_date { get; set; }
        
        [DataMember()]
        [DisplayName("updated_user_id")]
        public System.Nullable<int> updated_user_id { get; set; }
        
        [DataMember()]
        [DisplayName("updated_date")]
        public System.Nullable<System.DateTime> updated_date { get; set; }
    }
    
    public class CandidatesMapping : EntityTypeConfiguration<Candidates>, IEntityTypeConfiguration
    {
        
        public CandidatesMapping()
        {
            this.ToTable("Module_Candidates");
            this.HasKey(m => m.Id);
            this.Property(m => m.full_name).IsRequired().HasMaxLength(250);
            this.Property(m => m.birthday);
            this.Property(m => m.mail_address).IsRequired().HasMaxLength(50);
            this.Property(m => m.phone_number).HasMaxLength(11);
            this.Property(m => m.address).HasMaxLength(500);
            this.Property(m => m.start_working_date);
            this.Property(m => m.hr_user_id);
            this.Property(m => m.cv_path).IsRequired().HasMaxLength(500);
            this.Property(m => m.created_user_id).IsRequired();
            this.Property(m => m.created_date).IsRequired();
            this.Property(m => m.updated_user_id);
            this.Property(m => m.updated_date);
        }
    }
}
