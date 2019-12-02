using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
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
        public async void Test_GetAsync_SetAsync_With_Object()
        {
            var resultExpectedFromCache = await typedCache.GetAsync("Test_GetAsync_SetAsync_With_Object",
                TimeSpan.FromSeconds(30),
                async () =>
                {
                    await Task.Delay(10);
                    return new User()
                    {
                        Id = 1,
                        Name = "Success test"
                    };
                });

            Assert.Equal(resultExpectedFromCache.Id, resultExpectedFromCache.Id);
        }

        [Fact]
        public async void Test_GetAsync_SetAsync_With_LongValue()
        {
            long valueExpected = 123456789;
            var keyCache = "Test_GetAsync_SetAsync_With_LongValue";

            var resultExpectedFromCache = await typedCache.GetAsync(keyCache,
                TimeSpan.FromSeconds(30),
                async () =>
                {
                    await Task.Delay(10);
                    return valueExpected;
                });

            Assert.Equal(valueExpected, resultExpectedFromCache);
            Assert.IsType<long>(resultExpectedFromCache);
        }

        [Fact]
        public async void Test_GetAsync_SetAsync_With_CancellationToken()
        {
            long valueExpected = 123456789;
            var keyCache = "Test_GetAsync_SetAsync_With_CancellationToken";

            var cancellationToken = new CancellationTokenSource();

            cancellationToken.Cancel();

            var resultExpectedFromCache = await typedCache.GetAsync(keyCache,
                TimeSpan.FromSeconds(30),
                async () =>
                {
                    await Task.Delay(10);
                    return valueExpected;
                }, cancellationToken.Token);

            Assert.Equal(valueExpected, resultExpectedFromCache);
            Assert.IsType<long>(resultExpectedFromCache);
        }

        [Fact]
        [Obsolete]
        public void Test_Get()
        {
            var resultFromCache = typedCache.Get<User>("Test_Get");

            Assert.Null(resultFromCache);
        }

        [Fact]
        [Obsolete]
        public void Test_Set()
        {
            typedCache.Set("Test_Set", new User(), TimeSpan.FromSeconds(30));

            var resultFromCache = typedCache.Get<User>("Test_Set");

            Assert.NotNull(resultFromCache);
        }

        [Fact]
        public async void Test_RemoveAsync()
        {
            await typedCache.GetAsync("Test_RemoveAsync", TimeSpan.FromSeconds(30), 
                () => Task.FromResult(new User()));

            await typedCache.RemoveAsync("Test_RemoveAsync");
        }

        [Fact]
        [Obsolete]
        public void Test_Remove()
        {
            typedCache.Set("Test_Remove", new User(), TimeSpan.FromSeconds(30));

            typedCache.Remove("Test_Remove");

            var resultFromCache = typedCache.Get<User>("Test_Remove");

            Assert.Null(resultFromCache);
        }
    }
}
