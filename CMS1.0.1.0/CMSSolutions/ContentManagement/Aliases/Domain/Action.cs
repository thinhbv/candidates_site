using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Aliases.Domain
{
    [DataContract]
    public class Action : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("Area")]
        public string Area { get; set; }

        [DataMember]
        [DisplayName("Controller")]
        public string Controller { get; set; }

        [DataMember]
        [DisplayName("ActionName")]
        public string ActionName { get; set; }
    }

    [Feature(Constants.Areas.Aliases)]
    public class ActionMap : EntityTypeConfiguration<Action>
    {
        public ActionMap()
        {
            ToTable("System_AliasActions");
            HasKey(x => x.Id);
            Property(x => x.Area).HasMaxLength(255).IsRequired();
            Property(x => x.Controller).HasMaxLength(255).IsRequired();
            Property(x => x.ActionName).HasMaxLength(255).IsRequired();
        }
    }
}