using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Configuration.Domain
{
    [DataContract]
    [TableName("System_Settings")]
    public class Setting : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DataMember]
        [DisplayName("Value")]
        public string Value { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }

    [Feature(Constants.Areas.Core)]
    public class SettingMap : EntityTypeConfiguration<Setting>
    {
        public SettingMap()
        {
            ToTable("System_Settings");
            HasKey(s => s.Id);
            Property(s => s.Name).IsRequired().HasMaxLength(255);
            Property(s => s.Value).IsMaxLength();
        }
    }
}