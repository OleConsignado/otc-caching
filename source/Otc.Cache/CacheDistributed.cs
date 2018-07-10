using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Otc.Cache.Abstractions;
using System;

namespace Otc.Cache
{
    public class CacheDistributed : ICache
    {
        private readonly IDistributedCache _distrinutedCache;
        private readonly ILogger _logger;
        public const int CacheErrorEventId = 23332;
        public CacheDistributed(IDistributedCache distrinutedCache, ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null)
            {
                throw new ArgumentNullException(nameof(loggerFactory));
            }

            _distrinutedCache = distrinutedCache;
            _logger = loggerFactory.CreateLogger<CacheDistributed>();
        }

        public void Remove(string key)
        {
            _distrinutedCache.Remove(key);
        }

        public void Set<T>(string key, T entity, int minutes)
             where T : class
        {
            string value = JsonConvert.SerializeObject(entity, new JsonSerializerSettings() { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpiration = DateTime.Now.AddMinutes(minutes)
            };

            try
            {
                _distrinutedCache.SetString(key, value, options);
            }
            catch (Exception e)
            {
                _logger.LogCritical(CacheErrorEventId, e, "Erro ao gravar no cache.");
            }
        }

        public bool TryGetValue<T>(string key, out T entity)
            where T : class
        {
            string value;

            try
            {
                value = _distrinutedCache.GetString(key);
            }
            catch (Exception e)
            {
                _logger.LogCritical(CacheErrorEventId, e, "Erro ao acessar o cache.");
                entity = null;
                return false;
            }

            if (value == null)
            {
                entity = null;
                return false;
            }

            entity = JsonConvert.DeserializeObject<T>(value);
            return true;
        }
    }
}