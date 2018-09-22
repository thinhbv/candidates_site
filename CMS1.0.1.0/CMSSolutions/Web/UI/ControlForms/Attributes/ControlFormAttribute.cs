using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using CMSSolutions.Collections.Generic;

namespace CMSSolutions.Web.UI.ControlForms
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public abstract class ControlFormAttribute : Attribute, ICloneable<ControlFormAttribute>
    {
        protected ControlFormAttribute()
        {
            Init();
        }

        private void Init()
        {
            HasLabelControl = true;
            ContainerCssClass = "col-md-12";
            ContainerRowIndex = -100;
        }

        public string Name { get; set; }

        public int Order { get; set; }

        public string CssClass { get; set; }

        public bool Required { get; set; }

        public string RequiredIf { get; set; }

        public bool ReadOnly { get; set; }

        public string LabelText { get; set; }

        public string HelpText { get; set; }

        public virtual bool HasLabelControl { get; set; }

        public virtual bool HideLabelControl { get; set; }

        public string PropertyName { get; set; }

        public Type PropertyType { get; set; }

        public PropertyInfo PropertyInfo { get; set; }

        public string DataBind { get; set; }

        public int ContainerRowIndex { get; set; }

        public string ContainerCssClass { get; set; }
        
        public string ContainerDataBind { get; set; }

        public string LabelCssClass { get; set; }
        
        public string ControlContainerCssClass { get; set; }

        public int ColumnWidth { get; set; }

        public object Value { get; internal set; }

        public string ControlSpan { get; set; }

        public string AppendText { get; set; }

        public string PrependText { get; set; }

        #region Overrides

        public virtual IEnumerable<ResourceType> GetAdditionalResources()
        {
            return Enumerable.Empty<ResourceType>();
        }

        public virtual string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper)
            where TModel : class
        {
            return SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }

        public abstract string SelfGenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, WorkContext workContext, HtmlHelper htmlHelper) where TModel : class;

        #endregion Overrides

        #region Helpers

        protected static string MergeCssClass(params string[] cssClass)
        {
            if (cssClass == null || cssClass.Length == 0)
            {
                return null;
            }

            return string.Join(" ", cssClass.Where(x => !string.IsNullOrEmpty(x)));
        }

        #endregion Helpers

        #region Implementation of ICloneable<ControlFormAttribute>

        public ControlFormAttribute ShallowCopy()
        {
            return (ControlFormAttribute)MemberwiseClone();
        }

        public ControlFormAttribute DeepCopy()
        {
            return ShallowCopy();
        }

        #endregion Implementation of ICloneable<ControlFormAttribute>
    }
}