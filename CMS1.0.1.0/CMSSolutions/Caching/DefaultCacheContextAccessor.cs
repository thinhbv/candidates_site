using System;

namespace CMSSolutions.Caching
{
    public class DefaultCacheContextAccessor : ICacheContextAccessor
    {
        [ThreadStatic]
        private static IAcquireContext threadInstance;

        public static IAcquireContext ThreadInstance
        {
            get { return threadInstance; }
            set { threadInstance = value; }
        }

        public IAcquireContext Current {
            get { return ThreadInstance; }
            set { ThreadInstance = value; }
        }
    }
}