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
    public class Candidates : BaseEntity<int>
    {
        [DataMember()]
        [DisplayName("full_name")]
        public string full_name { get; set; }
        
        [DataMember()]
        [DisplayName("birthday")]
        public System.Nullable<System.DateTime> birthday { get; set; }

		[NotMapped]
		[DisplayName(Constants.NotMapped)]
		public string display_birthday { get { return CMSSolutions.Websites.Extensions.Utilities.DateString(birthday); } }
        
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

		[NotMapped]
		[DisplayName(Constants.NotMapped)]
		public string display_working_date { get { return CMSSolutions.Websites.Extensions.Utilities.DateString(start_working_date); } }

        [DataMember()]
        [DisplayName("hr_user_id")]
		public int hr_user_id { get; set; }

		[NotMapped]
		[DisplayName("hr_full_name")]
		public string hr_full_name { get; set; }
        
        [DataMember()]
        [DisplayName("cv_path")]
        public string cv_path { get; set; }
        
        [DataMember()]
        [DisplayName("created_user_id")]
        public int created_user_id { get; set; }

		[NotMapped]
		[DisplayName("created_full_name")]
		public string created_full_name { get; set; }
        
        [DataMember()]
        [DisplayName("created_date")]
        public System.DateTime created_date { get; set; }

		[NotMapped]
		[DisplayName(Constants.NotMapped)]
		public string display_created_date { get { return CMSSolutions.Websites.Extensions.Utilities.DateString(created_date); } }

        [DataMember()]
        [DisplayName("updated_user_id")]
        public System.Nullable<int> updated_user_id { get; set; }
        
        [DataMember()]
        [DisplayName("updated_date")]
        public System.Nullable<System.DateTime> updated_date { get; set; }

		[NotMapped]
		[DisplayName(Constants.NotMapped)]
		public string display_updated_date { get { return CMSSolutions.Websites.Extensions.Utilities.DateString(updated_date); } }

		[DataMember()]
		[DisplayName("notes")]
		public string notes { get;set;}

		[NotMapped]
		[DisplayName("is_employee")]
		public bool is_employee
		{
			get
			{
				if (start_working_date != null)
				{
					if (start_working_date.Value > DateTime.Now)
					{
						return true;
					}
				}

				return false;
			} 
		}
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
            this.Property(m => m.hr_user_id).IsRequired();
            this.Property(m => m.cv_path).IsRequired().HasMaxLength(500);
            this.Property(m => m.created_user_id).IsRequired();
            this.Property(m => m.created_date).IsRequired();
            this.Property(m => m.updated_user_id);
            this.Property(m => m.updated_date);
			this.Property(m => m.notes);
        }
    }
}
