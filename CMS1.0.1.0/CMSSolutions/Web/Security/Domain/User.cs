using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Web.Security.Domain
{
    [DataContract]
    [TableName("System_Users")]
    public class User : BaseEntity<int>, IUserInfo
    {
        [DataMember]
        [DisplayName("UserName")]
        public string UserName { get; set; }

        [DataMember]
        [DisplayName("FullName")]
        public string FullName { get; set; }

        [DataMember]
        [DisplayName("PhoneNumber")]
        public string PhoneNumber { get; set; }

        [DataMember]
        [DisplayName("Email")]
        public string Email { get; set; }

        [DataMember]
        [DisplayName("IsLockedOut")]
        public bool IsLockedOut { get; set; }

        [DataMember]
        [DisplayName("SuperUser")]
        public bool SuperUser { get; set; }

        [DataMember]
        [DisplayName("CreateDate")]
        public DateTime CreateDate { get; set; }

        [DataMember]
        [DisplayName("UserSite")]
        public string UserSite { get; set; }

        public override string ToString()
        {
            return UserName;
        }
    }

    [Feature(Constants.Areas.Security)]
    public class UserMap : EntityTypeConfiguration<User>
    {
        public UserMap()
        {
            ToTable("System_Users");
            HasKey(x => x.Id);
            Property(x => x.FullName).HasMaxLength(255);
            Property(x => x.PhoneNumber).HasMaxLength(50);
            Property(x => x.UserName).HasMaxLength(255).IsRequired();
            Property(x => x.Email).HasMaxLength(255).IsRequired();
            Property(x => x.UserSite).HasMaxLength(255);
        }
    }
}