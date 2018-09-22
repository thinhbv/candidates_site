using System.Collections.Generic;

namespace CMSSolutions.ContentManagement.Messages
{
    public interface IMessageTokensProvider : IDependency
    {
        IEnumerable<string> GetAvailableTokens(string template);

        void GetTokens(string template, WorkContext workContext, IList<Token> tokens);
    }
}