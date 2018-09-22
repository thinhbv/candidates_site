namespace CMSSolutions.Websites.Entities
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel;
    using CMSSolutions.Data;
    using CMSSolutions.Data.Entity;
    using System.Runtime.Serialization;
    
    
    [DataContract()]
    public class MailTemplates : BaseEntity<int>
    {
        
        [DataMember()]
        [DisplayName("name")]
        public string name { get; set; }
        
        [DataMember()]
        [DisplayName("url_template")]
        public string url_template { get; set; }
    }
    
    public class MailTemplatesMapping : EntityTypeConfiguration<MailTemplates>, IEntityTypeConfiguration
    {
        
        public MailTemplatesMapping()
        {
            this.ToTable("Module_MailTemplates");
            this.HasKey(m => m.Id);
            this.Property(m => m.name).IsRequired().HasMaxLength(250);
            this.Property(m => m.url_template).IsRequired().HasMaxLength(500);
        }
    }
}
