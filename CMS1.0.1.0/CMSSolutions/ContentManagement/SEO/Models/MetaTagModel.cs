using System;
using CMSSolutions.ContentManagement.SEO.Domain;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.SEO.Models
{
    public class MetaTagModel : BaseModel<Guid>
    {
        [ControlText(Required = true, MaxLength = 255)]
        public string Name { get; set; }

        [ControlText(MaxLength = 255)]
        public string Content { get; set; }

        [ControlText(MaxLength = 10)]
        public string Charset { get; set; }

        public static implicit operator MetaTagModel(MetaTag metaTag)
        {
            return new MetaTagModel
            {
                Id = metaTag.Id,
                Name = metaTag.Name,
                Content = metaTag.Content,
                Charset = metaTag.Charset,
            };
        }
    }
}
