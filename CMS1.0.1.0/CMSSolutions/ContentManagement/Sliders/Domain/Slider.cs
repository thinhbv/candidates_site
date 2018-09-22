using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Sliders.Domain
{
    [DataContract]
    [TableName("System_Sliders")]
    public class Slider : BaseEntity<Guid>
    {
        [DataMember]
        [DisplayName("Name")]
        public string Name { get; set; }

        [DataMember]
        [DisplayName("Width")]
        public int Width { get; set; }

        [DataMember]
        [DisplayName("Height")]
        public int Height { get; set; }
    }

    [Feature(Constants.Areas.Sliders)]
    public class SliderMap : EntityTypeConfiguration<Slider>
    {
        public SliderMap()
        {
            ToTable("System_Sliders");
            HasKey(x => x.Id);
            Property(x => x.Name).HasMaxLength(255).IsRequired();
        }
    }
}