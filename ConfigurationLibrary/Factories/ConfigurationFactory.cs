using ConfigurationLibrary.Common;
using ConfigurationLibrary.Interfaces;
using ConfigurationLibrary.Repositories;
using ConfigurationLibrary.Services;
using MongoDB.Driver;
using RedLockNet.SERedis.Configuration;
using RedLockNet.SERedis;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using RedLockNet;

namespace ConfigurationLibrary.Factories
{
    public static class ConfigurationFactory
    {
        public static ConfigurationReader Create(string applicationName, int refreshTimerIntervalInMs)
        {
            var mongoConnectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING");
            var redisConnectionString = Environment.GetEnvironmentVariable("REDIS_CONNECTION_STRING");
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");


            if (string.IsNullOrEmpty(mongoConnectionString) || string.IsNullOrEmpty(redisConnectionString))
            {
                throw new InvalidOperationException("Configuration environment variables are not set.");
            }

            var connectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
            IDistributedLockFactory redLockFactory = RedLockFactory.Create(new List<RedLockMultiplexer> { connectionMultiplexer });

            var mongoClient = new MongoClient(mongoConnectionString);
            IConfigurationRepository configRepository = new ConfigurationRepository(mongoClient);

            var redisConnection = ConnectionMultiplexer.Connect(redisConnectionString);
            var redisDatabase = redisConnection.GetDatabase();

            ICacheService cacheService = new RedisCacheService(redisDatabase);

            ITypeParser typeParser = new TypeParser();

            return new ConfigurationReader(
                applicationName,
                environment,
                refreshTimerIntervalInMs,
                cacheService,
                configRepository,
                redLockFactory,
                typeParser
            );
        }
    }
}
