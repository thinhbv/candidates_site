using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Lists.Domain
{
    [DataContract]
    [TableName("System_ListFields")]
    public class ListField : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("Title")]
        public string Title { get; set; }

        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DataMember]
        [DisplayName("FieldType")]
        public string FieldType { get; set; }

        [DataMember]
        [DisplayName("ListId")]
        public int ListId { get; set; }

        [DataMember]
        [DisplayName("FieldProperties")]
        public string FieldProperties { get; set; }

        [DataMember]
        [DisplayName("Position")]
        public int Position { get; set; }

        [DataMember]
        [DisplayName("Required")]
        public bool Required { get; set; }

        [IgnoreDataMember]
        public virtual List List { get; set; }
    }

    [Feature(Constants.Areas.Lists)]
    public class ListFieldMap : EntityTypeConfiguration<ListField>
    {
        public ListFieldMap()
        {
            ToTable("System_ListFields");
            HasKey(x => x.Id);
            Property(x => x.Title).HasMaxLength(255).IsRequired();
            Property(x => x.Name).HasMaxLength(255).IsRequired();
            Property(x => x.FieldType).HasMaxLength(255).IsRequired();
        }
    }
}