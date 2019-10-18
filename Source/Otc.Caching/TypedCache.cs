using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Otc.Caching.Abstractions;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Otc.Caching
{
    public class TypedCache : ITypedCache
    {
        private readonly IDistributedCache distributedCache;
        private readonly CacheConfiguration cacheConfiguration;
        private readonly ILogger logger;
        private readonly string keyPrefix;

        public TypedCache(IDistributedCache distributedCache, ILoggerFactory loggerFactory,
            CacheConfiguration cacheConfiguration)
        {
            this.distributedCache = distributedCache ??
                throw new ArgumentNullException(nameof(distributedCache));
            this.cacheConfiguration = cacheConfiguration ??
                throw new ArgumentNullException(nameof(cacheConfiguration));
            logger = loggerFactory?.CreateLogger<TypedCache>() ??
                throw new ArgumentNullException(nameof(loggerFactory));
            keyPrefix = cacheConfiguration.CacheKeyPrefix ?? string.Empty;
        }

        private string BuildKey(string key) => keyPrefix + key;

        public T Get<T>(string key) where T : class =>
            GetAsync<T>(key).GetAwaiter().GetResult();

        public async Task<T> GetAsync<T>(string key)
        {
            if (cacheConfiguration.CacheEnabled)
            {
                var distributedCacheKey = BuildKey(key);

                logger.LogDebug($"{nameof(GetAsync)}: Reading cache for key " +
                    "'{DistributedCacheKey}'.", distributedCacheKey);

                try
                {
                    var cache = await distributedCache.GetStringAsync(distributedCacheKey);

                    if (cache != null)
                    {
                        var value = JsonConvert.DeserializeObject<T>(cache);

                        return ConvertValueToGenericType(value);

                    }

                    logger.LogDebug($"{nameof(GetAsync)}: Cache for key '{{DistributedCacheKey}}' " +
                        "was successfuly read.", distributedCacheKey);
                }
                catch (Exception e)
                {
                    logger.LogWarning(e, $"{nameof(GetAsync)}: Exception was thrown while reading cache.");
                }
            }

            return default;
        }

        public void Remove(string key) =>
            RemoveAsync(key).GetAwaiter().GetResult();

        public async Task RemoveAsync(string key)
        {
            if (cacheConfiguration.CacheEnabled)
            {
                var distributedCacheKey = BuildKey(key);

                try
                {
                    await distributedCache.RemoveAsync(distributedCacheKey);
                }
                catch (Exception e)
                {
                    logger.LogWarning(e,
                        $"{nameof(RemoveAsync)}: Exception was thrown while removing cache with key " +
                        "'{DistributedCacheKey}'.", distributedCacheKey);
                }
            }
        }

        public void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow) 
            where T : class => 
            SetAsync<T>(key, value, absoluteExpirationRelativeToNow).GetAwaiter().GetResult();

        public async Task SetAsync<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow)
        {
            if (cacheConfiguration.CacheEnabled)
            {
                var distributedCacheKey = BuildKey(key);

                logger.LogInformation($"{nameof(SetAsync)}: Creating cache with key '{{DistributedCacheKey}}'",
                    distributedCacheKey);

                try
                {

                    await distributedCache.SetStringAsync(distributedCacheKey,
                        JsonConvert.SerializeObject(value),
                        new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
                        });

                    logger.LogInformation($"{nameof(SetAsync)}: Cache with key {{DistributedCacheKey}} was " +
                        "successful created with absolute expiration set to {Expiration}",
                        distributedCacheKey,
                        DateTimeOffset.Now.Add(absoluteExpirationRelativeToNow));
                }
                catch (Exception e)
                {
                    logger.LogWarning(e,
                        $"{nameof(SetAsync)}: Exception was thrown while writing cache with key " +
                        "'{DistributedCacheKey}'.", distributedCacheKey);
                }
            }
        }

        public bool TryGet<T>(string key, out T value) where T : class
        {
            value = GetAsync<T>(key).GetAwaiter().GetResult();

            return value != null ? true : false;
        }

        public async Task<T> GetAsync<T>(string key, TimeSpan absoluteExpirationRelativeToNow,
            Func<Task<T>> funcAsync)
        {
            var value = await GetAsync<T>(key);
            
            if (value == null || value.Equals(default(T)))
            {
                if (funcAsync != null)
                {
                    value = await funcAsync();

                    await SetAsync(key, value, absoluteExpirationRelativeToNow);
                }
            }

            return value;
        }

        private T ConvertValueToGenericType<T>(T value)
        {
            var convertToType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);

            return (T)Convert.ChangeType(value, convertToType, CultureInfo.InvariantCulture);
        }
    }
}
