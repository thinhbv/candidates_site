using System;

namespace CMSSolutions.Caching
{
    public class DefaultCacheManager : ICacheManager
    {
        private readonly Type component;
        private readonly ICacheHolder cacheHolder;

        public DefaultCacheManager(Type component, ICacheHolder cacheHolder)
        {
            this.component = component;
            this.cacheHolder = cacheHolder;
        }

        public ICache<TKey, TResult> GetCache<TKey, TResult>()
        {
            return cacheHolder.GetCache<TKey, TResult>(component);
        }

        public TResult Get<TKey, TResult>(TKey key, Func<AcquireContext<TKey>, TResult> acquire)
        {
            return GetCache<TKey, TResult>().Get(key, acquire);
        }

        public void Reset()
        {
            cacheHolder.Reset();
        }
    }
}