using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using CMSSolutions.DisplayManagement;
using CMSSolutions.Web.Mvc;
using CMSSolutions.Web.Mvc.Filters;

namespace CMSSolutions.Web.UI.Notify
{
    public class NotifyFilter : FilterProvider, IActionFilter, IResultFilter
    {
        private readonly INotifier notifier;
        private readonly IWorkContextAccessor workContextAccessor;
        private readonly dynamic shapeFactory;

        private const string TempDataMessages = "messages";

        public NotifyFilter(INotifier notifier, IWorkContextAccessor workContextAccessor, IShapeFactory shapeFactory)
        {
            this.notifier = notifier;
            this.workContextAccessor = workContextAccessor;
            this.shapeFactory = shapeFactory;
        }

        public void OnActionExecuting(ActionExecutingContext filterContext)
        {
        }

        public void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!notifier.List().Any())
                return;

            var tempData = filterContext.Controller.TempData;

            var sb = new StringBuilder();

            if (tempData.ContainsKey(TempDataMessages))
            {
                sb.Append(tempData[TempDataMessages]);
            }

            foreach (var entry in notifier.List())
            {
                sb.Append(Convert.ToString(entry.Type))
                    .Append(':')
                    .AppendLine(entry.Message)
                    .AppendLine("-");
            }

            tempData[TempDataMessages] = sb.ToString();
        }

        public void OnResultExecuting(ResultExecutingContext filterContext)
        {
            var viewResult = filterContext.Result as ViewResultBase;
            var ajaxResult = filterContext.Result as AjaxResult;

            if (viewResult == null && ajaxResult == null)
                return;

            var messages = Convert.ToString(filterContext.Controller.TempData[TempDataMessages]);
            if (string.IsNullOrEmpty(messages))
                return;

            var messageEntries = new List<NotifyEntry>();
            foreach (var line in messages.Split(new[] { System.Environment.NewLine + "-" + System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                var delimiterIndex = line.IndexOf(':');
                if (delimiterIndex != -1)
                {
                    var type = (NotifyType)Enum.Parse(typeof(NotifyType), line.Substring(0, delimiterIndex));
                    var message = line.Substring(delimiterIndex + 1);
                    if (messageEntries.All(ne => ne.Message != message))
                    {
                        messageEntries.Add(new NotifyEntry
                        {
                            Type = type,
                            Message = message
                        });
                    }
                }
                else
                {
                    var message = line.Substring(delimiterIndex + 1);
                    if (messageEntries.All(ne => ne.Message != message))
                    {
                        messageEntries.Add(new NotifyEntry
                        {
                            Type = NotifyType.Info,
                            Message = message
                        });
                    }
                }
            }

            if (!messageEntries.Any())
                return;

            if (ajaxResult != null)
            {
                foreach (var messageEntry in messageEntries)
                {
                    switch (messageEntry.Type)
                    {
                        case NotifyType.Info:
                            ajaxResult.NotifyMessage("NOTIFY_INFO", new { messageEntry.Message });
                        break;
                        case NotifyType.Warning:
                            ajaxResult.NotifyMessage("NOTIFY_WARNING", new { messageEntry.Message });
                        break;
                        case NotifyType.Error:
                            ajaxResult.NotifyMessage("NOTIFY_ERROR", new { messageEntry.Message });
                        break;
                    }
                }
            }
            else
            {
                var messagesZone = workContextAccessor.GetContext(filterContext).Layout.Zones["Messages"];
                foreach (var messageEntry in messageEntries)
                {
                    messagesZone = messagesZone.Add(shapeFactory.Message(messageEntry));
                }    
            }
        }

        public void OnResultExecuted(ResultExecutedContext filterContext)
        {
        }
    }
}