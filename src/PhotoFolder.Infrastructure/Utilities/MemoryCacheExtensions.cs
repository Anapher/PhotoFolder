using System;
using System.Threading;
using Microsoft.Extensions.Caching.Memory;

namespace PhotoFolder.Infrastructure.Utilities
{
    public static class MemoryCacheExtensions
    {
        public static T GetOrSetValueSafe<T>(this IMemoryCache cache, string key, Action<ICacheEntry> configureCacheEntry,
            Func<T> valueFactory)
        {
            if (cache.TryGetValue(key, out Lazy<T> cachedValue))
                return cachedValue.Value;

            cache.GetOrCreate(key, entry =>
            {
                configureCacheEntry(entry);
                return new Lazy<T>(valueFactory, LazyThreadSafetyMode.ExecutionAndPublication);
            });

            return cache.Get<Lazy<T>>(key).Value;
        }
    }
}
