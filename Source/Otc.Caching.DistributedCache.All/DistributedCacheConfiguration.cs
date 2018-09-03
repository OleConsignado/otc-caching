namespace Otc.Caching.DistributedCache.All
{
    public class DistributedCacheConfiguration : CacheConfiguration
    {
        public StorageType CacheStorageType { get; set; }
        public string CacheSqlConnectionString { get; set; }
        public string CacheSqlSchemaName { get; set; }
        public string CacheSqlTableName { get; set; }
        public string CacheRedisConfiguration { get; set; }
        public string CacheRedisInstanceName { get; set; }

        /// <summary>
        /// Max size of cache in bytes default: 100 * 1024 * 1024; // 100MB
        /// </summary>
        public long MemorySizeLimit { get; set; } = 100 * 1024 * 1024; // 100MB
    }
}
