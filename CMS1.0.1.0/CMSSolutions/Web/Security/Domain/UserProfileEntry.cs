using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Web.Security.Domain
{
    [DataContract]
    public class UserProfileEntry : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("UserId")]
        public int UserId { get; set; }

        [DataMember]
        [DisplayName("Key")]
        public string Key { get; set; }

        [DataMember]
        [DisplayName("Value")]
        public string Value { get; set; }
    }

    [Feature(Constants.Areas.Security)]
    public class UserProfileEntryMap : EntityTypeConfiguration<UserProfileEntry>
    {
        public UserProfileEntryMap()
        {
            ToTable("System_UserProfiles");
            HasKey(x => x.Id);
            Property(x => x.UserId).IsRequired();
            Property(x => x.Key).HasMaxLength(255).IsRequired();
        }
    }
}