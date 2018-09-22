using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Lists.Domain
{
    [DataContract]
    [TableName("System_Lists")]
    public class List : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DataMember]
        [DisplayName("Url")]
        public string Url { get; set; }

        [DataMember]
        [DisplayName("Enabled")]
        public bool Enabled { get; set; }

        [DataMember]
        [DisplayName("SummaryTemplate")]
        public string SummaryTemplate { get; set; }

        [DataMember]
        [DisplayName("DetailTemplate")]
        public string DetailTemplate { get; set; }

        [DataMember]
        [DisplayName("Sorting")]
        public string Sorting { get; set; }

        [DataMember]
        [DisplayName("PageSize")]
        public int PageSize { get; set; }

        [DataMember]
        [DisplayName("EnabledMetaTags")]
        public bool EnabledMetaTags { get; set; }

        [DataMember]
        [DisplayName("EnabledComments")]
        public bool EnabledComments { get; set; }

        [DataMember]
        [DisplayName("AutoModeratedComments")]
        public bool AutoModeratedComments { get; set; }

        [DataMember]
        [DisplayName("CssClass")]
        public string CssClass { get; set; }

        #region Methods

        public object GetFieldValue(IDictionary<string, object> values, string fieldName)
        {
            return values.ContainsKey(fieldName) ? values[fieldName] : string.Empty;
        }

        #endregion Methods
    }

    [Feature(Constants.Areas.Lists)]
    public class ListMap : EntityTypeConfiguration<List>
    {
        public ListMap()
        {
            ToTable("System_Lists");
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(255).IsRequired();
            Property(x => x.Url).HasMaxLength(255).IsRequired();
            Property(x => x.PageSize);
            Property(x => x.Sorting).HasMaxLength(255);
            Property(x => x.CssClass).HasMaxLength(255);
        }
    }
}