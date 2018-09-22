using System.ComponentModel.DataAnnotations;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web.Security.Models
{
    public class ChangePasswordModel
    {
        [ControlText(ReadOnly = true, ContainerCssClass = Constants.ContainerCssClassCol6, ContainerRowIndex = 0)]
        public string Email { get; set; }

        [ControlText(ReadOnly = true, LabelText = "User Name", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 1)]
        public string UserName { get; set; }

        [Required]
        [Display(Name = "Current Password")]
        [ControlText(Type = ControlText.Password, Required = true, LabelText = "Current Password", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 1)]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [ControlText(Type = ControlText.Password, Required = true, LabelText = "New Password", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 2)]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm New Password")]
        [ControlText(Type = ControlText.Password, Required = true, EqualTo = "Password", LabelText = "Confirm New Password", ContainerCssClass = Constants.ContainerCssClassCol3, ContainerRowIndex = 2)]
        public string ConfirmPassword { get; set; }
    }

    public class PasswordRecoveryModel
    {
        [Display(Name = "Your email address")]
        [ControlText(Type = ControlText.Email, Required = true, LabelText = "Your email address")]
        public string Email { get; set; }
    }

    public class ResetPasswordModel
    {
        [ControlHidden]
        public string ResetToken { get; set; }

        [Required]
        [Display(Name = "New Password")]
        [ControlText(Type = ControlText.Password, Required = true, LabelText = "New Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Confirm New Password")]
        [ControlText(Type = ControlText.Password, Required = true, EqualTo = "Password", LabelText = "Confirm New Password")]
        public string ConfirmPassword { get; set; }
    }

    public class LocalPasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Current password")]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "New password")]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm new password")]
        [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        [ControlText(Required = true, LabelText = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [ControlText(Type = ControlText.Password, Required = true)]
        public string Password { get; set; }

        [Display(Name = "Remember me")]
        [ControlChoice(ControlChoice.CheckBox, PrependText = "Remember me", RenderLabelControl = false)]
        public bool RememberMe { get; set; }

        [ControlHidden]
        public string ReturnUrl { get; set; }

        public bool AllowRegisterUser { get; set; }

        public bool AllowForgotPassword { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        [ControlText(Required = true, Order = -99, LabelText = "User name")]
        public string UserName { get; set; }

        [ControlText(Type = ControlText.Email, Required = true, Order = -98)]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [ControlText(Type = ControlText.Password, Required = true, Order = -97)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        [ControlText(Type = ControlText.Password, Required = true, EqualTo = "Password", Order = -96, LabelText = "Confirm password")]
        public string ConfirmPassword { get; set; }
    }

    public class ExternalLogin
    {
        public string Provider { get; set; }

        public string ProviderDisplayName { get; set; }

        public string ProviderUserId { get; set; }
    }
}