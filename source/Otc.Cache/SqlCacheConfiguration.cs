namespace Otc.Cache
{
    public class SqlCacheConfiguration : ICacheConfiguration
    {
        public string SqlConnection { get; set; }

        public string SchemaName { get; set; }

        public string TableName { get; set; }

        public int CacheDuration { get; set; }

        public bool Enabled { get; set; }

        public string Aplicacao { get; set; }

        public SqlCacheConfiguration()
        {
        }
    }
}