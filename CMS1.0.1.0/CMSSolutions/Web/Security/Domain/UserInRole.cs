using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.Web.Security.Domain
{
    [DataContract]
    [TableName("System_UsersInRoles")]
    public class UserInRole : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("UserId")]
        public int UserId { get; set; }

        [DataMember]
        [DisplayName("RoleId")]
        public int RoleId { get; set; }
    }

    [Feature(Constants.Areas.Security)]
    public class UserInRoleMap : EntityTypeConfiguration<UserInRole>
    {
        public UserInRoleMap()
        {
            ToTable("System_UsersInRoles");
            HasKey(x => x.Id);
        }
    }
}