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

        public T Get<T>(string key) where T : class
        {
            T result = null;

            if (cacheConfiguration.CacheEnabled)
            {
                var distributedCacheKey = BuildKey(key);

                logger.LogDebug($"{nameof(Get)}: Reading cache for key " +
                    $"'{{DistributedCacheKey}}'.", distributedCacheKey);

                try
                {
                    var cache = distributedCache.GetString(distributedCacheKey);

                    if (cache != null)
                    {
                        result = JsonConvert.DeserializeObject<T>(cache);
                    }

                    logger.LogDebug($"{nameof(Get)}: Cache for key " +
                        $"'{{DistributedCacheKey}}' was successfuly read.", distributedCacheKey);
                }
                catch (Exception e)
                {
                    logger.LogWarning(e,
                        $"{nameof(Get)}: Exception was thrown while " +
                        $"reading cache.");
                }
            }

            return result;
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
            where T : class
        {
            if (cacheConfiguration.CacheEnabled)
            {
                var distributedCacheKey = BuildKey(key);

                logger.LogInformation($"{nameof(Set)}: Creating cache with key '{{DistributedCacheKey}}'",
                    distributedCacheKey);

                try
                {

                    await distributedCache.SetString(distributedCacheKey,
                        JsonConvert.SerializeObject(value),
                        new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
                        });

                    logger.LogInformation($"{nameof(Set)}: Cache with key {{DistributedCacheKey}} was " +
                        "successful created with absolute expiration set to {Expiration}",
                        distributedCacheKey,
                        DateTimeOffset.Now.Add(absoluteExpirationRelativeToNow));
                }
                catch (Exception e)
                {
                    logger.LogWarning(e,
                        $"{nameof(Set)}: Exception was thrown while writing cache with key " +
                        "'{DistributedCacheKey}'.", distributedCacheKey);
                }
            }
        }

        public bool TryGet<T>(string key, out T value) where T : class
        {
            value = Get<T>(key);

            return value != null ? true : false;
        }

        public async Task<T> GetAsync<T>(string key, TimeSpan absoluteExpirationRelativeToNow,
            Func<Task<T>> funcAsync, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentException("message", nameof(key));
            }
            
            if (funcAsync is null)
            {
                throw new ArgumentNullException(nameof(funcAsync));
            }

            T result = default(T);

            var distributedCacheKey = BuildKey(key);

            try
            {
                logger.LogDebug($"{nameof(GetAsync)}: Reading cache for key " +
                        $"'{{DistributedCacheKey}}'.", distributedCacheKey);

                var rawValue = await distributedCache.GetStringAsync(distributedCacheKey, cancellationToken);

                if (rawValue == null)
                {
                    logger.LogDebug($"{nameof(GetAsync)}: Not found any cache with the key " +
                        "'{DistributedCacheKey}'.", distributedCacheKey);

                    result = await funcAsync.Invoke();

                    var serializedValue = JsonConvert.SerializeObject(result);

                    var cacheEntryOptions = new DistributedCacheEntryOptions()
                    {
                        AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
                    };

                    await distributedCache.SetStringAsync(distributedCacheKey,
                        serializedValue, cacheEntryOptions, cancellationToken);

                    logger.LogDebug($"{nameof(GetAsync)}: Cached for key " +
                        $"'{{DistributedCacheKey}}' was successfuly read.", distributedCacheKey);
                }
                else
                {
                    logger.LogDebug($"{nameof(GetAsync)}: Found cache value with the given key " +
                        "'{DistributedCacheKey}'. Trying to deserialize the object.", distributedCacheKey);

                    result = JsonConvert.DeserializeObject<T>(rawValue);
                }
            }
            catch (Exception e)
            {
                logger.LogWarning(e,
                    $"{nameof(GetAsync)}: Exception was thrown while removing cache with key " +
                    "'{DistributedCacheKey}'.", distributedCacheKey);
            }

            return result;
        }
    }
}
