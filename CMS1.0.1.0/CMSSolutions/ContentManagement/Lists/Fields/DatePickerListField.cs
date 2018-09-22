using System.ComponentModel.DataAnnotations;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists.Fields
{
    [Feature(Constants.Areas.Lists)]
    public class DatePickerListField : BaseListField
    {
        public override string FieldType
        {
            get { return "Date Picker Field"; }
        }

        public override void BindControlField(ControlFormResult controlForm, object value)
        {
            var attribute = new ControlDatePickerAttribute
                            {
                                LabelText = Title,
                                Required = Required,
                                Order = Position,
                                DateFormat = DateFormat,
                                ContainerCssClass = "col-md-6",
                                ContainerRowIndex = 10
                            };

            controlForm.AddProperty(Name, attribute, value);
        }

        [ControlText(LabelText = "Date Format")]
        [Display(Name = "Date Format")]
        public string DateFormat { get; set; }
    }
}