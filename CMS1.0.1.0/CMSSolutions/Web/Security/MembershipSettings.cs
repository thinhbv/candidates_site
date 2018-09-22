using CMSSolutions.Configuration;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web.Security
{
    public class MembershipSettings : ISettings
    {
        public MembershipSettings()
        {
            MaxInvalidPasswordAttempts = 5;
            PasswordAttemptWindow = 10;
            MinRequiredPasswordLength = 5;
            MinRequiredNonAlphanumericCharacters = 1;
            EnablePasswordReset = true;
            RequireConfirmation = true;
            AllowRegisterUser = false;
            AllowForgotPassword = false;
            ErrorLoginMessage = "The Username/Password is incorrect.";
        }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Allow Register User")]
        public bool AllowRegisterUser { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Allow Forgot Password")]
        public bool AllowForgotPassword { get; set; }

        [ControlNumeric(Required = true, MinimumValue = "0", LabelText = "Max Invalid Password Attempts")]
        public int MaxInvalidPasswordAttempts { get; set; }

        [ControlNumeric(Required = true, MinimumValue = "0", LabelText = "Password Attempt Window")]
        public int PasswordAttemptWindow { get; set; }

        [ControlNumeric(Required = true, MinimumValue = "3", LabelText = "Min Required Password Length")]
        public int MinRequiredPasswordLength { get; set; }

        [ControlNumeric(Required = true, MinimumValue = "0", LabelText = "Min Required Non Alphanumeric Characters")]
        public int MinRequiredNonAlphanumericCharacters { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Enable Password Reset")]
        public bool EnablePasswordReset { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Require Confirmation")]
        public bool RequireConfirmation { get; set; }

        [ControlText(LabelText = "Error Login Message")]
        public string ErrorLoginMessage { get; set; }

        #region Implementation of ISettings

        string ISettings.Name { get { return "Membership Settings"; } }

        bool ISettings.Hidden { get { return false; } }

        void ISettings.OnEditing(ControlFormResult<ISettings> controlForm, WorkContext workContext)
        {
        }

        #endregion Implementation of ISettings
    }
}