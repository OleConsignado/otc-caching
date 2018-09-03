using Otc.Caching;
using Otc.Caching.Abstractions;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class OtcCachingServiceCollectionExtensions
    {
        public static IServiceCollection AddOtcCaching(this IServiceCollection services, CacheConfiguration cacheConfiguration = null)
        {
            if (services == null)
            {
                throw new System.ArgumentNullException(nameof(services));
            }

            services.AddSingleton(cacheConfiguration ?? new CacheConfiguration());
            services.AddSingleton<ITypedCache, TypedCache>();

            return services;
        }
    }
}
