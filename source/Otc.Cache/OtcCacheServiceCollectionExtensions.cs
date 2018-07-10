using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Otc.Cache
{
    public static class OtcCacheServiceCollectionExtensions
    {
        public static IServiceCollection AddCacheDistributed(this IServiceCollection services, Action<ApplicationConfigurationLambda> configurationLambda)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            if (configurationLambda == null)
            {
                throw new ArgumentNullException(nameof(configurationLambda));
            }

            // Chama o lambda referente a configuracao da app, o que ira registrar o tipo ApplicationConfiguration
            var applicationConfigurationLambda = new ApplicationConfigurationLambda(services);
            configurationLambda.Invoke(applicationConfigurationLambda);

            return services;
        }
    }

    public class ApplicationConfigurationLambda
    {
        private readonly IServiceCollection services;

        public ApplicationConfigurationLambda(IServiceCollection services)
        {
            this.services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public void Configure(ICacheConfiguration applicationConfiguration)
        {
            if (applicationConfiguration == null)
            {
                throw new ArgumentNullException(nameof(applicationConfiguration));
            }

            if (applicationConfiguration.CacheType == CacheType.Redis)
            {
                RedisCacheConfiguration config = applicationConfiguration as RedisCacheConfiguration;

                services.AddDistributedRedisCache(options =>
                {
                    options.Configuration = config.RedisConnection;
                });
            }
            else if (applicationConfiguration.CacheType == CacheType.Sql)
            {
                SqlCacheConfiguration config = applicationConfiguration as SqlCacheConfiguration;

                services.AddDistributedSqlServerCache(o =>
                {
                    o.ConnectionString = config.SqlConnection;
                    o.SchemaName = config.SchemaName;
                    o.TableName = config.TableName;
                });
            }

            services.AddSingleton(c => new CacheParametros(applicationConfiguration.CacheDuration, applicationConfiguration.Enabled, applicationConfiguration.Aplicacao, applicationConfiguration.CacheType));
            services.AddSingleton(applicationConfiguration);
        }
    }
}