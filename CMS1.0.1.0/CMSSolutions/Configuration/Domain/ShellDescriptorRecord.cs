using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Configuration.Domain
{
    [DataContract]
    [TableName("System_ShellDescriptors")]
    public class ShellDescriptorRecord : BaseEntity<Guid>
    {
        [DataMember]
        [DisplayName("SerialNumber")]
        public int SerialNumber { get; set; }

        [DataMember]
        [DisplayName("Features")]
        public string Features { get; set; }
    }

    [Feature(Constants.Areas.Core)]
    public class ShellDescriptorMap : EntityTypeConfiguration<ShellDescriptorRecord>
    {
        public ShellDescriptorMap()
        {
            ToTable("System_ShellDescriptors");
            HasKey(x => x.Id);
        }
    }
}