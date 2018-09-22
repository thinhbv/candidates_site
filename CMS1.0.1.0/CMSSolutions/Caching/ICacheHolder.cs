using System;

namespace CMSSolutions.Caching
{
    public interface ICacheHolder : ISingletonDependency
    {
        ICache<TKey, TResult> GetCache<TKey, TResult>(Type component);

        void Reset();
    }
}