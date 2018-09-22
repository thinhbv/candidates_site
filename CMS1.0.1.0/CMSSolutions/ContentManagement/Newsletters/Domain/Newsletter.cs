using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;

namespace CMSSolutions.ContentManagement.Newsletters.Domain
{
    [DataContract]
    public class Newsletter : BaseEntity<Guid>, ILocalizableEntity<Guid>
    {
        [DataMember]
        [DisplayName("Title")]
        public string Title { get; set; }

        [DataMember]
        [DisplayName("BodyContent")]
        public string BodyContent { get; set; }

        [DataMember]
        [DisplayName("DateCreated")]
        public DateTime DateCreated { get; set; }

        [DataMember]
        [DisplayName("CultureCode")]
        public string CultureCode { get; set; }

        [DataMember]
        [DisplayName("RefId")]
        public Guid? RefId { get; set; }
    }

    [Feature(Constants.Areas.Newsletters)]
    public class NewsletterMap : EntityTypeConfiguration<Newsletter>
    {
        public NewsletterMap()
        {
            ToTable("System_Newsletters");
            HasKey(x => x.Id);
            Property(x => x.Title).HasMaxLength(255).IsRequired();
            Property(x => x.BodyContent).IsRequired();
            Property(x => x.CultureCode).HasMaxLength(10);
        }
    }
}