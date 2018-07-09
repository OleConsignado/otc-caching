namespace Otc.Cache
{
    public class RedisCacheConfiguration : ICacheConfiguration
    {
        public string RedisConnection { get; set; }

        public int CacheDuration { get; set; }

        public bool Enabled { get; set; }

        public string Aplicacao { get; set; }

        public CacheType CacheType { get; }

        public RedisCacheConfiguration()
        {
            CacheType = CacheType.Redis;
        }
    }
}