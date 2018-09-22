using CMSSolutions.Configuration;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web
{
    public class CaptchaSettings : ISettings
    {
        [ControlText(Required = true, LabelText = "Public Key", MaxLength = 255)]
        public string PublicKey { get; set; }

        [ControlText(Required = true, LabelText = "Private Key", MaxLength = 255)]
        public string PrivateKey { get; set; }

        #region Implementation of ISettings

        string ISettings.Name { get { return "Captcha Settings"; } }

        bool ISettings.Hidden { get { return false; } }

        void ISettings.OnEditing(ControlFormResult<ISettings> controlForm, WorkContext workContext)
        {
        }

        #endregion Implementation of ISettings
    }
}