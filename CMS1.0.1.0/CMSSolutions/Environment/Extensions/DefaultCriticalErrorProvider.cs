using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using CMSSolutions.Localization;

namespace CMSSolutions.Environment.Extensions
{
    public class DefaultCriticalErrorProvider : ICriticalErrorProvider
    {
        private ConcurrentBag<LocalizedString> errorMessages;
        private readonly object synLock = new object();

        public DefaultCriticalErrorProvider()
        {
            errorMessages = new ConcurrentBag<LocalizedString>();
        }

        public IEnumerable<LocalizedString> GetErrors()
        {
            return errorMessages;
        }

        public void RegisterErrorMessage(LocalizedString message)
        {
            if (errorMessages != null && errorMessages.All(m => m.TextHint != message.TextHint))
            {
                errorMessages.Add(message);
            }
        }

        public void Clear()
        {
            lock (synLock)
            {
                errorMessages = new ConcurrentBag<LocalizedString>();
            }
        }
    }
}