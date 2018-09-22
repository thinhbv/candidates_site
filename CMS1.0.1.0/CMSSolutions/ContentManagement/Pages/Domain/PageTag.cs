using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Pages.Domain
{
    [DataContract]
    [TableName("System_PageTags")]
    public class PageTag : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DataMember]
        [DisplayName("Content")]
        public string Content { get; set; }
    }

    [Feature(Constants.Areas.Pages)]
    public class PageTagMap : EntityTypeConfiguration<PageTag>
    {
        public PageTagMap()
        {
            ToTable("System_PageTags");
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(255).IsRequired();
        }
    }
}
