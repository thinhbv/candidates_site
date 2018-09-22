using System.Collections.Generic;

namespace CMSSolutions.ContentManagement.Messages
{
    public interface IMessageTemplatesProvider : IMessageTokensProvider
    {
        IEnumerable<MessageTemplate> GetTemplates();
    }
}