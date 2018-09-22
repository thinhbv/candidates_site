using CMSSolutions.Configuration;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Pages
{
    [Feature(Constants.Areas.Pages)]
    public class PageSettings : ISettings
    {
        public PageSettings()
        {
            NumberOfPageVersionsToKeep = 5;
        }

        string ISettings.Name { get { return "CMS: Page Settings"; } }

        bool ISettings.Hidden { get { return false; } }

        [ControlNumeric(LabelText = "# Page Versions to Keep")]
        public short NumberOfPageVersionsToKeep { get; set; }

        public void OnEditing(ControlFormResult<ISettings> controlForm, WorkContext workContext)
        {
        }
    }
}