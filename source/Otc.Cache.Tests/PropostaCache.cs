using Microsoft.Extensions.Caching.Distributed;
//using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Otc.Cache.Tests
{
    public class PropostaCache : Cache<PropostaDto>
    {
        public PropostaCache(IDistributedCache distrinutedCache, CacheParametros parametros) : base(distrinutedCache, parametros)
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
