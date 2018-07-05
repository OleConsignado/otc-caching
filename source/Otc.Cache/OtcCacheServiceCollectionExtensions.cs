using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Otc.Cache
{
    public static class OtcCacheServiceCollectionExtensions
    {
        public static IServiceCollection AddCacheDistributed(this IServiceCollection services, IConfiguration configuration)
        {
            var aplicacao = "Otc.Cache";
            var cacheType = configuration.GetSection("CacheConfig").GetValue<CacheType>("CacheType");

            switch (cacheType)
            {
                case CacheType.Sql:
                    services.AddDistributedSqlServerCache(o =>
                    {
                        o.ConnectionString = configuration.GetSection("ConnectionStrings").GetValue<string>("SQLCONNSTR_CacheContext");
                        o.SchemaName = "dbo";
                        o.TableName = "CacheTable";
                    });
                    break;
                case CacheType.Redis:
                    services.AddDistributedRedisCache(options =>
                    {
                        options.InstanceName = $"{aplicacao}-";
                        options.Configuration = configuration.GetSection("ConnectionStrings").GetValue<string>("REDIS_CacheContext");
                    });
                    break;
            }


            services.AddSingleton
                (
                    c => new CacheParametros(configuration.GetSection("CacheConfig").GetValue<int>("CacheTime"),
                                             configuration.GetSection("CacheConfig").GetValue<bool>("CacheEnable"),
                                             aplicacao, cacheType)
                );

            return services;
        }
    }
}
