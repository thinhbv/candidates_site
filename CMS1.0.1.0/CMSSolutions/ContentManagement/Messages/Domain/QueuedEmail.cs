using System;
using System.ComponentModel;
using System.Net.Mail;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Messages.Domain
{
    [DataContract]
    [TableName("System_QueuedEmails")]
    public class QueuedEmail : BaseEntity<Guid>
    {
        [DataMember]
        [DisplayName("Priority")]
        public int Priority { get; set; }

        [DataMember]
        [DisplayName("FromAddress")]
        public string FromAddress { get; set; }

        [DataMember]
        [DisplayName("FromName")]
        public string FromName { get; set; }

        [DataMember]
        [DisplayName("ToAddress")]
        public string ToAddress { get; set; }

        [DataMember]
        [DisplayName("ToName")]
        public string ToName { get; set; }

        [DataMember]
        [DisplayName("Subject")]
        public string Subject { get; set; }

        [DataMember]
        [DisplayName("MailMessage")]
        public string MailMessage { get; set; }

        [DataMember]
        [DisplayName("CreatedOnUtc")]
        public DateTime CreatedOnUtc { get; set; }

        [DataMember]
        [DisplayName("SentTries")]
        public int SentTries { get; set; }

        [DataMember]
        [DisplayName("SentOnUtc")]
        public DateTime? SentOnUtc { get; set; }

        public MailMessage GetMailMessage()
        {
            var wrap = MailMessageWrapper.Create(MailMessage);
            return wrap.ToMailMessage();
        }
    }

    [Feature(Constants.Areas.Messages)]
    public class QueuedEmailMap : EntityTypeConfiguration<QueuedEmail>
    {
        public QueuedEmailMap()
        {
            ToTable("System_QueuedEmails");
            HasKey(x => x.Id);
            Property(x => x.FromAddress).HasMaxLength(255);
            Property(x => x.FromName).HasMaxLength(255);
            Property(x => x.ToAddress).IsRequired().HasMaxLength(255);
            Property(x => x.ToName).HasMaxLength(255);
            Property(x => x.Subject).HasMaxLength(255);
            Property(x => x.CreatedOnUtc).IsRequired();
            Property(x => x.SentTries).IsRequired();
            Property(x => x.SentOnUtc);
        }
    }
}