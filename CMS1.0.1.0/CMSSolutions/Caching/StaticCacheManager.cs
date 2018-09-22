using System;

namespace CMSSolutions.Caching
{
    public class StaticCacheManager : IStaticCacheManager
    {
        private static readonly Type component = typeof(StaticCacheManager);
        private readonly ICacheHolder cacheHolder;

        /// <summary>
        /// Constructs a new cache manager for a given component type and with a specific cache holder implementation.
        /// </summary>
        /// <param name="cacheHolder">The cache holder that contains the entities cached.</param>
        public StaticCacheManager(ICacheHolder cacheHolder)
        {
            this.cacheHolder = cacheHolder;
        }

        public TResult Get<TKey, TResult>(TKey key, Func<AcquireContext<TKey>, TResult> acquire)
        {
            return GetCache<TKey, TResult>().Get(key, acquire);
        }

        public ICache<TKey, TResult> GetCache<TKey, TResult>()
        {
            return cacheHolder.GetCache<TKey, TResult>(component);
        }

        public void Reset()
        {
            cacheHolder.Reset();
        }
    }
}