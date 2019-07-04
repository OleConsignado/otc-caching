# Otc.Caching
[![Build Status](https://travis-ci.org/OleConsignado/otc-caching.svg?branch=master)](https://travis-ci.org/OleConsignado/otc-caching)

Otc.Caching is a simple distributed cache built on top of Microsoft.Extensions.Caching. It provides the convenient `ITypedCache` interface and the ability to switch between Redis, Sql Server and Memory storage engine without needs of recompile it.

## Quickstart

### Installation

Install the [Otc.Caching.DistributedCache.All](https://www.nuget.org/packages/Otc.Caching.DistributedCache.All) package from NuGet.org.

At startup, add `DistributedCache` to your service collection by calling `AddDistributedCacheConfiguration` extension method for `IServiceCollection`:

```cs
services.AddOtcDistributedCache(new DistributedCacheConfiguration(){
    // ... see DistributedCacheConfiguration for details
});

```

### Usage


Async

```cs
ITypedCache cache = ... // Get it by dependency injection
var cacheKey = "my-cache-key-async";

var myModelObj = await cache.CacheManagerAsync<MyModelClass>(cacheKey, TimeSpan.FromSeconds(30), async () => { 
    myModelObj = ... // retrieve the object from it source here
    return myModelObj;
} ));
```

Sync

```cs
ITypedCache cache = ... // Get it by dependency injection
var cacheKey = "my-cache-key-sync";

var myModelObj = cache.CacheManager<MyModelClass>(cacheKey, TimeSpan.FromSeconds(30), () => { 
    myModelObj = ... // retrieve the object from it source here
    return myModelObj;
} ));
```

