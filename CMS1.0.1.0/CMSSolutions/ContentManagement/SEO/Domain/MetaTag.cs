using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.SEO.Domain
{
    [DataContract]
    [TableName("System_MetaTags")]
    public class MetaTag : BaseEntity<Guid>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DataMember]
        [DisplayName("Content")]
        public string Content { get; set; }

        [DataMember]
        [DisplayName("Charset")]
        public string Charset { get; set; }
    }

    [Feature(Constants.Areas.SEO)]
    public class MetaTagMap : EntityTypeConfiguration<MetaTag>
    {
        public MetaTagMap()
        {
            ToTable("System_MetaTags");
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(255).IsRequired();
            Property(x => x.Content).HasMaxLength(255);
            Property(x => x.Charset).HasMaxLength(10);
        }
    }
}
