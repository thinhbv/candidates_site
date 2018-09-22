using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Lists.Domain
{
    [DataContract]
    [TableName("System_ListItems")]
    public class ListItem : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("Title")]
        public string Title { get; set; }

        [DataMember]
        [DisplayName("Slug")]
        public string Slug { get; set; }

        [DataMember]
        [DisplayName("PictureUrl")]
        public string PictureUrl { get; set; }

        [DataMember]
        [DisplayName("MetaKeywords")]
        public string MetaKeywords { get; set; }

        [DataMember]
        [DisplayName("MetaDescription")]
        public string MetaDescription { get; set; }

        [DataMember]
        [DisplayName("ListId")]
        public int ListId { get; set; }

        [DataMember]
        [DisplayName("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [DisplayName("ModifiedDate")]
        public DateTime ModifiedDate { get; set; }

        [DataMember]
        [DisplayName("Position")]
        public int Position { get; set; }

        [DataMember]
        [DisplayName("Enabled")]
        public bool Enabled { get; set; }

        [DataMember]
        [DisplayName("Values")]
        public string Values { get; set; }

        [ForeignKey("ListId")]
        [IgnoreDataMember]
        public virtual List List { get; set; }

        [IgnoreDataMember]
        public virtual ICollection<ListComment> Comments { get; set; }
    }

    [Feature(Constants.Areas.Lists)]
    public class ListItemMap : EntityTypeConfiguration<ListItem>
    {
        public ListItemMap()
        {
            ToTable("System_ListItems");
            HasKey(x => x.Id);
            Property(x => x.Title).IsRequired().HasMaxLength(255);
            Property(x => x.Slug).IsRequired().HasMaxLength(255);
            Property(x => x.PictureUrl).HasMaxLength(255);
            Property(x => x.MetaKeywords).HasMaxLength(255);
            Property(x => x.MetaDescription).HasMaxLength(255);
            HasRequired(x => x.List);
        }
    }
}