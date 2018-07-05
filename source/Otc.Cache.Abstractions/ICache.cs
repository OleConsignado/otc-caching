namespace Otc.Cache.Abstractions
{
    public interface ICache
    {
        void Set<T>(string key, T entity, int minutes) where T : class;

        bool TryGetValue<T>(string key, out T entity) where T : class;

        void Remove(string key);
    }
}
