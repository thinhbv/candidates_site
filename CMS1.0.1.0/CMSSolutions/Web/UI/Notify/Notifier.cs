using System.Collections.Generic;
using Castle.Core.Logging;

namespace CMSSolutions.Web.UI.Notify
{
    public class Notifier : INotifier
    {
        private readonly IList<NotifyEntry> entries;

        public Notifier()
        {
            Logger = NullLogger.Instance;
            entries = new List<NotifyEntry>();
        }

        public ILogger Logger { get; set; }

        public void Add(NotifyType type, string message)
        {
            entries.Add(new NotifyEntry { Type = type, Message = message });
        }

        public IEnumerable<NotifyEntry> List()
        {
            return entries;
        }
    }
}