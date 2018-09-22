using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Aliases.Domain
{
    [DataContract]
    public class Alias : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("Path")]
        public string Path { get; set; }

        [DataMember]
        [DisplayName("RouteValues")]
        public string RouteValues { get; set; }

        [DataMember]
        [DisplayName("Source")]
        public string Source { get; set; }

        [DataMember]
        [DisplayName("IsEnabled")]
        public bool IsEnabled { get; set; }

        [DataMember]
        [DisplayName("ActionId")]
        public int ActionId { get; set; }

        [ForeignKey("ActionId")]
        [IgnoreDataMember]
        public virtual Action Action { get; set; }
    }

    [Feature(Constants.Areas.Aliases)]
    public class AliasMap : EntityTypeConfiguration<Alias>
    {
        public AliasMap()
        {
            ToTable("System_Alias");
            HasKey(x => x.Id);
            Property(x => x.Path).HasMaxLength(2048);
            Property(x => x.Source).HasMaxLength(255).IsRequired();
        }
    }
}