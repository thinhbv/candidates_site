using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Mvc;
using Newtonsoft.Json.Linq;
using CMSSolutions.Web.UI.JQueryBuilder;

namespace CMSSolutions.Web.Mvc
{
    public class AjaxResult : ActionResult
    {
        private readonly IList<ActionBase> actions;

        public AjaxResult()
        {
            actions = new List<ActionBase>();
        }

        public override void ExecuteResult(ControllerContext context)
        {
            var sb = new StringBuilder();

            foreach (var action in actions)
            {
                action.ExecuteResult(context, sb);
            }

            var isAjaxRequest = context.HttpContext.Request.IsAjaxRequest();
            if (isAjaxRequest)
            {
                var result = new ContentResult { Content = sb.ToString(), ContentType = "application/x-javascript" };
                result.ExecuteResult(context);
            }
            else
            {
                sb.Insert(0, "<html><head><script type=\"text/javascript\">");
                sb.Append("</script></head></html>");

                var result = new ContentResult { Content = sb.ToString(), ContentType = "text/html" };
                result.ExecuteResult(context);
            }
        }

        public AjaxResult CloseModalDialog(string returnValue = null)
        {
            actions.Add(new CloseModalDialogAction(returnValue));
            return this;
        }

        public AjaxResult Reload(bool parentTarget = false)
        {
            return Redirect(null, parentTarget);
        }

        public AjaxResult Redirect(string redirectUrl, bool parentTarget = false)
        {
            actions.Add(new RedirectAction(redirectUrl, parentTarget));
            return this;
        }

        public AjaxResult Alert(string message)
        {
            actions.Add(new AlertAction(message));
            return this;
        }

        public AjaxResult NotifyMessage(string message, object data = null)
        {
            actions.Add(new NotifyMessageAction(message, data));
            return this;
        }

        public AjaxResult UpdateContent(string controlId, string content)
        {
            actions.Add(new UpdateContentAction(controlId, content));
            return this;
        }

        public AjaxResult ExecuteScript(string script)
        {
            actions.Add(new ExecuteScriptAction(script));
            return this;
        }

        public AjaxResult OpenContentInNewWindow(string content)
        {
            actions.Add(new OpenContentInNewWindowAction(content));
            return this;
        }

        #region Ajax Actions

        private abstract class ActionBase
        {
            public abstract void ExecuteResult(ControllerContext context, StringBuilder scriptBuilder);
        }

        private class RedirectAction : ActionBase
        {
            private readonly string redirectUrl;
            private readonly bool parentTarget;

            public RedirectAction(string redirectUrl, bool parentTarget)
            {
                this.redirectUrl = redirectUrl;
                this.parentTarget = parentTarget;
            }

            public override void ExecuteResult(ControllerContext context, StringBuilder scriptBuilder)
            {
                if (string.IsNullOrEmpty(redirectUrl))
                {
                    scriptBuilder.AppendLine(parentTarget
                                                ? "window.parent.location.reload();"
                                                : "window.location.reload();");
                }
                else
                {
                    scriptBuilder.AppendLine(parentTarget
                                                 ? string.Format("window.parent.location = {0};", JQueryUtility.EncodeJsString(redirectUrl))
                                                 : string.Format("window.location = {0};", JQueryUtility.EncodeJsString(redirectUrl)));
                }
            }
        }

        private class AlertAction : ActionBase
        {
            private readonly string message;

            public AlertAction(string message)
            {
                this.message = message;
            }

            public override void ExecuteResult(ControllerContext context, StringBuilder scriptBuilder)
            {
                scriptBuilder.AppendLine(string.Format("alert({0});", JQueryUtility.EncodeJsString(message)));
            }
        }

        private class NotifyMessageAction : ActionBase
        {
            private readonly string systemMessage;
            private readonly object data;

            public NotifyMessageAction(string systemMessage, object data = null)
            {
                this.systemMessage = systemMessage;
                this.data = data;
            }

            public override void ExecuteResult(ControllerContext context, StringBuilder scriptBuilder)
            {
                scriptBuilder.AppendLine(string.Format("if(self != top){{ parent.$('body').trigger({{ type: 'SystemMessageEvent', SystemMessage: {0}, Data: {1} }}); }} $('body').trigger({{ type: 'SystemMessageEvent', SystemMessage: {0}, Data: {1} }});", JQueryUtility.EncodeJsString(systemMessage), data != null ? JObject.FromObject(data).ToString() : "{}"));
            }
        }

        private class CloseModalDialogAction : ActionBase
        {
            private readonly string returnValue;

            public CloseModalDialogAction(string returnValue)
            {
                this.returnValue = returnValue;
            }

            public override void ExecuteResult(ControllerContext context, StringBuilder scriptBuilder)
            {
                if (returnValue != null)
                {
                    throw new NotImplementedException();
                }
                scriptBuilder.AppendLine(string.Format("parent.jQuery.fancybox.close();"));
            }
        }

        private class UpdateContentAction : ActionBase
        {
            private readonly string controlId;
            private readonly string content;

            public UpdateContentAction(string controlId, string content)
            {
                this.controlId = controlId;
                this.content = content;
            }

            public override void ExecuteResult(ControllerContext context, StringBuilder scriptBuilder)
            {
                scriptBuilder.AppendFormat("$('#{0}').html({1});", controlId, content);
            }
        }

        private class ExecuteScriptAction : ActionBase
        {
            private readonly string script;

            public ExecuteScriptAction(string script)
            {
                this.script = script;
            }

            public override void ExecuteResult(ControllerContext context, StringBuilder scriptBuilder)
            {
                scriptBuilder.Append(script);
            }
        }

        private class OpenContentInNewWindowAction : ActionBase
        {
            private readonly string content;

            public OpenContentInNewWindowAction(string content)
            {
                this.content = content;
            }

            public override void ExecuteResult(ControllerContext context, StringBuilder scriptBuilder)
            {
                scriptBuilder.AppendFormat("var w = window.open(); $(w.document.body).html('{0}');", content);
            }
        }

        #endregion Ajax Actions
    }
}