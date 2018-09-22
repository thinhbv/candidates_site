using CMSSolutions.Configuration;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.SEO.Models
{
    [Feature(Constants.Areas.SEO)]
    public class ControltsSettings : ISettings
    {
        string ISettings.Name { get { return "Controlts Settings"; } }

        bool ISettings.Hidden { get { return false; } }

        [ControlText(Type = ControlText.MultiText, Rows = 6, LabelText = "Your Controlts.txt File")]
        public string Content { get; set; }

        public void OnEditing(ControlFormResult<ISettings> controlForm, WorkContext workContext)
        {
        }
    }
}
