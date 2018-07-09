using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using Xunit;

namespace Otc.Cache.Tests
{
    public class IntegrationTest
    {
        private IServiceProvider serviceProvider;
        private PropostaCache _propostaCache;

        public IntegrationTest()
        {
            var services = new ServiceCollection();

            services.AddCacheDistributed(app => app.Configure(new RedisCacheConfiguration()
            {
                CacheDuration = 10,
                RedisConnection = "k8s01.oleconsignado.com.br:30379"
            }));

            //services.AddCacheDistributed(app => app.Configure(new SqlCacheConfiguration()
            //{
            //    SchemaName = "dbo",
            //    TableName = "CacheTable",
            //    SqlConnection = "Data Source=BBSSQLHML,1433;Initial Catalog=OleCache;User Id=u_olaadmin;Password=u_0l@@dm1n;"
            //}));

            services.AddScoped<PropostaCache>();
            serviceProvider = services.BuildServiceProvider();

        }

        [Fact]
        public void Test1()
        {
            _propostaCache = serviceProvider.GetService<PropostaCache>();

            var propostaDto = new PropostaDto
            {
                Cpf = "012.345.678-90",
                Nome = "Luciano Lima",
                Data = DateTime.Now.ToShortDateString()
            };

            _propostaCache.Set(propostaDto.Cpf, propostaDto.Nome, propostaDto);
        }
    }
}
