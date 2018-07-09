namespace Otc.Cache
{
    public interface ICacheConfiguration
    {
        CacheType CacheType { get; }

        int CacheDuration { get; set; }

        bool Enabled { get; set; }

        string Aplicacao { get; set; }
    }
}
