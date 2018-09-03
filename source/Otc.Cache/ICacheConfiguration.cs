using System;

namespace Otc.Cache
{
    [Obsolete("This package is obsolete, use Otc.Caching instead.")]
    public interface ICacheConfiguration
    {
        int CacheDuration { get; set; }

        bool Enabled { get; set; }

        string Aplicacao { get; set; }
    }
}
