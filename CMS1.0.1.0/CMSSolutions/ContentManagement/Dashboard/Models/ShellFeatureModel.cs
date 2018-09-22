using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Dashboard.Models
{
    public class ShellFeatureModel
    {
        [ControlText(Required = true, LabelText = "Assembly Name", MaxLength = 255, ContainerCssClass = "col-xs-12 col-md-12", ContainerRowIndex = 0)]
        public string Name { get; set; }

        [ControlText(Required = true, LabelText = "Category", MaxLength = 255, ContainerCssClass = "col-xs-12 col-md-12", ContainerRowIndex = 0)]
        public string Category { get; set; }
    }
}
