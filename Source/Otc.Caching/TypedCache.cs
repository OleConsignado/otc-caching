﻿using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Otc.Caching.Abstractions;
using System;
using System.Threading.Tasks;

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

        private string BuildKey(string key) => keyPrefix + key;

        public T Get<T>(string key) where T : class =>
            GetAsync<T>(key).GetAwaiter().GetResult();

        public async Task<T> GetAsync<T>(string key) where T : class
        {
            T result = null;

            if (cacheConfiguration.CacheEnabled)
            {
                var distributedCacheKey = BuildKey(key);

                logger.LogDebug($"{nameof(TypedCache)}.{nameof(GetAsync)}: Reading cache for key '{{DistributedCacheKey}}'.", distributedCacheKey);

                try
                {
                    var cache = await distributedCache.GetStringAsync(distributedCacheKey);

                    if (cache != null)
                    {
                        result = JsonConvert.DeserializeObject<T>(cache);
                    }

                    logger.LogDebug($"{nameof(TypedCache)}.{nameof(GetAsync)}: Cache for key '{{DistributedCacheKey}}' was successfuly read.", distributedCacheKey);
                }
                catch (Exception e)
                {
                    logger.LogWarning(e,
                        $"{nameof(TypedCache)}.{nameof(GetAsync)}: Exception was thrown while reading cache.");
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
                        $"{nameof(TypedCache)}.{nameof(RemoveAsync)}: Exception was thrown while removing cache with key '{{DistributedCacheKey}}'.",
                        distributedCacheKey);
                }
            }
        }

        public void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow) where T : class =>
            SetAsync<T>(key, value, absoluteExpirationRelativeToNow).GetAwaiter().GetResult();

        public async Task SetAsync<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow) where T : class
        {
            if (cacheConfiguration.CacheEnabled)
            {
                var distributedCacheKey = BuildKey(key);

                logger.LogInformation($"{nameof(TypedCache)}.{nameof(SetAsync)}: Creating cache with key '{{DistributedCacheKey}}'", distributedCacheKey);

                try
                {

                    await distributedCache.SetStringAsync(distributedCacheKey, JsonConvert.SerializeObject(value),
                        new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpirationRelativeToNow = absoluteExpirationRelativeToNow
                        });

                    logger.LogInformation($"{nameof(TypedCache)}.{nameof(SetAsync)}: Cache with key {{DistributedCacheKey}} was successful created with absolute expiration set to {{Expiration}}",
                        distributedCacheKey, DateTimeOffset.Now.Add(absoluteExpirationRelativeToNow));
                }
                catch (Exception e)
                {
                    logger.LogWarning(e,
                        $"{nameof(TypedCache)}.{nameof(GetAsync)}: Exception was thrown while writing cache with key '{{DistributedCacheKey}}'.",
                        distributedCacheKey);
                }
            }
        }

        public bool TryGet<T>(string key, out T value) where T : class
        {
            value = GetAsync<T>(key).GetAwaiter().GetResult();

            return value != null ? true : false;
        }

        public async Task<T> GetAsync<T>(string key, TimeSpan absoluteExpirationRelativeToNow, Func<Task<T>> funcAsync) where T : class
        {
            var value = await GetAsync<T>(key);

            if (value == null)
            {
                if (funcAsync != null)
                {
                    value = await funcAsync();

                    await SetAsync<T>(key, value, absoluteExpirationRelativeToNow);
                }
            }

            return value;
        }
    }
}
