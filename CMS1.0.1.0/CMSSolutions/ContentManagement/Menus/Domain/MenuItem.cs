using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Menus.Domain
{
    [DataContract]
    [TableName("System_MenuItems")]
    public class MenuItem : BaseEntity<int>, IRelationshipEntity<int>
    {
        [DataMember]
        [DisplayName("MenuId")]
        public int MenuId { get; set; }

        [DataMember]
        [DisplayName("Text")]
        public string Text { get; set; }

        [DataMember]
        [DisplayName("Description")]
        public string Description { get; set; }

        [DataMember]
        [DisplayName("Url")]
        public string Url { get; set; }

        [DataMember]
        [DisplayName("CssClass")]
        public string CssClass { get; set; }

        [DataMember]
        [DisplayName("Position")]
        public int Position { get; set; }

        [DataMember]
        [DisplayName("ParentId")]
        public int? ParentId { get; set; }

        [DataMember]
        [DisplayName("Enabled")]
        public bool Enabled { get; set; }

        [DataMember]
        [DisplayName("IsExternalUrl")]
        public bool IsExternalUrl { get; set; }

        [DataMember]
        [DisplayName("RefId")]
        public int? RefId { get; set; }
    }

    [Feature(Constants.Areas.Menus)]
    public class MenuItemMap : EntityTypeConfiguration<MenuItem>
    {
        public MenuItemMap()
        {
            ToTable("System_MenuItems");
            HasKey(x => x.Id);
            Property(x => x.MenuId);
            Property(x => x.Position);
            Property(x => x.Text).HasMaxLength(255).IsRequired();
            Property(x => x.Description).HasMaxLength(255);
            Property(x => x.Url).HasMaxLength(255).IsRequired();
            Property(x => x.CssClass).HasMaxLength(255);
            Property(x => x.ParentId);
            Property(x => x.Enabled);
        }
    }
}