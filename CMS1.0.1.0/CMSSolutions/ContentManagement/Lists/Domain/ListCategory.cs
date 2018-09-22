using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Lists.Domain
{
    [DataContract]
    [TableName("System_ListCategories")]
    public class ListCategory : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DataMember]
        [DisplayName("Url")]
        public string Url { get; set; }

        [DisplayName("Position")]
        public int Position { get; set; }

        [DataMember]
        [DisplayName("FullUrl")]
        public string FullUrl { get; set; }

        [DataMember]
        [DisplayName("ListId")]
        public int ListId { get; set; }

        [DataMember]
        [DisplayName("ParentId")]
        public int? ParentId { get; set; }
    }

    [Feature(Constants.Areas.Lists)]
    public class CategoryMap : EntityTypeConfiguration<ListCategory>
    {
        public CategoryMap()
        {
            ToTable("System_ListCategories");
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(255).IsRequired();
            Property(x => x.Url).HasMaxLength(255).IsRequired();
            Property(x => x.FullUrl).HasMaxLength(255).IsRequired();
        }
    }
}