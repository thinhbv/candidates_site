using System;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Messages.Domain
{
    [DataContract]
    [TableName("System_QueuedSMS")]
    public class QueuedSms : BaseEntity<Guid>
    {
        /// <summary>
        /// Gets or sets the priority
        /// </summary>
        [DataMember]
        public int Priority { get; set; }

        /// <summary>
        /// Gets or sets the From Number property
        /// </summary>
        [DataMember]
        public string FromNumber { get; set; }

        /// <summary>
        /// Gets or sets the To Number property
        /// </summary>
        [DataMember]
        public string ToNumber { get; set; }

        /// <summary>
        /// Gets or sets the Message property
        /// </summary>
        [DataMember]
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the date and time of item creation in UTC
        /// </summary>
        [DataMember]
        public DateTime CreatedOnUtc { get; set; }

        /// <summary>
        /// Gets or sets the send tries
        /// </summary>
        [DataMember]
        public int SentTries { get; set; }

        /// <summary>
        /// Gets or sets the sent date and time
        /// </summary>
        [DataMember]
        public DateTime? SentOnUtc { get; set; }
    }

    [Feature(Constants.Areas.Messages)]
    public class QueuedSmsMap : EntityTypeConfiguration<QueuedSms>
    {
        public QueuedSmsMap()
        {
            ToTable("System_QueuedSMS");
            HasKey(x => x.Id);
            Property(x => x.FromNumber).HasMaxLength(30);
            Property(x => x.ToNumber).HasMaxLength(30).IsRequired();
        }
    }
}