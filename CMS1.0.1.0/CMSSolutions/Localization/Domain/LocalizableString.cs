using System;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Localization.Domain
{
    [DataContract]
    [TableName("System_LocalizableStrings")]
    public class LocalizableString : BaseEntity<int>
    {
        [DataMember]
        public string CultureCode { get; set; }

        [DataMember]
        public string TextKey { get; set; }

        [DataMember]
        public string TextValue { get; set; }
    }

    [Feature(Constants.Areas.Localization)]
    public class LocalizableStringMap : EntityTypeConfiguration<LocalizableString>
    {
        public LocalizableStringMap()
        {
            ToTable("System_LocalizableStrings");
            HasKey(m => m.Id);
            Property(m => m.CultureCode).HasMaxLength(10);
            Property(m => m.TextKey).IsRequired();
        }
    }
}