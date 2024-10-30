using ConfigurationLibrary.Interfaces;
using RedLockNet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigurationLibrary
{
    public class ConfigurationReader
    {
        private readonly ICacheService _cacheService;
        private readonly IConfigurationRepository _configRepository;
        private readonly IDistributedLockFactory _redLockFactory;
        private readonly ITypeParser _typeParser;
        private readonly string _applicationName;
        private readonly string _environment;
        private readonly TimeSpan _cacheTTL;

        public ConfigurationReader(
        string applicationName,
        string environment,
        int refreshTimerIntervalInMs,
        ICacheService cacheService,
        IConfigurationRepository configRepository,
        IDistributedLockFactory redLockFactory,
        ITypeParser typeParser)
        {
            _applicationName = applicationName;
            _environment = environment;
            _cacheTTL = TimeSpan.FromMilliseconds(refreshTimerIntervalInMs);
            _cacheService = cacheService;
            _configRepository = configRepository;
            _redLockFactory = redLockFactory;
            _typeParser = typeParser;
        }

        public async Task<T> GetValueAsync<T>(string key)
        {
            var cacheKey = $"{_environment}_{_applicationName}_{key}";
            var lockTimeout = TimeSpan.FromSeconds(60);


            var cachedValue = await _cacheService.GetCacheValueAsync(cacheKey);
            if (!string.IsNullOrEmpty(cachedValue))
            {
                return _typeParser.Parse<T>(cachedValue);
            }

            using (var redLock = await _redLockFactory.CreateLockAsync(key, lockTimeout))
            {
                if (redLock.IsAcquired)
                {
                    cachedValue = await _cacheService.GetCacheValueAsync(cacheKey);
                    if (!string.IsNullOrEmpty(cachedValue))
                    {
                        return _typeParser.Parse<T>(cachedValue);
                    }

                    var dbValue = await _configRepository.GetConfigValueAsync(_environment, _applicationName, key);
                    if (dbValue != null)
                    {
                        await _cacheService.SetCacheValueAsync(cacheKey, dbValue.Value, _cacheTTL);
                        return _typeParser.Parse<T>(dbValue.Value);
                    }

                    throw new KeyNotFoundException($"The configuration key '{key}' was not found.");
                }
                else
                {
                    throw new Exception("Could not acquire lock to retrieve configuration.");
                }
            }
        }
    }
}
