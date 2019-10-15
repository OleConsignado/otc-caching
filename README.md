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

## Usage


1. Get the data from cache key 'my-cache-key-async', if does not found nothing, it will cache the value.

```cs
//Trying to get data from cache key 'my-cache-key-async',
//if doest not found nothing, it will be cached.
ITypedCache cache = ... // Get it by dependency injection
var cacheKey = "my-cache-key-async";

var myModelObj = await cache.GetAsync<MyModelClass>(cacheKey, TimeSpan.FromSeconds(30), async () => { 
    myModelObj = ... // retrieve the object from it source here
    return myModelObj;
} ));
```

2. Get the data from cache key 'my-cache-get-key-async'.

```cs
//Trying to get data from cache key 'my-cache-get-key-async'.
ITypedCache cache = ... // Get it by dependency injection
var cacheKey = "my-cache-get-key-async";

var myModelObjFromCache = await cache.GetAsync<MyModelClass>(cacheKey);
```

3. Cache the data with the cache key 'set-cache-async' and getting the data with the given key.
```cs
var cacheKey = "set-cache-async";

await typedCache.SetAsync(cacheKey, new User(), TimeSpan.FromSeconds(30));

var resultFromCache = await typedCache.GetAsync<User>("Test_SetAsync");
```