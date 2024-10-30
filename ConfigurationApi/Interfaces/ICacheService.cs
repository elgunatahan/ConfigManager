using System;
using System.Threading.Tasks;

namespace ConfigurationApi.Interfaces
{
    public interface ICacheService
    {
        Task<string> GetCacheValueAsync(string key);
        Task SetCacheValueAsync(string key, string value, TimeSpan ttl);
        Task<bool> RemoveCacheValueAsync(string key);
    }
}
