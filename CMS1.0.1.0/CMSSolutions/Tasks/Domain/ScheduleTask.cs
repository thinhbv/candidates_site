using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Tasks.Domain
{
    [DataContract]
    [TableName("System_ScheduleTasks")]
    public class ScheduleTask : BaseEntity<Guid>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DataMember]
        [DisplayName("CronExpression")]
        public string CronExpression { get; set; }

        [DataMember]
        [DisplayName("Type")]
        public string Type { get; set; }

        [DataMember]
        [DisplayName("DisallowConcurrentExecution")]
        public bool DisallowConcurrentExecution { get; set; }

        [DataMember]
        [DisplayName("Enabled")]
        public bool Enabled { get; set; }

        [DataMember]
        [DisplayName("LastStartUtc")]
        public DateTime? LastStartUtc { get; set; }

        [DataMember]
        [DisplayName("LastEndUtc")]
        public DateTime? LastEndUtc { get; set; }

        [DataMember]
        [DisplayName("LastSuccessUtc")]
        public DateTime? LastSuccessUtc { get; set; }
    }

    [Feature(Constants.Areas.ScheduledTasks)]
    public class ScheduleTaskMap : EntityTypeConfiguration<ScheduleTask>
    {
        public ScheduleTaskMap()
        {
            ToTable("System_ScheduleTasks");
            HasKey(x => x.Id);
            Property(x => x.Name).IsRequired().HasMaxLength(255);
            Property(x => x.Type).IsRequired().HasMaxLength(255);
            Property(x => x.CronExpression).IsRequired().HasMaxLength(255);
        }
    }
}