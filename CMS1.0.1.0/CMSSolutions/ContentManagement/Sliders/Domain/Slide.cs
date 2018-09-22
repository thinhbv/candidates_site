using System;
using System.ComponentModel;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;

namespace CMSSolutions.ContentManagement.Sliders.Domain
{
    [DataContract]
    [TableName("System_Slides")]
    public class Slide : BaseEntity<Guid>
    {
        [DataMember]
        [DisplayName("SliderId")]
        public Guid SliderId { get; set; }

        [DataMember]
        [DisplayName("Position")]
        public int Position { get; set; }

        [DataMember]
        [DisplayName("Title")]
        public string Title { get; set; }

        [DataMember]
        [DisplayName("BackgroundUrl")]
        public string BackgroundUrl { get; set; }

        [DataMember]
        [DisplayName("SlideDirection")]
        public string SlideDirection { get; set; }

        [DataMember]
        [DisplayName("SlideDelay")]
        public int? SlideDelay { get; set; }

        [DataMember]
        [DisplayName("Transition2D")]
        public string Transition2D { get; set; }

        [DataMember]
        [DisplayName("Transition3D")]
        public string Transition3D { get; set; }

        [DataMember]
        [DisplayName("Sublayers")]
        public string Sublayers { get; set; }

        [DataMember]
        [DisplayName("OnClick")]
        public string OnClick { get; set; }
    }

    [Feature(Constants.Areas.Sliders)]
    public class SlideMap : EntityTypeConfiguration<Slide>
    {
        public SlideMap()
        {
            ToTable("System_Slides");
            HasKey(x => x.Id);
            Property(x => x.Title).HasMaxLength(255);
            Property(x => x.BackgroundUrl).HasMaxLength(255);
            Property(x => x.SlideDirection).HasMaxLength(10).IsRequired();
            Property(x => x.Transition2D).HasMaxLength(255);
            Property(x => x.Transition3D).HasMaxLength(255);
            Property(x => x.Sublayers).IsMaxLength();
            Property(x => x.OnClick).HasMaxLength(255);
        }
    }
}