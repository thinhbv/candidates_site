using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class Foundation4ControlFormProvider : ControlFormProvider
    {
        private readonly StringBuilder builder;

        public Foundation4ControlFormProvider()
        {
            builder = new StringBuilder();
        }

        public override void WriteToOutput(string htmlString)
        {
            builder.Append(htmlString);
        }

        public override void WriteToOutput(params string[] inputControls)
        {
            if (inputControls != null && inputControls.Length != 0)
            {
                WriteToOutput("<div class=\"row\">");

                var renderSpaces = false;

                foreach (var inputControl in inputControls.Where(x => x != null))
                {
                    if (renderSpaces)
                    {
                        WriteToOutput("&nbsp;&nbsp;&nbsp;");
                    }
                    WriteToOutput(inputControl);
                    renderSpaces = true;
                }

                WriteToOutput("</div>");
            }
        }

        public override void WriteToOutput(ControlFormAttribute formAttribute, string inputControl)
        {
            throw new NotImplementedException();
        }

        public override void WriteToOutput(ControlFormAttribute formAttribute, params string[] inputControls)
        {
            if (inputControls == null || inputControls.Length == 0) return;

            WriteToOutput(formAttribute != null && !string.IsNullOrEmpty(formAttribute.ContainerDataBind)
                ? string.Format("<div class=\"row\" data-bind=\"{0}\">", formAttribute.ContainerDataBind)
                : "<div class=\"row\">");

            if (formAttribute == null)
            {
                WriteToOutput("<label>&nbsp;</label>");
            }
            else
            {
                if (formAttribute.HasLabelControl)
                {
                    WriteToOutput(formAttribute.HideLabelControl
                        ? string.Format("<label>&nbsp;</label>")
                        : string.Format("<label for=\"{1}\">{0}</label>",
                            formAttribute.LabelText ?? formAttribute.PropertyName ?? formAttribute.Name,
                            formAttribute.Name.Replace(".", "_").Replace("[", "_").Replace("]", "_")));
                }
            }

            var renderSpaces = false;

            foreach (var inputControl in inputControls.Where(x => x != null))
            {
                if (renderSpaces)
                {
                    WriteToOutput("&nbsp;&nbsp;&nbsp;");
                }
                WriteToOutput(inputControl);
                renderSpaces = true;
            }

            WriteToOutput("</div>");
        }

        public override void WriteActions(string formActionsContainerCssClass, string formActionsCssClass, params string[] htmlActions)
        {
            throw new NotImplementedException();
        }

        public override void WriteActions(IList<ControlFormAction> actions)
        {
            if (actions.Count == 0)
            {
                return;
            }

            WriteToOutput("<div style=\"display:block;clear:both;\"><div class=\"left\"><div class=\"button-bar\">");

            foreach (var action in actions)
            {
                WriteToOutput("<div class=\"button-group\">");
                WriteToOutput(action.Create(this));
                WriteToOutput("</div>");
            }

            WriteToOutput("</div></div></div>");
        }

        public override string GetHtmlString()
        {
            return builder.ToString();
        }

        public override string GetButtonSizeCssClass(ButtonSize buttonSize)
        {
            switch (buttonSize)
            {
                case ButtonSize.Default: return "button";
                case ButtonSize.Large: return "large button";
                case ButtonSize.Small: return "small button";
                case ButtonSize.ExtraSmall: return "tiny button";
                default:
                    throw new ArgumentOutOfRangeException("buttonSize");
            }
        }

        public override string GetButtonStyleCssClass(ButtonStyle buttonStyle)
        {
            switch (buttonStyle)
            {
                case ButtonStyle.Default: return "button secondary";
                case ButtonStyle.Primary: return "button primary";
                case ButtonStyle.Info: return "button secondary";
                case ButtonStyle.Success: return "button success";
                case ButtonStyle.Warning: return "button alert";
                case ButtonStyle.Danger: return "button alert";
                case ButtonStyle.Inverse: return "button secondary";
                case ButtonStyle.Link: return "button primary";
                default:
                    throw new ArgumentOutOfRangeException("buttonStyle");
            }
        }

        public override string ControlCssClass
        {
            get { return null; }
        }
    }
}