using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Sliders.Models
{
    public class SublayerModel
    {
        public SublayerModel()
        {
            Top = "0px";
            Left = "0px";
        }

        [ControlHidden]
        public string Id { get; set; }

        [ControlHidden]
        public string Tag { get; set; }

        [ControlHidden]
        public string Top { get; set; }

        [ControlHidden]
        public string Left { get; set; }

        [ControlFileUpload(EnableFineUploader = true, AllowBrowseOnServer = true, UploadFolder = "Slides\\Sublayers", LabelText = "Image Source", ContainerCssClass = "col-xs-6", ContainerRowIndex = 0)]
        public string ImageSource { get; set; }

        [ControlText(Required = true, LabelText = "Html Content", ContainerCssClass = "col-xs-6", ContainerRowIndex = 0)]
        public string HtmlContent { get; set; }

        [ControlText(LabelText = "Font Family", ContainerCssClass = "col-xs-6", ContainerRowIndex = 0)]
        public string FontFamily { get; set; }

        [ControlText(LabelText = "Font Size", ContainerCssClass = "col-xs-6", ContainerRowIndex = 1)]
        public string FontSize { get; set; }

        [ControlText(ContainerCssClass = "col-xs-6", ContainerRowIndex = 1)]
        public string Color { get; set; }

        [ControlText(LabelText = "Background Color", ContainerCssClass = "col-xs-6", ContainerRowIndex = 2)]
        public string BackgroundColor { get; set; }

        [ControlChoice(ControlChoice.DropDownList, LabelText = "Slide Direction", ContainerCssClass = "col-xs-3", ContainerRowIndex = 3)]
        public string SlideDirection { get; set; }

        [ControlChoice(ControlChoice.DropDownList, LabelText = "Slide Out Direction", ContainerCssClass = "col-xs-3", ContainerRowIndex = 3)]
        public string SlideOutDirection { get; set; }

        [ControlNumeric(LabelText = "Duration In", ContainerCssClass = "col-xs-3", ContainerRowIndex = 3)]
        public int? DurationIn { get; set; }

        [ControlNumeric(LabelText = "Duration Out", ContainerCssClass = "col-xs-3", ContainerRowIndex = 3)]
        public int? DurationOut { get; set; }

        [ControlChoice(ControlChoice.DropDownList, LabelText = "Easing In", ContainerCssClass = "col-xs-3", ContainerRowIndex = 4)]
        public string EasingIn { get; set; }

        [ControlChoice(ControlChoice.DropDownList, LabelText = "Easing Out", ContainerCssClass = "col-xs-3", ContainerRowIndex = 4)]
        public string EasingOut { get; set; }

        [ControlNumeric(LabelText = "Rotate In", ContainerCssClass = "col-xs-3", ContainerRowIndex = 4)]
        public int? RotateIn { get; set; }

        [ControlNumeric(LabelText = "Rotate Out", ContainerCssClass = "col-xs-3", ContainerRowIndex = 4)]
        public int? RotateOut { get; set; }
    }
}