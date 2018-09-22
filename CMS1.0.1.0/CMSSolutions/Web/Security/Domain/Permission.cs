using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;

namespace CMSSolutions.Web.Security.Domain
{
    [DataContract]
    [TableName("System_Permissions")]
    public class Permission : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DataMember]
        [DisplayName("Category")]
        public string Category { get; set; }

        [DataMember]
        [DisplayName("Description")]
        public string Description { get; set; }
    }

    public class PermissionMap : EntityTypeConfiguration<Permission>
    {
        public PermissionMap()
        {
            ToTable("System_Permissions");
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(255).IsRequired();
            Property(x => x.Category).HasMaxLength(255);
            Property(x => x.Description).HasMaxLength(255);
        }
    }
}