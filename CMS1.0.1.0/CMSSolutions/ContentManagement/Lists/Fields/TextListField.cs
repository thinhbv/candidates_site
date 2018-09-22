using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists.Fields
{
    [Feature(Constants.Areas.Lists)]
    public class TextListField : BaseListField
    {
        public TextListField()
        {
            MaxLength = 255;
        }

        public override string FieldType
        {
            get { return "Text Field"; }
        }

        [Display(Name = "Text Type")]
        [ControlChoice(ControlChoice.DropDownList, LabelText = "Text Type")]
        public TextFieldType TextType { get; set; }

        [Display(Name = "Max Length")]
        [ControlNumeric(Required = true, MinimumValue = "0", LabelText = "Max Length")]
        public int MaxLength { get; set; }

        public override void BindControlField(ControlFormResult controlForm, object value)
        {
            var attribute = new ControlTextAttribute();

            switch (TextType)
            {
                case TextFieldType.SingleLine:
                    attribute.Type = ControlText.TextBox;
                    attribute.MaxLength = MaxLength;
                    break;

                case TextFieldType.Email:
                    attribute.Type = ControlText.Email;
                    attribute.MaxLength = MaxLength;
                    break;

                case TextFieldType.Url:
                    attribute.Type = ControlText.Url;
                    attribute.MaxLength = MaxLength;
                    break;

                case TextFieldType.MultiLine:
                    attribute.Type = ControlText.MultiText;
                    break;

                case TextFieldType.RichText:
                    attribute.Type = ControlText.RichText;
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            attribute.LabelText = Title;
            attribute.Required = Required;
            attribute.Order = Position;
            attribute.ContainerCssClass = "col-md-6";
            attribute.ContainerRowIndex = 10;

            if (TextType == TextFieldType.RichText || TextType == TextFieldType.MultiLine)
            {
                attribute.ContainerCssClass = "col-md-12";    
            }

            controlForm.AddProperty(Name, attribute, value);
        }

        public override object RenderField(object value, object[] args)
        {
            if (TextType == TextFieldType.RichText)
            {
                return MvcHtmlString.Create(Convert.ToString(value));
            }

            return base.RenderField(value, args);
        }

        public enum TextFieldType
        {
            [Display(Name = "Single Line of Text")]
            SingleLine,

            [Display(Name = "Email")]
            Email,

            [Display(Name = "Url")]
            Url,

            [Display(Name = "Multi Line of Text")]
            MultiLine,

            [Display(Name = "Rich Text")]
            RichText
        }
    }
}