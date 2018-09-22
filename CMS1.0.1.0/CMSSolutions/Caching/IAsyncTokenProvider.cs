using System;

namespace CMSSolutions.Caching
{
    public interface IAsyncTokenProvider
    {
        IVolatileToken GetToken(Action<Action<IVolatileToken>> task);
    }
}