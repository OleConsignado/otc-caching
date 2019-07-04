using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Otc.Caching.Abstractions;
using Otc.Caching.DistributedCache.All;
using System;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace Otc.Caching.Tests
{
    public class TypedCacheTests
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly CacheConfiguration _cacheConfiguration;
        private readonly ServiceProvider _serviceProvider;
        private readonly IDistributedCache _distributedCache;
        private readonly ITypedCache _typedCache;

        public TypedCacheTests()
        {
            _loggerFactory = new LoggerFactory();
            _cacheConfiguration = new CacheConfiguration()
            {
                CacheEnabled = true,
                CacheKeyPrefix = "tst"
            };

            var services = new ServiceCollection();
            services.AddOtcDistributedCache(new DistributedCacheConfiguration()
            {
                CacheStorageType = StorageType.Memory
            });

            _serviceProvider = services.BuildServiceProvider();
            _distributedCache = _serviceProvider.GetService<IDistributedCache>();
            _typedCache = new TypedCache(_distributedCache, _loggerFactory, _cacheConfiguration);
        }

        [Fact]
        public async void Test_CacheManagerAsync_SetAsync()
        {
            var resultFromCache = await _typedCache.CacheManagerAsync("success", TimeSpan.FromSeconds(30), async () => new User()
            {
                Id = 1,
                Name = "Success test"
            });

            resultFromCache = await _typedCache.CacheManagerAsync<User>("success", TimeSpan.FromSeconds(1), null);
            
            Assert.True(resultFromCache.Id == 1);
        }

        [Fact]
        public void Test_CacheManager_Set()
        {
            var resultFromCache = _typedCache.CacheManager("success", TimeSpan.FromSeconds(30), () => new User()
            {
                Id = 1,
                Name = "Success test"
            });

            resultFromCache = _typedCache.CacheManager<User>("success", TimeSpan.FromSeconds(1), null);

            Assert.True(resultFromCache.Id == 1);
        }

        [Fact]
        public async void Test_CacheManagerAsync_GetAsync_ExpiresCache()
        {
            await _typedCache.CacheManagerAsync<User>("Test_CacheManagerAsync_GetAsync_ExpiresCache", TimeSpan.FromSeconds(1), null);

            await Task.Delay(1500);

            _typedCache.TryGet("Test_CacheManagerAsync_GetAsync_ExpiresCache", out User resultFromCache);

            Assert.Null(resultFromCache);
        }

        [Fact]
        public void Test_CacheManager_Get_ExpiresCache()
        {
            _typedCache.CacheManager<User>("Test_CacheManager_Get_ExpiresCache", TimeSpan.FromSeconds(1), null);

            Thread.Sleep(1500);

            _typedCache.TryGet("Test_CacheManager_Get_ExpiresCache", out User resultFromCache);

            Assert.Null(resultFromCache);
        }

        [Fact]
        public async void Test_CacheManagerAsync_GetAsync()
        {
            await _typedCache.CacheManagerAsync<User>("Test_CacheManagerAsync_GetAsync", TimeSpan.FromSeconds(1), null);

            _typedCache.TryGet("Test_CacheManagerAsync_GetAsync", out User resultFromCache);

            Assert.Null(resultFromCache);
        }

        [Fact]
        public void Test_CacheManager_Get()
        {
            _typedCache.CacheManager<User>("Test_CacheManager_Get", TimeSpan.FromSeconds(30), () => new User());

            _typedCache.TryGet("Test_CacheManager_Get", out User resultFromCache);

            Assert.NotNull(resultFromCache);
        }

        [Fact]
        public async void Test_GetAsync()
        {
            var resultFromCache = await _typedCache.GetAsync<User>("Test_GetAsync");

            Assert.Null(resultFromCache);
        }

        [Fact]
        public void Test_Get()
        {
            var resultFromCache = _typedCache.Get<User>("Test_Get");

            Assert.Null(resultFromCache);
        }

        [Fact]
        public async void Test_SetAsync()
        {
            await _typedCache.SetAsync("Test_SetAsync", new User(), TimeSpan.FromSeconds(30));

            var resultFromCache = await _typedCache.GetAsync<User>("Test_SetAsync");

            Assert.NotNull(resultFromCache);
        }

        [Fact]
        public void Test_Set()
        {
            _typedCache.Set("Test_Set", new User(), TimeSpan.FromSeconds(30));

            var resultFromCache = _typedCache.Get<User>("Test_Set");

            Assert.NotNull(resultFromCache);
        }

        [Fact]
        public async void Test_RemoveAsync()
        {
            await _typedCache.SetAsync("Test_RemoveAsync", new User(), TimeSpan.FromSeconds(30));

            await _typedCache.RemoveAsync("Test_RemoveAsync");

            var resultFromCache = await _typedCache.GetAsync<User>("Test_RemoveAsync");

            Assert.Null(resultFromCache);
        }

        [Fact]
        public void Test_Remove()
        {
            _typedCache.Set("Test_Remove", new User(), TimeSpan.FromSeconds(30));

            _typedCache.Remove("Test_Remove");

            var resultFromCache = _typedCache.Get<User>("Test_Remove");

            Assert.Null(resultFromCache);
        }
    }
}
