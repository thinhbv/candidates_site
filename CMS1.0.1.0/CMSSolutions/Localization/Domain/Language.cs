using System;
using System.Runtime.Serialization;
using CMSSolutions.Data;
using CMSSolutions.Data.Entity;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Events;
using CMSSolutions.Web;

namespace CMSSolutions.Localization.Domain
{
    [DataContract]
    [TableName("System_Languages")]
    public class Language : BaseEntity<int>
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string CultureCode { get; set; }
        
        [DataMember]
        public string ImageFlag { get; set; }

        [DataMember]
        public bool Active { get; set; }

        [DataMember]
        public int SortOrder { get; set; }

        [DataMember]
        public string Theme { get; set; }

        [DataMember]
        public bool IsBlocked { get; set; }
    }

    [Feature(Constants.Areas.Localization)]
    public class LanguageMap : EntityTypeConfiguration<Language>
    {
        public LanguageMap()
        {
            ToTable("System_Languages");
            HasKey(m => m.Id);
            Property(m => m.Name).HasMaxLength(255).IsRequired();
            Property(m => m.CultureCode).HasMaxLength(10).IsRequired();
            Property(m => m.Theme).HasMaxLength(255);
            Property(m => m.ImageFlag).HasMaxLength(500);
        }
    }

    [Feature(Constants.Areas.Localization)]
    public class LanguageContentHandler : IContentHandler<Language>
    {
        private readonly SiteSettings siteSettings;

        public LanguageContentHandler(SiteSettings siteSettings)
        {
            this.siteSettings = siteSettings;
        }

        #region Implementation of IContentHandler<Language>

        public void Creating(CreateContentContext<Language> context)
        {
        }

        public void Created(CreateContentContext<Language> context)
        {
        }

        public void Updating(UpdateContentContext<Language> context)
        {
        }

        public void Updated(UpdateContentContext<Language> context)
        {
        }

        public void Removing(RemoveContentContext<Language> context)
        {
            if (context.ContentItem.CultureCode == siteSettings.DefaultLanguage)
            {
                throw new NotSupportedException("Cannot delete default language.");
            }
        }

        public void Removed(RemoveContentContext<Language> context)
        {
        }

        #endregion Implementation of IContentHandler<Language>
    }
}