using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Otc.Cache.Tests
{
    public class PropostaCache : Cache<PropostaDto>
    {
        public PropostaCache(IDistributedCache distrinutedCache, CacheParametros parametros, ILoggerFactory loggerFactory) : base(distrinutedCache, parametros, loggerFactory)
        {
        }

        public void Set(string cpf, string nome, PropostaDto entity)
        {
            Set(GetKeys(cpf, nome), entity);
        }

        protected string GetKeys(string cpf, string nome)
        {
            return $"{cpf.Replace(".", "").Replace("-", "")}-{nome}";
        }

        public bool TryGetValue(string cpf, string nome, out PropostaDto propostaDto)
        {
            var key = GetKeys(cpf, nome);

            return TryGetValue(key, out propostaDto);
        }
    }
}
