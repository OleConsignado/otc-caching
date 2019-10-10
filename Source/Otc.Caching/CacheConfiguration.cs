using System.Reflection;

namespace Otc.Caching
{
    public class CacheConfiguration
    {
        /// <summary>
        /// Prefix for every cache entry. Default value is entry assembly name;
        /// </summary>
        public string CacheKeyPrefix { get; set; } = 
            $"{Assembly.GetEntryAssembly()?.GetName()?.Name ?? string.Empty}_";
        /// <summary>
        /// Enable/Disable cache. Default True.
        /// </summary>
        public bool CacheEnabled { get; set; } = true;
    }
}
