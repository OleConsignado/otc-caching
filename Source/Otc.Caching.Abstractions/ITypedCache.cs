using System;
using System.Threading.Tasks;

namespace Otc.Caching.Abstractions
{
    /// <summary>
    /// TypedCache Api
    /// </summary>
    public interface ITypedCache
    {
        /// <summary>
        /// Async - Get an object from cache for the provided key.
        /// <para>
        /// IMPORTANT: This operation should be exception free, if an error occurs while performing it, the exception should be logged as warning.
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>The object read from cache or null if doesn't exists.</returns>
        Task<T> GetAsync<T>(string key) where T : class;

        /// <summary>
        /// Get an object from cache for the provided key.
        /// <para>
        /// IMPORTANT: This operation should be exception free, if an error occurs while performing it, the exception should be logged as warning.
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns>The object read from cache or null if doesn't exists.</returns>
        T Get<T>(string key) where T : class;

        /// <summary>
        /// Convenient way to get/test if exists.
        /// <para>
        /// IMPORTANT: This operation should be exception free, if an error occurs while performing it, the execption should be logged as warning.
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <returns>True if the cache exists; Otherwise False</returns>
        bool TryGet<T>(string key, out T value) where T : class;

        /// <summary>
        /// Async - Set an object to cache with the given absoluteExpirationRelativeToNow duration.
        /// <para>
        /// IMPORTANT: This operation should be exception free, if an error occurs while performing it, the exception should be logged as warning.
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpirationRelativeToNow"></param>
        Task SetAsync<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow) where T : class;

        /// <summary>
        /// Set an object to cache with the given absoluteExpirationRelativeToNow duration.
        /// <para>
        /// IMPORTANT: This operation should be exception free, if an error occurs while performing it, the exception should be logged as warning.
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="absoluteExpirationRelativeToNow"></param>
        void Set<T>(string key, T value, TimeSpan absoluteExpirationRelativeToNow) where T : class;

        /// <summary>
        /// Async - Remove the item with the provided key from cache.]
        /// <para>
        /// IMPORTANT: This operation should be exception free, if an error occurs while performing it, the exception should be logged as warning.
        /// </para>
        /// </summary>
        /// <param name="key"></param>
        Task RemoveAsync(string key);

        /// <summary>
        /// Remove the item with the provided key from cache.]
        /// <para>
        /// IMPORTANT: This operation should be exception free, if an error occurs while performing it, the exception should be logged as warning.
        /// </para>
        /// </summary>
        /// <param name="key"></param>
        void Remove(string key);

        /// <summary>
        /// Async - Convenient way to get/add cache provided by key .
        /// <para>
        /// IMPORTANT: This operation should be exception free, if an error occurs while performing it, the exception should be logged as warning.
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="absoluteExpirationRelativeToNow"></param>
        /// <param name="funcAsync"></param>
        /// <returns>True if the cache exists; Otherwise False</returns>
        Task<T> CacheManagerAsync<T>(string key, TimeSpan absoluteExpirationRelativeToNow, Func<Task<T>> funcAsync) where T : class;

        /// <summary>
        /// Convenient way to get/add cache provided by key.
        /// <para>
        /// IMPORTANT: This operation should be exception free, if an error occurs while performing it, the exception should be logged as warning.
        /// </para>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <param name="absoluteExpirationRelativeToNow"></param>
        /// <param name="func"></param>
        /// <returns>True if the cache exists; Otherwise False</returns>
        T CacheManager<T>(string key, TimeSpan absoluteExpirationRelativeToNow, Func<T> func) where T : class;
    }
}
