namespace Otc.Cache
{
    public interface ICacheConfiguration
    {
        int CacheDuration { get; set; }

        bool Enabled { get; set; }

        string Aplicacao { get; set; }
    }
}
