namespace Otc.Cache
{
    public class CacheParametros
    {
        public int Time { get; private set; }

        public bool Enabled { get; private set; }

        public string Aplicacao { get; private set; }

        public CacheType CacheType { get; private set; }

        public CacheParametros(int time, bool enabled, string aplicacao, CacheType cacheType)
        {
            Time = time;
            Enabled = enabled;
            Aplicacao = aplicacao;
            CacheType = cacheType;
        }

        public void SetEnabled(bool enabled)
        {
            Enabled = enabled;
        }
    }
}