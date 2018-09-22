namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlFileUploadOptions
    {
        public string UploadUrl { get; set; }

        public string AllowedExtensions { get; set; }

        public int SizeLimit { get; set; }

        public string UploadFolder { get; set; }
    }
}