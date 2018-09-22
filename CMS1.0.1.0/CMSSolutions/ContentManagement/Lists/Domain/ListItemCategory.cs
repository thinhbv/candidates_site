using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Lists.Domain
{
    [DataContract]
    [TableName("System_ListItemsInCategories")]
    public class ListItemCategory : BaseEntity<int>
    {
        [DataMember]
        [DisplayName("ItemId")]
        public int ItemId { get; set; }

        [DataMember]
        [DisplayName("CategoryId")]
        public int CategoryId { get; set; }
    }

    [Feature(Constants.Areas.Lists)]
    public class ListItemCategoryMap : EntityTypeConfiguration<ListItemCategory>
    {
        public ListItemCategoryMap()
        {
            ToTable("System_ListItemsInCategories");
            HasKey(x => x.Id);
        }
    }
}