using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;

namespace CMSSolutions.ContentManagement.Widgets.Domain
{
    [DataContract]
    [TableName("System_Widgets")]
    public class Widget : BaseEntity<int>, ILocalizableEntity<int>
    {
        [DataMember]
        [DisplayName("Title")]
        public string Title { get; set; }

        [DataMember]
        [DisplayName("Order")]
        public int Order { get; set; }

        [DataMember]
        [DisplayName("Enabled")]
        public bool Enabled { get; set; }

        [DataMember]
        [DisplayName("WidgetName")]
        public string WidgetName { get; set; }

        [DataMember]
        [DisplayName("WidgetType")]
        public string WidgetType { get; set; }

        [DataMember]
        [DisplayName("ZoneId")]
        public int ZoneId { get; set; }

        [DataMember]
        [DisplayName("DisplayCondition")]
        public string DisplayCondition { get; set; }

        [DataMember]
        [DisplayName("WidgetValues")]
        public string WidgetValues { get; set; }

        [DataMember]
        [DisplayName("PageId")]
        public int? PageId { get; set; }

        [DataMember]
        [DisplayName("CultureCode")]
        public string CultureCode { get; set; }

        [DataMember]
        [DisplayName("RefId")]
        public int? RefId { get; set; }
    }

    [Feature(Constants.Areas.Widgets)]
    public class WidgetMap : EntityTypeConfiguration<Widget>
    {
        public WidgetMap()
        {
            ToTable("System_Widgets");
            HasKey(x => x.Id);
            Property(x => x.Title).HasMaxLength(255).IsRequired();
            Property(x => x.WidgetName).HasMaxLength(255).IsRequired();
            Property(x => x.WidgetType).HasMaxLength(1024).IsRequired();
            Property(x => x.CultureCode).HasMaxLength(10);
            Property(x => x.Enabled);
        }
    }
}