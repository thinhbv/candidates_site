using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists.Fields
{
    [Feature(Constants.Areas.Lists)]
    public class NumericListField : BaseListField
    {
        public override string FieldType
        {
            get { return "Numeric Field"; }
        }

        public override void BindControlField(ControlFormResult controlForm, object value)
        {
            var attribute = new ControlNumericAttribute
                            {
                                LabelText = Title,
                                Required = Required,
                                Order = Position,
                                ContainerCssClass = "col-md-6",
                                ContainerRowIndex = 10
                            };

            controlForm.AddProperty(Name, attribute, value);
        }

        [ControlNumeric(MinimumValue = "0")]
        public int Decimals { get; set; }
    }
}