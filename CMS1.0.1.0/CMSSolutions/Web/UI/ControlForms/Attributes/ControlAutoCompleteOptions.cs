using System;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class ControlAutoCompleteOptions
    {
        public string SourceUrl { get; set; }

        public virtual bool HasTextSelector { get { return false; } }

        public virtual string GetText(object model)
        {
            return null;
        }
    }

    public class ControlAutoCompleteOptions<TModel> : ControlAutoCompleteOptions
    {
        public override bool HasTextSelector
        {
            get { return TextSelector != null; }
        }

        public Func<TModel, string> TextSelector { get; set; }

        public override string GetText(object model)
        {
            return TextSelector != null ? TextSelector((TModel)model) : null;
        }
    }
}