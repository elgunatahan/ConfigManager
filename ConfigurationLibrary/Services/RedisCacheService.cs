using ConfigurationLibrary.Interfaces;
using StackExchange.Redis;
using System;
using System.Threading.Tasks;

namespace ConfigurationLibrary.Services
{
    internal class RedisCacheService : ICacheService
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
    }
}
