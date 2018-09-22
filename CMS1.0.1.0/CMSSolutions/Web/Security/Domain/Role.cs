using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Web.Security.Domain
{
    [DataContract]
    [TableName("System_Roles")]
    public class Role : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }
    }

    [Feature(Constants.Areas.Security)]
    public class RoleMap : EntityTypeConfiguration<Role>
    {
        public RoleMap()
        {
            ToTable("System_Roles");
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(255).IsRequired();
        }
    }
}