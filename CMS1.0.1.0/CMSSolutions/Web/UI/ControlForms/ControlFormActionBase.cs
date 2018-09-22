using System.Collections.Generic;
using CMSSolutions.Localization;

namespace CMSSolutions.Web.UI.ControlForms
{
    public abstract class ControlFormActionBase<T> where T : ControlFormActionBase<T>
    {
        protected ControlFormActionBase(bool isSubmitButton, bool isValidationSupported)
        {
            IsSubmitButton = isSubmitButton;
            IsValidationSupported = isValidationSupported;
            HtmlAttributes = new Dictionary<string, string>();
            ShowBoxButton = true;
        }

        public ButtonSize ButtonSize { get; set; }

        public ButtonStyle ButtonStyle { get; set; }

        public string ClientClickCode { get; set; }

        public string ClientId { get; set; }

        public string ConfirmMessage { get; set; }

        public string CssClass { get; set; }

        public string Description { get; set; }

        public Dictionary<string, string> HtmlAttributes { get; set; }

        public string IconCssClass { get; set; }

        public bool IsShowModalDialog { get; set; }

        public bool IsSubmitButton { get; set; }

        public bool IsValidationSupported { get; set; }

        public int ModalDialogWidth { get; set; }

        public int ModalDialogHeight { get; set; }

        public string Name { get; set; }

        public string Text { get; set; }

        public bool NewRow { get; set; }

        public string ParentClass { get; set; }

        public bool ShowBoxButton { get; set; }

        public T HasAttribute(string key, string value)
        {
            HtmlAttributes.Add(key, value);
            return (T)this;
        }

        public T HasButtonSize(ButtonSize buttonSize)
        {
            ButtonSize = buttonSize;
            return (T)this;
        }

        public T HasButtonStyle(ButtonStyle style)
        {
            ButtonStyle = style;
            return (T)this;
        }

        public T HasClientId(string id)
        {
            ClientId = id;
            return (T)this;
        }

        public T HasConfirmMessage(string value)
        {
            ConfirmMessage = value;
            return (T)this;
        }

        public T HasCssClass(string value)
        {
            CssClass = value;
            return (T)this;
        }

        public T HasRow(bool newRow)
        {
            NewRow = newRow;
            return (T)this;
        }

        public T HasParentClass(string parentClass)
        {
            ParentClass = parentClass;
            return (T)this;
        }

        public T HasBoxButton(bool showBoxButton)
        {
            ShowBoxButton = showBoxButton;
            return (T)this;
        }

        public T HasDescription(LocalizedString value)
        {
            Description = value.Text;
            return (T)this;
        }

        public T HasIconCssClass(string value)
        {
            IconCssClass = value;
            return (T)this;
        }

        public T HasName(string value)
        {
            Name = value;
            return (T)this;
        }

        public T HasText(string value)
        {
            Text = value;
            return (T)this;
        }

        public T OnClientClick(string jsCode)
        {
            ClientClickCode = jsCode;
            return (T)this;
        }

        public T ShowModalDialog(int width = 600, int height = 500)
        {
            IsShowModalDialog = true;
            ModalDialogWidth = width;
            ModalDialogHeight = height;
            return (T)this;
        }

        public virtual string GetConfirmMessage()
        {
            return ConfirmMessage;
        }
    }
}