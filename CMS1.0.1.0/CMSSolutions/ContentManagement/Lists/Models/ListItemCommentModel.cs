using System;
using System.ComponentModel.DataAnnotations;
using CMSSolutions.ContentManagement.Lists.Domain;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists.Models
{
    public class ListItemCommentModel
    {
        [ControlHidden]
        public int Id { get; set; }

        [Required]
        [ControlText(Required = true, MaxLength = 255)]
        public string Name { get; set; }

        [Required, RegularExpression("^[a-z0-9_\\+-]+(\\.[a-z0-9_\\+-]+)*@[a-z0-9]+(\\.[a-z0-9]+)*\\.([a-z]{2,4})$", ErrorMessage = "Please enter a valid email address.")]
        [ControlText(Type = ControlText.Email, Required = true, MaxLength = 255)]
        public string Email { get; set; }

        [ControlText(Type = ControlText.Url, MaxLength = 255)]
        public string Website { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "Approved")]
        public bool IsApproved { get; set; }

        [Required]
        [ControlText(Type = ControlText.MultiText, Required = true)]
        public string Comments { get; set; }

        public int ListItemId { get; set; }

        public DateTime CreatedDate { get; set; }

        public string IPAddress { get; set; }

        public static implicit operator ListItemCommentModel(ListComment comment)
        {
            return new ListItemCommentModel
                       {
                           Id = comment.Id,
                           Name = comment.Name,
                           Email = comment.Email,
                           Website = comment.Website,
                           IsApproved = comment.IsApproved,
                           Comments = comment.Comments,
                       };
        }
    }
}