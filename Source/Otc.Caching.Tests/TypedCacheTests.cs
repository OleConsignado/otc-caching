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
        public void Get_FromInexistentKey_GetsNull()
        {
            // Arrange & Act
            var key = nameof(Get_FromInexistentKey_GetsNull);
            var resultFromCache = typedCache.Get<User>(key);

            // Assert
            Assert.Null(resultFromCache);
        }

        [Fact]
        public async Task GetAsync_WithNoFunction_GetsNull()
        {
            // Arrange
            var key = nameof(GetAsync_WithNoFunction_GetsNull);
            await typedCache.GetAsync<User>(key, TimeSpan.FromSeconds(20), null);

            // Act
            typedCache.TryGet(key, out User resultFromCache);

            // Assert
            Assert.Null(resultFromCache);
        }

        [Fact]
        public async Task GetAsync_FromInexistentKey_GetsNull()
        {
            // Arrange & Act
            var key = nameof(GetAsync_FromInexistentKey_GetsNull);
            var resultFromCache = await typedCache.GetAsync<User>("Test_GetAsync");

            // Assert
            Assert.Null(resultFromCache);
        }

        [Fact]
        public async Task GetAsync_FromExistentKey_GetObject()
        {
            // Arrange
            var key = nameof(GetAsync_FromExistentKey_GetObject);
            var expectedObject = new User { Id = 1, Name = "User name" };

            await typedCache.GetAsync<User>(
                key,
                TimeSpan.FromMinutes(1),
                () => Task.FromResult(expectedObject));

            // Act
            var user = await typedCache.GetAsync<User>(key);

            // Assert
            Assert.Equal(user.Id, user.Id);
            Assert.Equal(user.Name, user.Name);
        }

        [Fact]
        public async Task GetAsync_SetsAndAwaitExpiringTime_GetsNull()
        {
            // Arrange
            var key = nameof(GetAsync_SetsAndAwaitExpiringTime_GetsNull);
            await typedCache.GetAsync<User>(
                key,
                TimeSpan.FromSeconds(1),
                () => Task.FromResult(new User { Id = 1, Name = "User name" }));

            await Task.Delay(1500);

            // Act
            typedCache.TryGet(key, out User resultFromCache);

            // Assert
            Assert.Null(resultFromCache);
        }

        [Fact]
        public async Task GetAsync_WithMultipleThreads_SingleSetExecution()
        {
            // Arrange & Act
            var key = nameof(GetAsync_WithMultipleThreads_SingleSetExecution);
            var start = 0;
            var end = 10;
            var tasks = new Task[10];

            int timesCalled = 0;

            var functionMock = new Mock<Func<Task<User>>>();
            functionMock.Setup(f => f())
                .Callback(() => 
                {
                    Interlocked.Increment(ref timesCalled); 
                })
                .ReturnsAsync(new User { Id = 0, Name = "User name" })
                .Verifiable();


            for (int i = start; i < end; i++)
            {
                var task = Task.Run(async () => 
                                await typedCache.GetAsync<User>(
                                key,
                                TimeSpan.FromMinutes(1),
                                functionMock.Object));

                tasks[i] = task;
            }

            await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(1, timesCalled);
        }

        [Fact]
        public async Task GetAsync_WithMultipleThreadsAndTwoDifferentKeys_TwiceSetExecution()
        {
            // Arrange & Act
            var key1 = "key1";
            var key2 = "key2";

            var start = 0;
            var end = 10;
            var tasks = new Task[10];

            int timesCalled = 0;

            var functionMock = new Mock<Func<Task<User>>>();
            functionMock.Setup(f => f())
                .Callback(() =>
                {
                    Interlocked.Increment(ref timesCalled);
                })
                .ReturnsAsync(new User { Id = 0, Name = "User name" })
                .Verifiable();

            for (int i = start; i < end; i++)
            {
                var key = i % 2 == 0 ? key1 : key2;

                var task = Task.Run(async () =>
                                await typedCache.GetAsync<User>(
                                key,
                                TimeSpan.FromMinutes(1),
                                functionMock.Object));

                tasks[i] = task;
            }

            await Task.WhenAll(tasks);

            // Assert
            Assert.Equal(2, timesCalled);
        }

        [Fact]
        public async Task GetAsync_TrySetOnStillValidKey_GetOldObject()
        {
            // Arrange
            var key = nameof(GetAsync_TrySetOnStillValidKey_GetOldObject);
            var resultExpectedFromCache = await typedCache.GetAsync(
                key, TimeSpan.FromSeconds(30), 
                () => Task.FromResult(
                new User()
                {
                    Id = 1,
                    Name = "Success test"
                }));

            // Action
            var result = await typedCache.GetAsync<User>(
                key, 
                TimeSpan.FromSeconds(1),
                () => Task.FromResult(
                new User
                {
                    Id = 2,
                    Name = "Test"
                }));

            // Assert
            Assert.Equal(resultExpectedFromCache.Id, result.Id);
        }
               
        [Fact]
        public async Task SetAsync_SetObject_ObjectSetted()
        {
            // Arrange
            var key = nameof(SetAsync_SetObject_ObjectSetted);
            var user = new User { Id = 1, Name = "Test" };

            // Act
            await typedCache.SetAsync(key, user, TimeSpan.FromSeconds(30));
            var resultFromCache = await typedCache.GetAsync<User>(key);

            // Assert
            Assert.Equal(user.Id, resultFromCache.Id);
            Assert.Equal(user.Name, resultFromCache.Name);
        }

        [Fact]
        public void Set_SetObject_ObjectSetted()
        {
            // Arrange
            var key = nameof(Set_SetObject_ObjectSetted);
            var user = new User { Id = 1, Name = "Test" };

            // Act
            typedCache.Set(key, user, TimeSpan.FromSeconds(30));
            var resultFromCache = typedCache.Get<User>(key);

            // Assert
            Assert.Equal(user.Id, resultFromCache.Id);
            Assert.Equal(user.Name, resultFromCache.Name);
        }

        [Fact]
        public async Task RemoveAsync_RemoveBeforeExpiringTime_RemovedObject()
        {
            // Arrange
            var key = nameof(RemoveAsync_RemoveBeforeExpiringTime_RemovedObject);
            await typedCache.SetAsync(key, new User(), TimeSpan.FromSeconds(30));

            // Act
            await typedCache.RemoveAsync(key);
            var resultFromCache = await typedCache.GetAsync<User>(key);

            // Assert
            Assert.Null(resultFromCache);
        }

        [Fact]
        public void Remove_RemoveBeforeExpiringTime_RemovedObject()
        {
            // Arrange
            var key = nameof(Remove_RemoveBeforeExpiringTime_RemovedObject);
            typedCache.Set(key, new User(), TimeSpan.FromSeconds(30));

            // Act
            typedCache.Remove(key);
            var resultFromCache = typedCache.Get<User>(key);

            // Assert
            Assert.Null(resultFromCache);
        }
    }
}
