using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Widgets.Domain
{
    [DataContract]
    [TableName("System_Zones")]
    public class Zone : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }
    }

    [Feature(Constants.Areas.Widgets)]
    public class ZoneMap : EntityTypeConfiguration<Zone>
    {
        public ZoneMap()
        {
            ToTable("System_Zones");
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(255).IsRequired();
        }
    }
}