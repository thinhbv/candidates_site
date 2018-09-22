using Microsoft.Practices.EnterpriseLibrary.Caching;
using Microsoft.Practices.EnterpriseLibrary.Caching.Expirations;

namespace CMSSolutions.Caching
{
    public class CacheManager : ICacheInfo
    {
        private Microsoft.Practices.EnterpriseLibrary.Caching.ICacheManager manager;   
        public CacheManager()
        {
            manager = CacheFactory.GetCacheManager(Constants.CacheInstance.InstanceName);
        }

        public void Add(string key, object item)
        {
            manager.Add(key.ToUpper(), item, CacheItemPriority.High, null, new NeverExpired());
        }

        public void Remove(string key)
        {
            manager.Remove(key.ToUpper());
        }

        public void RemoveAll()
        {
            manager.Flush();
        }

        public object Get(string key)
        {
            return manager.GetData(key.ToUpper());
        }
    }
}
