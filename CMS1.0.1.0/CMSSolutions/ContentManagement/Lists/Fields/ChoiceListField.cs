using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using CMSSolutions.Environment.Extensions;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists.Fields
{
    [Feature(Constants.Areas.Lists)]
    public class ChoiceListField : BaseListField
    {
        public override string FieldType
        {
            get { return "Choice Field"; }
        }

        public override void BindControlField(ControlFormResult controlForm, object value)
        {
            ControlChoiceAttribute attribute;

            switch (ChoiceType)
            {
                case ChoiceFieldType.DropDownList:
                    attribute = new ControlChoiceAttribute(ControlChoice.DropDownList);
                    break;

                case ChoiceFieldType.RadioButtonList:
                    attribute = new ControlChoiceAttribute(ControlChoice.RadioButtonList);
                    break;

                case ChoiceFieldType.CheckboxiesList:
                    attribute = new ControlChoiceAttribute(ControlChoice.CheckBoxList);
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }

            attribute.LabelText = Title;
            attribute.Required = Required;
            attribute.Order = Position;
            attribute.ContainerCssClass = "col-md-6";
            attribute.ContainerRowIndex = 10;

            var split = Values.Split(System.Environment.NewLine.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            var selectListItems = split.Select(str => new SelectListItem { Text = str, Value = str }).ToList();

            if (!Required && ChoiceType == ChoiceFieldType.DropDownList)
            {
                selectListItems.Insert(0, new SelectListItem());
            }

            attribute.SelectListItems = selectListItems;

            controlForm.AddProperty(Name, attribute, value);
        }

        [Display(Name = "Choice Type")]
        [ControlChoice(ControlChoice.DropDownList, LabelText = "Choice Type")]
        public ChoiceFieldType ChoiceType { get; set; }

        [ControlText(Type = ControlText.MultiText, HelpText = "Please enter each value per each line.", Required = true)]
        public string Values { get; set; }

        public enum ChoiceFieldType
        {
            [Display(Name = "Dropdown List")]
            DropDownList,

            [Display(Name = "Radio Button List")]
            RadioButtonList,

            [Display(Name = "Checkboxies List")]
            CheckboxiesList,
        }
    }
}