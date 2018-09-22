using System;
using System.ComponentModel.DataAnnotations;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Security.Domain;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web.Security.Models
{
    public class UserModel : BaseModel<int>
    {
        [Required]
        [ControlText(Required = true, LabelText = "User Name", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 0)]
        public string UserName { get; set; }

        [ControlText(LabelText = "Full Name", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 0)]
        public string FullName { get; set; }

        [ControlText(LabelText = "Phone Number", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 0)]
        public string PhoneNumber { get; set; }

        [Required]
        [ControlText(Type = ControlText.Email, Required = true, ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 0)]
        public string Email { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Locked Out", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 2)]
        public bool IsLockedOut { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Super User", ContainerCssClass = "col-xs-6 col-md-6", ContainerRowIndex = 2)]
        public bool SuperUser { get; set; }

        public static implicit operator UserModel(User user)
        {
            return new UserModel
            {
                Id = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Email = user.Email,
                IsLockedOut = user.IsLockedOut,
                SuperUser = user.SuperUser
            };
        }
    }
}