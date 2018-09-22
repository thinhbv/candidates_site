using System;
using CMSSolutions.ContentManagement.Newsletters.Domain;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Newsletters.Models
{
    public class NewsletterModel
    {
        [ControlHidden]
        public Guid Id { get; set; }

        [ControlText(Required = true, MaxLength = 255, ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 0)]
        public string Title { get; set; }

        [ControlText(Type = ControlText.RichText, LabelText = "Body Content", HasLabelControl = false, ContainerCssClass = "col-xs-12 col-md-12", ContainerRowIndex = 1)]
        public string BodyContent { get; set; }

        [ControlHidden]
        public string CultureCode { get; set; }

        [ControlHidden]
        public Guid? RefId { get; set; }

        public static implicit operator NewsletterModel(Newsletter other)
        {
            if (other == null)
            {
                return null;
            }

            return new NewsletterModel
            {
                Id = other.Id,
                Title = other.Title,
                BodyContent = other.BodyContent,
                CultureCode = other.CultureCode,
                RefId = other.RefId
            };
        }
    }
}