using System;
using System.Collections.Concurrent;

namespace CMSSolutions.Caching
{
    public class DefaultCacheHolder : ICacheHolder
    {
        private readonly ICacheContextAccessor cacheContextAccessor;
        private readonly ConcurrentDictionary<CacheKey, object> caches = new ConcurrentDictionary<CacheKey, object>();

        public DefaultCacheHolder(ICacheContextAccessor cacheContextAccessor)
        {
            this.cacheContextAccessor = cacheContextAccessor;
        }

        #region ICacheHolder Members

        public ICache<TKey, TResult> GetCache<TKey, TResult>(Type component)
        {
            var cacheKey = new CacheKey(component, typeof(TKey), typeof(TResult));
            object result = caches.GetOrAdd(cacheKey, k => new Cache<TKey, TResult>(cacheContextAccessor));
            return (Cache<TKey, TResult>)result;
        }

        public void Reset()
        {
            caches.Clear();
        }

        #endregion ICacheHolder Members

        #region Nested type: CacheKey

        private class CacheKey : Tuple<Type, Type, Type>
        {
            public CacheKey(Type component, Type key, Type result)
                : base(component, key, result)
            {

            }
        }

        #endregion Nested type: CacheKey
    }
}