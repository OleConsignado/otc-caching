namespace Otc.Cache
{
    public class RedisCacheConfiguration : ICacheConfiguration
    {
        /// <summary>
        /// Redis connection string (e.g: http://redis.mycompany.com:30379)
        /// </summary>
        public string RedisConnection { get; set; }

        /// <summary>
        /// Time the object will be persisted in cache (in minutes)
        /// </summary>
        public int CacheDuration { get; set; }

        /// <summary>
        /// Enable or disable cache configuration
        /// </summary>
        public bool Enabled { get; set; }

        /// <summary>
        /// The name os the application that use the Cache abstraction
        /// </summary>
        public string Aplicacao { get; set; }

        public RedisCacheConfiguration()
        {
        }
    }
}