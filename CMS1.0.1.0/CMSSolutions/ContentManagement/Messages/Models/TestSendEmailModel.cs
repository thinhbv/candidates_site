using System;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Messages.Models
{
    public class TestSendEmailModel
    {
        [ControlHidden]
        public Guid Id { get; set; }

        [ControlText(Type = ControlText.Email, Required = true, MaxLength = 255)]
        public string Email { get; set; }

        [ControlText(Required = true, MaxLength = 255)]
        public string Subject { get; set; }

        [ControlText(Type = ControlText.MultiText)]
        public string Content { get; set; }
    }
}