using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Otc.Caching.Abstractions;
using Otc.Caching.DistributedCache.All;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Otc.Caching.Tests
{
    public class TypedCacheTests
    {
        private readonly ITypedCache typedCache;

        public TypedCacheTests()
        {
            var loggerFactory = new LoggerFactory();
            var cacheConfiguration = new CacheConfiguration()
            {
                CacheEnabled = true,
                CacheKeyPrefix = "tst"
            };

            var services = new ServiceCollection();
            services.AddOtcDistributedCache(new DistributedCacheConfiguration()
            {
                CacheStorageType = StorageType.Memory
            });

            var serviceProvider = services.BuildServiceProvider();
            var distributedCache = serviceProvider.GetService<IDistributedCache>();
            typedCache = new TypedCache(distributedCache, loggerFactory, cacheConfiguration);
        }

        [Fact]
        public async void Test_GetAsync_SetAsync()
        {
            var resultExpectedFromCache = await typedCache.GetAsync("success",
                TimeSpan.FromSeconds(30), async () =>
                {
                    await Task.Delay(10);
                    return new User()
                    {
                        Id = 1,
                        Name = "Success test"
                    };
                });

            resultExpectedFromCache = await typedCache.GetAsync<User>("success",
                TimeSpan.FromSeconds(1), null);

            Assert.Equal(resultExpectedFromCache.Id, resultExpectedFromCache.Id);
        }

        [Fact]
        public async void Test_GetAsync_SetAsync_With_LongValue()
        {
            long value = 123456789;

            var resultExpectedFromCache = await typedCache.GetAsync("success",
                TimeSpan.FromSeconds(30), async () =>
                {
                    await Task.Delay(10);
                    return value;
                });

            resultExpectedFromCache = await typedCache.GetAsync<long>("success",
                TimeSpan.FromSeconds(1), null);

            Assert.Equal(resultExpectedFromCache, resultExpectedFromCache);
            Assert.IsType<long>(resultExpectedFromCache);
        }

        [Fact]
        [Obsolete]
        public async void Test_GetAsync_ExpiresCache()
        {
            await typedCache.GetAsync<User>("Test_GetAsync_ExpiresCache",
                TimeSpan.FromSeconds(1), null);

            await Task.Delay(1500);

            typedCache.TryGet("Test_GetAsync_ExpiresCache",
                out User resultFromCache);

            Assert.Null(resultFromCache);
        }

        [Fact]
        [Obsolete]
        public async void Test_GetAsync_TryGet()
        {
            await typedCache.GetAsync<User>("Test_GetAsync",
                TimeSpan.FromSeconds(1), null);

            typedCache.TryGet("Test_GetAsync", out User resultFromCache);

            Assert.Null(resultFromCache);
        }

        [Fact]
        public async void Test_GetAsync()
        {
            var resultFromCache = await typedCache.GetAsync<User>("Test_GetAsync");

            Assert.Null(resultFromCache);
        }

        [Fact]
        public void Test_Get()
        {
            var resultFromCache = typedCache.Get<User>("Test_Get");

            Assert.Null(resultFromCache);
        }

        [Fact]
        public async void Test_SetAsync()
        {
            await typedCache.SetAsync("Test_SetAsync", new User(), TimeSpan.FromSeconds(30));

            var resultFromCache = await typedCache.GetAsync<User>("Test_SetAsync");

            Assert.NotNull(resultFromCache);
        }

        [Fact]
        public async void Test_SetAsync_With_Value_Type()
        {
            int value = 12345;

            await typedCache.SetAsync("Test_SetAsync", value, TimeSpan.FromSeconds(30));

            var resultFromCache = await typedCache.GetAsync<int>("Test_SetAsync");

            Assert.Equal(12345, resultFromCache);
            Assert.IsType<int>(resultFromCache);
        }

        [Fact]
        public void Test_Set()
        {
            typedCache.Set("Test_Set", new User(), TimeSpan.FromSeconds(30));

            var resultFromCache = typedCache.Get<User>("Test_Set");

            Assert.NotNull(resultFromCache);
        }

        [Fact]
        public async void Test_RemoveAsync()
        {
            await typedCache.SetAsync("Test_RemoveAsync", new User(), TimeSpan.FromSeconds(30));

            await typedCache.RemoveAsync("Test_RemoveAsync");

            var resultFromCache = await typedCache.GetAsync<User>("Test_RemoveAsync");

            Assert.Null(resultFromCache);
        }

        [Fact]
        public void Test_Remove()
        {
            typedCache.Set("Test_Remove", new User(), TimeSpan.FromSeconds(30));

            typedCache.Remove("Test_Remove");

            var resultFromCache = typedCache.Get<User>("Test_Remove");

            Assert.Null(resultFromCache);
        }
    }
}
