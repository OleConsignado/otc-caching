namespace Otc.Cache
{
    public class CacheParametros
    {
        public int Time { get; private set; }

        public bool Enabled { get; private set; }

        public string Aplicacao { get; private set; }

        public CacheParametros(int time, bool enabled, string aplicacao)
        {
            Time = time;
            Enabled = enabled;
            Aplicacao = aplicacao;
        }
    }
}