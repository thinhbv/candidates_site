using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Web.Security.Domain
{
    [DataContract]
    [TableName("System_LocalAccounts")]
    public class LocalAccount : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("UserId")]
        public int UserId { get; set; }

        [DataMember]
        [DisplayName("ConfirmationToken")]
        public string ConfirmationToken { get; set; }

        [DataMember]
        [DisplayName("IsConfirmed")]
        public bool IsConfirmed { get; set; }

        [DataMember]
        [DisplayName("LastPasswordFailureDate")]
        public DateTime? LastPasswordFailureDate { get; set; }

        [DataMember]
        [DisplayName("PasswordFailuresSinceLastSuccess")]
        public int PasswordFailuresSinceLastSuccess { get; set; }

        [DataMember]
        [DisplayName("Password")]
        public string Password { get; set; }

        [DataMember]
        [DisplayName("PasswordChangedDate")]
        public DateTime? PasswordChangedDate { get; set; }

        [DataMember]
        [DisplayName("PasswordVerificationToken")]
        public string PasswordVerificationToken { get; set; }

        [DataMember]
        [DisplayName("PasswordVerificationTokenExpirationDate")]
        public DateTime? PasswordVerificationTokenExpirationDate { get; set; }
    }

    [Feature(Constants.Areas.Security)]
    public class LocalAccountMap : EntityTypeConfiguration<LocalAccount>
    {
        public LocalAccountMap()
        {
            ToTable("System_LocalAccounts");
            HasKey(x => x.Id);
            Property(x => x.UserId).IsRequired();
            Property(x => x.ConfirmationToken);
            Property(x => x.Password).IsRequired();
            Property(x => x.PasswordVerificationToken);
        }
    }
}