using System;
using System.Threading.Tasks;

namespace ConfigurationLibrary.Interfaces
{
    public interface ICacheService
    {
        Task<string> GetCacheValueAsync(string key);
        Task SetCacheValueAsync(string key, string value, TimeSpan ttl);
    }
}
