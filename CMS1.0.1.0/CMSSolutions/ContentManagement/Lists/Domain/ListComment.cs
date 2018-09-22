using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Lists.Domain
{
    [DataContract]
    [TableName("System_ListComments")]
    public class ListComment : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DataMember]
        [DisplayName("Email")]
        public string Email { get; set; }

        [DataMember]
        [DisplayName("Website")]
        public string Website { get; set; }

        [DataMember]
        [DisplayName("Comments")]
        public string Comments { get; set; }

        [DataMember]
        [DisplayName("ListId")]
        public int ListId { get; set; }

        [DataMember]
        [DisplayName("ListItemId")]
        public int ListItemId { get; set; }

        [DataMember]
        [DisplayName("CreatedDate")]
        public DateTime CreatedDate { get; set; }

        [DataMember]
        [DisplayName("IPAddress")]
        public string IPAddress { get; set; }

        [DataMember]
        [DisplayName("IsApproved")]
        public bool IsApproved { get; set; }
    }

    [Feature(Constants.Areas.Lists)]
    public class ListCommentMap : EntityTypeConfiguration<ListComment>
    {
        public ListCommentMap()
        {
            ToTable("System_ListComments");
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(255).IsRequired();
            Property(x => x.Email).HasMaxLength(255).IsRequired();
            Property(x => x.Website).HasMaxLength(255);
            Property(x => x.IPAddress).HasMaxLength(50).IsRequired();
        }
    }
}