using System;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Messages.Models
{
    public class MessageTemplateModel : BaseModel<Guid>
    {
        [ControlText(Required = true, MaxLength = 255, ReadOnly = true)]
        public string Name { get; set; }

        [ControlDiv(ShowLabelControl = true)]
        public string Tokens { get; set; }

        [ControlText(Required = true, MaxLength = 255)]
        public string Subject { get; set; }

        [ControlText(Type = ControlText.RichText)]
        public string Body { get; set; }

        [ControlChoice(ControlChoice.CheckBox)]
        public bool Enabled { get; set; }

        public static implicit operator MessageTemplateModel(Domain.MessageTemplate entity)
        {
            return new MessageTemplateModel
            {
                Id = entity.Id,
                Name = entity.Name,
                Subject = entity.Subject,
                Body = entity.Body,
                Enabled = entity.Enabled
            };
        }
    }
}