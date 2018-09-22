using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Media.Domain
{
    [DataContract]
    [TableName("System_MediaPartTypes")]
    public class MediaPartType : BaseEntity<Guid>
    {
        [DataMember]
        [DisplayName("Type")]
        public string Type { get; set; }
    }

    [Feature(Constants.Areas.Media)]
    public class MediaPartTypeMap : EntityTypeConfiguration<MediaPartType>
    {
        public MediaPartTypeMap()
        {
            ToTable("System_MediaPartTypes");
            HasKey(x => x.Id);
            Property(x => x.Type).IsRequired().HasMaxLength(2048);
        }
    }
}