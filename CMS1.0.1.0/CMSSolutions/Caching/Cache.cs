using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace CMSSolutions.Caching
{
    public class Cache<TKey, TResult> : ICache<TKey, TResult> {
        private readonly ICacheContextAccessor cacheContextAccessor;
        private readonly ConcurrentDictionary<TKey, CacheEntry> entries;

        public Cache(ICacheContextAccessor cacheContextAccessor) {
            this.cacheContextAccessor = cacheContextAccessor;
            entries = new ConcurrentDictionary<TKey, CacheEntry>();
        }

        public TResult Get(TKey key, Func<AcquireContext<TKey>, TResult> acquire) {
            var entry = entries.AddOrUpdate(key,
                // "Add" lambda
                k => AddEntry(k, acquire),
                // "Update" lamdba
                (k, currentEntry) => UpdateEntry(currentEntry, k, acquire));

            return entry.Result;
        }

        private CacheEntry AddEntry(TKey k, Func<AcquireContext<TKey>, TResult> acquire) {
            var entry = CreateEntry(k, acquire);
            PropagateTokens(entry);
            return entry;
        }

        private CacheEntry UpdateEntry(CacheEntry currentEntry, TKey k, Func<AcquireContext<TKey>, TResult> acquire) {
            var entry = (currentEntry.Tokens.Any(t => t != null && !t.IsCurrent)) ? CreateEntry(k, acquire) : currentEntry;
            PropagateTokens(entry);
            return entry;
        }

        private void PropagateTokens(CacheEntry entry) {
            // Bubble up volatile tokens to parent context
            if (cacheContextAccessor.Current != null) {
                foreach (var token in entry.Tokens)
                    cacheContextAccessor.Current.Monitor(token);
            }
        }


        private CacheEntry CreateEntry(TKey k, Func<AcquireContext<TKey>, TResult> acquire) {
            var entry = new CacheEntry();
            var context = new AcquireContext<TKey>(k, entry.AddToken);

            IAcquireContext parentContext = null;
            try {
                // Push context
                parentContext = cacheContextAccessor.Current;
                cacheContextAccessor.Current = context;

                entry.Result = acquire(context);
            }
            finally {
                // Pop context
                cacheContextAccessor.Current = parentContext;
            }
            entry.CompactTokens();
            return entry;
        }

        private class CacheEntry {
            private IList<IVolatileToken> tokens;
            public TResult Result { get; set; }

            public IEnumerable<IVolatileToken> Tokens {
                get {
                    return tokens ?? Enumerable.Empty<IVolatileToken>();
                }
            }

            public void AddToken(IVolatileToken volatileToken) {
                if (tokens == null) {
                    tokens = new List<IVolatileToken>();
                }

                tokens.Add(volatileToken);
            }

            public void CompactTokens() {
                if (tokens != null)
                    tokens = tokens.Distinct().ToArray();
            }
        }
    }
}
