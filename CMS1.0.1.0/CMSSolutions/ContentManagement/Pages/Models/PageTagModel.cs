using System;
using CMSSolutions.ContentManagement.Pages.Domain;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Pages.Models
{
    public class PageTagModel : BaseModel<int>
    {
        [ControlText(Required = true, MaxLength = 255)]
        public string Name { get; set; }

        [ControlText(Type = ControlText.MultiText)]
        public string Content { get; set; }

        public static implicit operator PageTagModel(PageTag pageTag)
        {
            return new PageTagModel
                   {
                       Id = pageTag.Id,
                       Name = pageTag.Name,
                       Content = pageTag.Content
                   };
        }
    }
}
