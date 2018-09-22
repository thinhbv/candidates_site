using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Localization;

namespace CMSSolutions.ContentManagement.Pages.Domain
{
    [DataContract]
    [TableName("System_Pages")]
    public class Page : BaseEntity<int>, ILocalizableEntity<int>
    {
        [DataMember]
        [DisplayName("Title")]
        public string Title { get; set; }

        [DataMember]
        [DisplayName("Slug")]
        public string Slug { get; set; }

        [DataMember]
        [DisplayName("MetaKeywords")]
        public string MetaKeywords { get; set; }

        [DataMember]
        [DisplayName("MetaDescription")]
        public string MetaDescription { get; set; }

        [DataMember]
        [DisplayName("IsEnabled")]
        public bool IsEnabled { get; set; }

        [DataMember]
        [DisplayName("BodyContent")]
        public string BodyContent { get; set; }

        [DataMember]
        [DisplayName("Theme")]
        public string Theme { get; set; }

        [DataMember]
        [DisplayName("CssClass")]
        public string CssClass { get; set; }

        [DataMember]
        [DisplayName("CultureCode")]
        public string CultureCode { get; set; }

        [DataMember]
        [DisplayName("RefId")]
        public int? RefId { get; set; }
    }

    [Feature(Constants.Areas.Pages)]
    public class PageMap : EntityTypeConfiguration<Page>
    {
        public PageMap()
        {
            ToTable("System_Pages");
            HasKey(x => x.Id);
            Property(x => x.Title).HasMaxLength(255).IsRequired();
            Property(x => x.Slug).HasMaxLength(255).IsRequired();
            Property(x => x.Theme).HasMaxLength(255);
            Property(x => x.CssClass).HasMaxLength(255);
            Property(x => x.MetaKeywords).HasMaxLength(255);
            Property(x => x.MetaDescription).HasMaxLength(255);
            Property(x => x.CultureCode).HasMaxLength(10);
        }
    }
}