using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Otc.Caching.Abstractions;
using System;

namespace Otc.Caching
{
    public class TypedCache : ITypedCache
    {
        private readonly IDistributedCache distributedCache;
        private readonly CacheConfiguration cacheConfiguration;
        private readonly ILogger logger;
        private readonly string keyPrefix;

        public TypedCache(IDistributedCache distributedCache, ILoggerFactory loggerFactory, CacheConfiguration cacheConfiguration)
        {
            this.distributedCache = distributedCache ?? throw new ArgumentNullException(nameof(distributedCache));
            this.cacheConfiguration = cacheConfiguration ?? throw new ArgumentNullException(nameof(cacheConfiguration));
            logger = loggerFactory?.CreateLogger<TypedCache>() ?? throw new ArgumentNullException(nameof(loggerFactory));
            keyPrefix = cacheConfiguration.CacheKeyPrefix ?? string.Empty;
        }

        private string BuildKey(string key)
        {
            return keyPrefix + key;
        }

        public T Get<T>(string key) where T : class
        {
            T result = null;

            if (cacheConfiguration.CacheEnabled)
            {
                var distributedCacheKey = BuildKey(key);

                logger.LogDebug($"{nameof(TypedCache)}.{nameof(Get)}: Reading cache for key '{{DistributedCacheKey}}'.", distributedCacheKey);

                try
                {
                    var cache = distributedCache.GetString(distributedCacheKey);

                    if (cache != null)
                    {
                        result = JsonConvert.DeserializeObject<T>(cache);
                    }

                    logger.LogDebug($"{nameof(TypedCache)}.{nameof(Get)}: Cache for key '{{DistributedCacheKey}}' was successfuly read.", distributedCacheKey);
                }
                catch (Exception e)
                {
                    logger.LogWarning(3554, e,
                        $"{nameof(TypedCache)}.{nameof(Get)}: Exception was thrown while reading cache.");
                }
            }

            return result;
        }

        public void Remove(string key)
        {
            if (cacheConfiguration.CacheEnabled)
            {
                var distributedCacheKey = BuildKey(key);

                try
                {
                    distributedCache.Remove(distributedCacheKey);
                }
                catch (Exception e)
                {
                    logger.LogWarning(2311, e,
                        $"{nameof(TypedCache)}.{nameof(Remove)}: Exception was thrown while removing cache with key '{{DistributedCacheKey}}'.", 
                        distributedCacheKey);
                }
            }
        }

        public void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow) where T : class
        {
            if (cacheConfiguration.CacheEnabled)
            {
                var distributedCacheKey = BuildKey(key);

                logger.LogInformation($"{nameof(TypedCache)}.{nameof(Set)}: Creating cache with key '{{DistributedCacheKey}}'", distributedCacheKey);

                try
                {

                    distributedCache.SetString(distributedCacheKey, JsonConvert.SerializeObject(value),
                        new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
                        });

                    logger.LogInformation($"{nameof(TypedCache)}.{nameof(Set)}: Cache with key {{DistributedCacheKey}} was successful created with absolute expiration set to {{Expiration}}",
                        distributedCacheKey, DateTimeOffset.Now.Add(absoluteExpirationRelativeToNow));
                }
                catch (Exception e)
                {
                    logger.LogWarning(9867, e,
                        $"{nameof(TypedCache)}.{nameof(Get)}: Exception was thrown while writing cache with key '{{DistributedCacheKey}}'.",
                        distributedCacheKey);
                }
            }
        }

        public bool TryGet<T>(string key, out T value) where T : class
        {
            value = Get<T>(key);

            return value != null ? true : false;
        }
    }
}
