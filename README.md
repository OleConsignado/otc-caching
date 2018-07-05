# Otc.Cache
[![Build Status](https://travis-ci.org/OleConsignado/otc-cache.svg?branch=master)](https://travis-ci.org/OleConsignado/otc-cache)

Otc.Cache is a simple distributed cache that´s works with Sql Server and Redis.

# Setup
Add nuget package 'Otc.Cache.Abstraction' and 'Otc.Cache' to your project.

> Install-Package 'Otc.Cache.Abstraction'

> Install-Package 'Otc.Cache'

Register the package in the 'Startup.cs' service collection passing the IConfiguration instance to the method.
```cs
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCacheDistributed(Configuration);
        }
```

