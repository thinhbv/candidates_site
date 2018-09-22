using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.ContentManagement.Media.Models;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Media.Domain
{
    [DataContract]
    [TableName("System_MediaParts")]
    public class MediaPart : BaseEntity<Guid>, IMediaPart
    {
        [DataMember]
        [ControlFileUpload(EnableFineUploader = true, ColumnWidth = 300, Required = true)]
        [DisplayName("Url")]
        public string Url { get; set; }

        [DataMember]
        [ControlText(MaxLength = 255)]
        [DisplayName("Caption")]
        public string Caption { get; set; }

        [DataMember]
        [ControlNumeric(Required = true, LabelText = "Sort Order")]
        [DisplayName("SortOrder")]
        public int SortOrder { get; set; }

        [DataMember]
        [DisplayName("MediaPartTypeId")]
        public Guid MediaPartTypeId { get; set; }

        [DataMember]
        [DisplayName("ParentId")]
        public int ParentId { get; set; }
    }

    [Feature(Constants.Areas.Media)]
    public class MediaPartMap : EntityTypeConfiguration<MediaPart>
    {
        public MediaPartMap()
        {
            ToTable("System_MediaParts");
            HasKey(x => x.Id);
            Property(x => x.Caption).HasMaxLength(255);
            Property(x => x.Url).IsRequired().HasMaxLength(2048);
        }
    }
}