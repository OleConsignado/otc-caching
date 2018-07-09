using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Otc.Cache.Abstractions;

namespace Otc.Cache
{
    public abstract class Cache<T> where T : class
    {
        private readonly CacheParametros _parametros;
        private readonly ICache _cache;

        public const int CacheErrorEventId = 23332;

        public Cache(IDistributedCache distrinutedCache, CacheParametros parametros)
        {
            _parametros = parametros;

            _cache = new CacheDistributed(distrinutedCache);
        }

        private string GetAlias() => GetType().FullName;

        protected virtual int GetTimeExpiration()
        {
            return _parametros.Time;
        }

        protected virtual bool GetCacheEnabled()
        {
            return _parametros.Enabled;
        }

        protected bool TryGetValue(string key, out T entity)
        {
            if (!_parametros.Enabled)
            {
                entity = null;
                return false;
            }

            return _cache.TryGetValue<T>(GetKey(key), out entity);
        }

        protected void Set(string key, T entity)
        {
            if (_parametros.Enabled)
            {
                _cache.Set<T>(GetKey(key), entity, GetTimeExpiration());
            }
        }


        protected void Remove(string key)
        {
            if (_parametros.Enabled)
            {
                _cache.Remove(GetKey(key));
            }
        }

        protected string GetKey(string key)
        {
            var cachekey = $"{GetAlias()}-{key}";

            return cachekey;
        }
    }
}
