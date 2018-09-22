using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Messages.Domain
{
    [DataContract]
    [TableName("System_MessageTemplates")]
    public class MessageTemplate : BaseEntity<Guid>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DataMember]
        [DisplayName("OwnerId")]
        public Guid? OwnerId { get; set; }

        [DataMember]
        [DisplayName("Subject")]
        public string Subject { get; set; }

        [DataMember]
        [DisplayName("Body")]
        public string Body { get; set; }

        [DataMember]
        [DisplayName("Enabled")]
        public bool Enabled { get; set; }
    }

    [Feature(Constants.Areas.Messages)]
    public class MessageTemplateMap : EntityTypeConfiguration<MessageTemplate>
    {
        public MessageTemplateMap()
        {
            ToTable("System_MessageTemplates");
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(255).IsRequired();
            Property(x => x.Subject).HasMaxLength(255).IsRequired();
        }
    }
}