using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CMSSolutions.Web.UI.ControlForms
{
    public class BootstrapControlFormProvider : ControlFormProvider
    {
        public enum BootstrapVersion
        {
            Version2,
            Version3,
        }

        private readonly StringBuilder builder;
        private readonly string controlCssClass;

        public BootstrapControlFormProvider(BootstrapVersion version = BootstrapVersion.Version3)
        {
            builder = new StringBuilder();
            Version = version;
            switch (version)
            {
                case BootstrapVersion.Version2:
                    GroupCssClass = "control-group";
                    LabelCssClass = "control-label";
                    ControlsCssClass = "controls";
                    break;

                case BootstrapVersion.Version3:
                    GroupCssClass = "form-group";
                    LabelCssClass = "col-lg-3 col-sm-3 col-md-3 col-xs-3 control-label";
                    controlCssClass = "form-control";
                    ControlsCssClass = "col-lg-9 col-sm-9 col-md-9 col-xs-9";
                    break;

                default:
                    throw new ArgumentOutOfRangeException("version");
            }
        }

        public BootstrapVersion Version { get; private set; }

        public string GroupCssClass { get; set; }

        public string LabelCssClass { get; set; }

        public override string ControlCssClass
        {
            get { return controlCssClass; }
        }

        public string ControlsCssClass { get; set; }

        public override void WriteToOutput(string htmlString)
        {
            builder.Append(htmlString);
        }

        public override void WriteToOutput(params string[] inputControls)
        {
            if (inputControls != null && inputControls.Length != 0)
            {
                WriteToOutput(string.Format("<div class=\"{0}\">", GroupCssClass));

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
            if (!string.IsNullOrEmpty(formAttribute.ContainerCssClass))
            {
                builder.AppendFormat("<div class=\"{0}\">", formAttribute.ContainerCssClass);
            }

            builder.AppendFormat("<div class=\"form-group\" data-bind=\"{0}\">", formAttribute.ContainerDataBind);

            if (formAttribute.HasLabelControl && !formAttribute.HideLabelControl)
            {
                builder.AppendFormat("<label for=\"{2}\" class=\"{1}\">{0}</label>", formAttribute.LabelText ?? formAttribute.Name, formAttribute.LabelCssClass, formAttribute.Name.Replace(".", "_").Replace("[", "_").Replace("]", "_"));
            }

            if (string.IsNullOrEmpty(formAttribute.ControlContainerCssClass))
            {
                builder.Append(inputControl);
            }
            else
            {
                builder.AppendFormat("<div class=\"{0}\">", formAttribute.ControlContainerCssClass);
                builder.Append(inputControl);
                builder.Append("</div>");
            }

            builder.Append("</div>");

            if (!string.IsNullOrEmpty(formAttribute.ContainerCssClass))
            {
                builder.Append("</div>");
            }
        }

        public override void WriteToOutput(ControlFormAttribute formAttribute, params string[] inputControls)
        {
            if (inputControls == null || inputControls.Length == 0) return;

            var hasLabelControl = false;
            var controlsCssClass = string.Empty;

            WriteToOutput(formAttribute != null && !string.IsNullOrEmpty(formAttribute.ContainerDataBind)
                ? string.Format("<div class=\"{1}\" data-bind=\"{0}\">", formAttribute.ContainerDataBind, GroupCssClass)
                : string.Format("<div class=\"{0}\">", GroupCssClass));

            if (formAttribute != null)
            {
                if (formAttribute.HasLabelControl)
                {
                    hasLabelControl = true;
                    WriteToOutput(formAttribute.HideLabelControl
                        ? string.Format("<label class=\"{0}\">&nbsp;</label>", LabelCssClass)
                        : string.Format("<label class=\"{2}\" for=\"{1}\">{0}</label>",
                            formAttribute.LabelText ?? formAttribute.PropertyName ?? formAttribute.Name,
                            formAttribute.Name.Replace(".", "_").Replace("[", "_").Replace("]", "_"), LabelCssClass));
                }
            }
            else
            {
                hasLabelControl = true;
                controlsCssClass = " form-buttons";
            }

            if (hasLabelControl)
            {
                WriteToOutput(string.Format("<div class=\"{0}\">", ControlsCssClass + controlsCssClass));
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

            if (hasLabelControl)
            {
                WriteToOutput("</div>");
            }

            WriteToOutput("</div>");
        }

        public override void WriteActions(string formActionsContainerCssClass, string formActionsCssClass, params string[] htmlActions)
        {
            if (!string.IsNullOrEmpty(formActionsContainerCssClass) && !string.IsNullOrEmpty(formActionsCssClass))
            {
                builder.AppendFormat("<div class=\"{0}\">", formActionsContainerCssClass);

                builder.Append("<div class=\"row\"><div class=\"form-group\">");
                builder.AppendFormat("<div class=\"{0}\">", formActionsCssClass);
                var flag = false;
                foreach (var htmlAction in htmlActions)
                {
                    if (flag)
                    {
                        builder.Append("&nbsp;&nbsp;&nbsp;");
                    }
                    flag = true;
                    builder.Append(htmlAction);
                }

                builder.Append("</div></div></div></div>");
            }
            else
            {
                builder.AppendFormat("<div class=\"{0}\">", formActionsContainerCssClass ?? "col-md-12");
                var flag = false;
                foreach (var htmlAction in htmlActions)
                {
                    if (flag)
                    {
                        builder.Append("&nbsp;&nbsp;&nbsp;");
                    }
                    flag = true;
                    builder.Append(htmlAction);
                }
                builder.Append("</div>");
            }
        }

        public override void WriteActions(IList<ControlFormAction> actions)
        {
            if (actions.Count == 0)
            {
                return;
            }

            WriteToOutput(string.Format("<div class=\"{0}\">", GroupCssClass));

            WriteToOutput("<div class=\"row btn-toolbar\">");
            foreach (var action in actions)
            {
                if (action.ShowBoxButton)
                {
                    if (!string.IsNullOrEmpty(action.ParentClass))
                    {
                        WriteToOutput("<div class=\"" + action.ParentClass + "\">");
                    }
                    else
                    {
                        WriteToOutput("<div class=\"btn-group\">");
                    }
                }
               
                WriteToOutput(action.Create(this));
                if (action.ShowBoxButton)
                {
                    WriteToOutput("</div>");
                }

                if (action.NewRow)
                {
                    WriteToOutput("<div class=\"row\"></div>");
                }
            }

            WriteToOutput("</div></div>");
        }

        public override string GetHtmlString()
        {
            return builder.ToString();
        }

        public override string GetButtonSizeCssClass(ButtonSize buttonSize)
        {
            if (Version == BootstrapVersion.Version2)
            {
                switch (buttonSize)
                {
                    case ButtonSize.Default: return "btn";
                    case ButtonSize.Large: return "btn btn-large";
                    case ButtonSize.Small: return "btn btn-small";
                    case ButtonSize.ExtraSmall: return "btn btn-mini";
                    default:
                        throw new ArgumentOutOfRangeException("buttonSize");
                }
            }

            switch (buttonSize)
            {
                case ButtonSize.Default: return "btn";
                case ButtonSize.Large: return "btn btn-lg";
                case ButtonSize.Small: return "btn btn-sm";
                case ButtonSize.ExtraSmall: return "btn btn-xs";
                default:
                    throw new ArgumentOutOfRangeException("buttonSize");
            }
        }

        public override string GetButtonStyleCssClass(ButtonStyle buttonStyle)
        {
            switch (buttonStyle)
            {
                case ButtonStyle.Default: return "btn-default";
                case ButtonStyle.Primary: return "btn-primary";
                case ButtonStyle.Info: return "btn-info";
                case ButtonStyle.Success: return "btn-success";
                case ButtonStyle.Warning: return "btn-warning";
                case ButtonStyle.Danger: return "btn-danger";
                case ButtonStyle.Inverse: return "btn-inverse";
                case ButtonStyle.Link: return "btn-link";
                default:
                    throw new ArgumentOutOfRangeException("buttonStyle");
            }
        }
    }
}