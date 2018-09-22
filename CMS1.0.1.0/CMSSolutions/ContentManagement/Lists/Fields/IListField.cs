using System;
using System.Web.Mvc;
using CMSSolutions.Web.UI.ControlForms;

namespace CMSSolutions.ContentManagement.Lists.Fields
{
    public interface IListField : ICloneable, IDependency
    {
        string FieldType { get; }

        int Id { get; set; }

        string Title { get; set; }

        string Name { get; set; }

        int Position { get; set; }

        int ListId { get; set; }

        [ControlChoice(ControlChoice.CheckBox, LabelText = "", AppendText = "Required")]
        bool Required { get; set; }

        void BindControlField(ControlFormResult controlForm, object value);

        object GetControlFormValue(Controller controller, WorkContext workContext);

        object RenderField(object value, object[] args);
    }
}