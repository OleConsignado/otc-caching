using Otc.Caching.DistributedCache.All;
using System;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class DistributedCacheAllServiceCollectionExtensions
    {
        public static IServiceCollection AddOtcDistributedCache(this IServiceCollection services,
            DistributedCacheConfiguration distributedCacheConfiguration)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (distributedCacheConfiguration == null)
            {
                throw new ArgumentNullException(nameof(distributedCacheConfiguration));
            }

            services.AddOtcCaching(distributedCacheConfiguration);

            if (distributedCacheConfiguration.CacheEnabled)
            {
                switch (distributedCacheConfiguration.CacheStorageType)
                {
                    case StorageType.SqlServer:
                        AddDistributedSqlServerCache(services, distributedCacheConfiguration);

                        break;
                    case StorageType.Redis:
                        AddDistributedRedisCache(services, distributedCacheConfiguration);

                        break;
                    case StorageType.Memory:
                        AddDistributedMemoryCache(services, distributedCacheConfiguration);

                        break;
                    default:
                        throw new InvalidOperationException("Could not determine caching storage type," +
                            " please verify StorageType configuration property.");
                }
            }

            return services;
        }

        private static void AddDistributedSqlServerCache(IServiceCollection services, 
            DistributedCacheConfiguration distributedCacheConfiguration)
        {
            if (string.IsNullOrEmpty(distributedCacheConfiguration.CacheSqlConnectionString))
            {
                throw new InvalidOperationException(
                    $"'{nameof(DistributedCacheConfiguration.CacheSqlConnectionString)}' configuration " +
                    $"is required for 'StorageType.SqlServer'.");
            }

            if (string.IsNullOrEmpty(distributedCacheConfiguration.CacheSqlSchemaName))
            {
                throw new InvalidOperationException(
                    $"'{nameof(DistributedCacheConfiguration.CacheSqlSchemaName)}' configuration " +
                    $"is required for 'StorageType.SqlServer'.");
            }

            if (string.IsNullOrEmpty(distributedCacheConfiguration.CacheSqlTableName))
            {
                throw new InvalidOperationException(
                    $"'{nameof(DistributedCacheConfiguration.CacheSqlTableName)}' configuration " +
                    $"is required for 'StorageType.SqlServer'.");
            }

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = distributedCacheConfiguration.CacheSqlConnectionString;
                options.SchemaName = distributedCacheConfiguration.CacheSqlSchemaName;
                options.TableName = distributedCacheConfiguration.CacheSqlTableName;
            });
        }

        private static void AddDistributedRedisCache(IServiceCollection services,
            DistributedCacheConfiguration distributedCacheConfiguration)
        {
            if (string.IsNullOrEmpty(distributedCacheConfiguration.CacheRedisInstanceName))
            {
                throw new InvalidOperationException(
                    $"'{nameof(DistributedCacheConfiguration.CacheRedisInstanceName)}' configuration " +
                    $"is required for 'StorageType.Redis'.");
            }

            if (string.IsNullOrEmpty(distributedCacheConfiguration.CacheRedisConfiguration))
            {
                throw new InvalidOperationException(
                    $"'{nameof(DistributedCacheConfiguration.CacheRedisConfiguration)}' configuration " +
                    $"is required for 'StorageType.Redis'.");
            }

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = distributedCacheConfiguration.CacheRedisConfiguration;
                options.InstanceName = distributedCacheConfiguration.CacheRedisInstanceName;
            });

        }

        private static void AddDistributedMemoryCache(IServiceCollection services,
            DistributedCacheConfiguration distributedCacheConfiguration)
        {
            services.AddDistributedMemoryCache(options =>
            {
                options.SizeLimit = distributedCacheConfiguration.MemorySizeLimit;
            });
        }
    }
}
