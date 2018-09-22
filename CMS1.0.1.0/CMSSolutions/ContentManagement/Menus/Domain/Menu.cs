using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Menus.Domain
{
    [DataContract]
    [TableName("System_Menus")]
    public class Menu : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DataMember]
        [DisplayName("IsMainMenu")]
        public bool IsMainMenu { get; set; }
    }

    [Feature(Constants.Areas.Menus)]
    public class MenuMap : EntityTypeConfiguration<Menu>
    {
        public MenuMap()
        {
            ToTable("System_Menus");
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(255).IsRequired();
        }
    }
}