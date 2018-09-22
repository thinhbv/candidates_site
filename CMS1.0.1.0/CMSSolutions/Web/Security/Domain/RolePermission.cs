using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Web.Security.Domain
{
    [DataContract]
    [TableName("System_RolesPermissions")]
    public class RolePermission : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("RoleId")]
        public int RoleId { get; set; }

        [DataMember]
        [DisplayName("PermissionId")]
        public int PermissionId { get; set; }
    }

    [Feature(Constants.Areas.Security)]
    public class RolePermissionMap : EntityTypeConfiguration<RolePermission>
    {
        public RolePermissionMap()
        {
            ToTable("System_RolesPermissions");
            HasKey(x => x.Id);
        }
    }
}