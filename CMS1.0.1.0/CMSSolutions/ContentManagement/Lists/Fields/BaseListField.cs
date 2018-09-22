using System;
using System.Web.Mvc;
using CMSSolutions.Serialization;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists.Fields
{
    public abstract class BaseListField : IListField
    {
        public abstract string FieldType { get; }

        [ControlHidden, ExcludeFromSerialization]
        public int Id { get; set; }

        [ControlText(Required = true, MaxLength = 255, Order = -9999)]
        public string Title { get; set; }

        [ControlText(Required = true, MaxLength = 255, Order = -9998)]
        public string Name { get; set; }

        [ControlNumeric(Required = true, Order = -9997)]
        public int Position { get; set; }

        [ControlHidden]
        public int ListId { get; set; }

        [ControlChoice(ControlChoice.CheckBox, Order = -9996)]
        public bool Required { get; set; }

        public abstract void BindControlField(ControlFormResult controlForm, object value);

        public virtual object GetControlFormValue(Controller controller, WorkContext workContext)
        {
            return controller.Request.Form[Name];
        }

        public virtual object RenderField(object value, object[] args)
        {
            return Convert.ToString(value);
        }

        public virtual object Clone()
        {
            return MemberwiseClone();
        }
    }
}