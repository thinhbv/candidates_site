using System.Linq;
using CMSSolutions.Configuration;
using CMSSolutions.Localization;
using CMSSolutions.Localization.Services;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.Web
{
    public class SiteSettings : ISettings
    {
        private string defaultLanguage;

        public SiteSettings()
        {
            Name = "CMS Solutions";
            DefaultPageSize = 10;
            Theme = "Default";
            DefaultLanguage = "vi-VN";
        }

        [ControlText(Required = true, LabelText = "Site Name", MaxLength = 255)]
        public string Name { get; set; }

        [ControlText(Required = true, LabelText = "Homepage Title", MaxLength = 255)]
        public string HomepageTitle { get; set; }

        [ControlText]
        public string Copyright { get; set; }

        [ControlNumeric(Required = true, LabelText = "Items Per Page")]
        public int DefaultPageSize { get; set; }

        [ControlHidden]
        public string Theme { get; set; }

        [ControlChoice(ControlChoice.DropDownList, LabelText = "Default Language")]
        public string DefaultLanguage
        {
            get { return string.IsNullOrEmpty(defaultLanguage) ? "vi-VN" : defaultLanguage; }
            set { defaultLanguage = value; }
        }

        [ControlText(Type = ControlText.Url, LabelText = "Facebook Link")]
        public string FacebookLink { get; set; }

        [ControlText(Type = ControlText.Url, LabelText = "Google+ Link")]
        public string GooglePlusLink { get; set; }

        [ControlText(Type = ControlText.Url, LabelText = "Twitter Link")]
        public string TwitterLink { get; set; }

        [ControlText(Type = ControlText.Url, LabelText = "Tumblr Link")]
        public string TumblrLink { get; set; }

        [ControlText(Type = ControlText.Url, LabelText = "RSS Link")]
        public string RssLink { get; set; }

        [ControlText(Type = ControlText.Url, LabelText = "YouTube Link")]
        public string YoutubeLink { get; set; }

        string ISettings.Name { get { return "Site Settings"; } }

        bool ISettings.Hidden { get { return false; } }

        void ISettings.OnEditing(ControlFormResult<ISettings> controlForm, WorkContext workContext)
        {
            var languageManager = workContext.Resolve<ILanguageManager>();
            controlForm.RegisterExternalDataSource("DefaultLanguage", languageManager.GetActiveLanguages(Constants.ThemeDefault, false).ToDictionary(k => k.CultureCode, v => v.Name));
        }
    }
}