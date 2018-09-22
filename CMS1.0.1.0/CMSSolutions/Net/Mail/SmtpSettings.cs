using CMSSolutions.Configuration;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Net.Mail
{
    [Feature(Constants.Areas.Core)]
    public class SmtpSettings : ISettings
    {
        public SmtpSettings()
        {
            MaxTries = 3;
            MessagesPerBatch = 50;
        }

        string ISettings.Name { get { return "SMTP Settings"; } }

        bool ISettings.Hidden { get { return false; } }

        [ControlText(Required = true, MaxLength = 255, LabelText = "From Email")]
        public string FromAddress { get; set; }

        [ControlText(MaxLength = 255, LabelText = "Display Name")]
        public string DisplayName { get; set; }

        [ControlText(Required = true, MaxLength = 255)]
        public string Host { get; set; }

        [ControlNumeric(Required = true, MinimumValue = "1", MaximumValue = "65535")]
        public int Port { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Enable SSL")]
        public bool EnableSsl { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Use Default Credentials")]
        public bool UseDefaultCredentials { get; set; }

        [ControlText(MaxLength = 255)]
        public string Username { get; set; }

        [ControlText(Type = ControlText.Password, MaxLength = 255)]
        public string Password { get; set; }

        [ControlNumeric(Required = true, MinimumValue = "1", MaximumValue = "9999", LabelText = "Max Tries")]
        public int MaxTries { get; set; }

        [ControlNumeric(Required = true, MinimumValue = "1", MaximumValue = "9999", LabelText = "Messages Per Batch")]
        public int MessagesPerBatch { get; set; }

        public void OnEditing(ControlFormResult<ISettings> controlForm, WorkContext workContext)
        {
        }
    }
}
