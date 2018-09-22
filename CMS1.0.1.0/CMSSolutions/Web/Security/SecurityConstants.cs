namespace CMSSolutions.Web.Security
{
    public static class SecurityConstants
    {
        public const string RolePopulated = @"The role ""{0}"" cannot be deleted because there are still users in the role.";
        public const string ArgumentCannotBeNullOrEmpty = "Argument cannot be null or empty.";
        public const string ConfirmDeleteItem = "return confirm('Are you sure you want to delete this item?')";
        public const string ErrorLockedOut = "Your account has been locked. Please contact the webmaster to get it unlocked.";
        public const string ErrorInvalidLogin = "The Username or Password is incorrect.";
        public const string ErrorUserNameRequired = "UserName is required.";
        public const string ErrorPasswordRequired = "Password is required.";
        public const string ErrorPasswordTooLong = "The password provided is too long.";
        public const string ErrorConfigKey = "Hãy đăng ký sử dụng tên miền tại địa chỉ http://www.laptrinhmvc.com/download.html hoặc thay đổi cấu hình IsLoginWithLocal=True trong web.config.";
        public const string ErrorChangePassword = "The current password is incorrect or the new password is invalid.";
        public const string ErrorInvalidPasswordResetToken = "The password reset token is incorrect or may be expired.";
        public const string ResetPasswordError = "The password could not be reset. Please contact the administrator.";
        public const string ErrorDuplicateRole = "There is already a role named, '{0}'.";
        public const string ErrorDuplicateUser = "User name already exists. Please enter a different user name.";
        public const string ErrorDuplicateEmail = "A user for that e-mail address already exists. Please enter a different e-mail address.";
        public const string ErrorUserNotConfirmed = "This account has not yet been confirmed.";
        public const string ErrorAccountNotFound = "No account exists for \"{0}\".";
        public const string SuccessResetPassword = "Your password has successfully been reset";
        public const string SuccessRegister = "Thank you for registering.";
        public const string SuccessUpdateUserProfile = "User profile has successfully been update.";
    }
}