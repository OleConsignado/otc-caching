# Otc.Cache
[![Build Status](https://travis-ci.org/OleConsignado/otc-cache.svg?branch=master)](https://travis-ci.org/OleConsignado/otc-cache)

Otc.Cache is a simple distributed cache that´s works with Sql Server and Redis.

# Setup
Add nuget package 'Otc.Cache.Abstraction' and 'Otc.Cache' to your project.

> Install-Package 'Otc.Cache.Abstraction'

> Install-Package 'Otc.Cache'

Register and initialize the package in the 'Startup.cs' service collection passing the cache configuration type that you want. You could use RedisCacheConfiguration OR SqlCacheConfiguration.

```cs
            /// Redis Cache Configuration
            services.AddCacheDistributed(app => app.Configure(new RedisCacheConfiguration()
            {
                Aplicacao = "redisCache",
                Enabled = true,
                CacheDuration = 10,
                RedisConnection = "redis.mycomp.com.br:30379"
            }));

            services.AddCacheDistributed(app => app.Configure(new SqlCacheConfiguration()
            {
                Aplicacao = "sqlCache",
                CacheDuration = 10,
                Enabled = true,
                SchemaName = "dbo",
                TableName = "CacheTable",
                SqlConnection = "Data Source=SQLHML,1433;Initial Catalog=MyCache;User Id=u_sqlcache;Password=u_xxx;"
            }));

```

