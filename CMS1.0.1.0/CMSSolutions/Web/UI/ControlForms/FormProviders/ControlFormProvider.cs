using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace CMSSolutions.Web.UI.ControlForms
{
    public abstract class ControlFormProvider
    {
        static ControlFormProvider()
        {
            DefaultFormProvider = () => new BootstrapControlFormProvider();
        }

        public static Func<ControlFormProvider> DefaultFormProvider { get; set; }

        public abstract string ControlCssClass { get; }

        public virtual string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, ControlAutoCompleteAttribute controlAttribute, WorkContext workContext, HtmlHelper htmlHelper) where TModel : class
        {
            return controlAttribute.SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }

        public virtual string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, ControlCascadingCheckBoxListAttribute controlAttribute, WorkContext workContext, HtmlHelper htmlHelper) where TModel : class
        {
            return controlAttribute.SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }

        public virtual string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, ControlCascadingDropDownAttribute controlAttribute, WorkContext workContext, HtmlHelper htmlHelper) where TModel : class
        {
            return controlAttribute.SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }

        public virtual string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, ControlChoiceAttribute controlAttribute, WorkContext workContext, HtmlHelper htmlHelper) where TModel : class
        {
            return controlAttribute.SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }

        public virtual string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, ControlDatePickerAttribute controlAttribute, WorkContext workContext, HtmlHelper htmlHelper) where TModel : class
        {
            return controlAttribute.SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }

        public virtual string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, ControlFileUploadAttribute controlAttribute, WorkContext workContext, HtmlHelper htmlHelper) where TModel : class
        {
            return controlAttribute.SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }

        public virtual string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, ControlGridAttribute controlAttribute, WorkContext workContext, HtmlHelper htmlHelper) where TModel : class
        {
            return controlAttribute.SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }

        public virtual string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, ControlImageAttribute controlAttribute, WorkContext workContext, HtmlHelper htmlHelper) where TModel : class
        {
            return controlAttribute.SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }

        public virtual string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, ControlLabelAttribute controlAttribute, WorkContext workContext, HtmlHelper htmlHelper) where TModel : class
        {
            return controlAttribute.SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }

        public virtual string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, ControlNumericAttribute controlAttribute, WorkContext workContext, HtmlHelper htmlHelper) where TModel : class
        {
            return controlAttribute.SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }

        public virtual string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, ControlSlugAttribute controlAttribute, WorkContext workContext, HtmlHelper htmlHelper) where TModel : class
        {
            return controlAttribute.SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }

        public virtual string GenerateControlUI<TModel>(ControlFormResult<TModel> controlForm, ControlTextAttribute controlAttribute, WorkContext workContext, HtmlHelper htmlHelper) where TModel : class
        {
            return controlAttribute.SelfGenerateControlUI(controlForm, workContext, htmlHelper);
        }

        public abstract string GetButtonSizeCssClass(ButtonSize buttonSize);

        public abstract string GetButtonStyleCssClass(ButtonStyle buttonStyle);

        public abstract string GetHtmlString();

        public abstract void WriteActions(string formActionsContainerCssClass, string formActionsCssClass, params string[] htmlActions);

        public abstract void WriteActions(IList<ControlFormAction> actions);

        public abstract void WriteToOutput(string htmlString);

        public abstract void WriteToOutput(params string[] inputControls);

        public abstract void WriteToOutput(ControlFormAttribute formAttribute, string inputControl);

        public abstract void WriteToOutput(ControlFormAttribute formAttribute, params string[] inputControls);
    }
}