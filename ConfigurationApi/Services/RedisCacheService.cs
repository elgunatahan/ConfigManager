using ConfigurationApi.Interfaces;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace ConfigurationApi.Services
{
    public class RedisCacheService : ICacheService
    {
        private readonly IDatabase _redis;

        public RedisCacheService(IDatabase redisDatabase)
        {
            _redis = redisDatabase;
        }

        public async Task<string> GetCacheValueAsync(string key)
        {
            return await _redis.StringGetAsync(key);
        }

        public async Task SetCacheValueAsync(string key, string value, TimeSpan ttl)
        {
            await _redis.StringSetAsync(key, value, ttl);
        }

        public async Task<bool> RemoveCacheValueAsync(string key)
        {
            return await _redis.KeyDeleteAsync(key);
        }
    }
}
