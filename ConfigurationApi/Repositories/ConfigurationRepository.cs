using ConfigurationApi.Documents;
using ConfigurationApi.Entities;
using ConfigurationApi.Entities.ValueObjects;
using ConfigurationApi.Exceptions;
using ConfigurationApi.Interfaces;
using MongoDB.Driver;
using SharpCompress.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigurationApi.Repositories
{
    public class ConfigurationRepository : IConfigurationRepository
    {
        private readonly IMongoCollection<ConfigurationRecordDocument> _configCollection;

        public ConfigurationRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("ConfigDB");
            _configCollection = database.GetCollection<ConfigurationRecordDocument>("Configurations");
        }

        public async Task CreateAsync(ConfigurationRecord config)
        {
            var document = (ConfigurationRecordDocument)config;
            document.Version = IdentityValueObject.NextVersionIncrement;
            document.CreatedAt = DateTime.UtcNow;

            await _configCollection.InsertOneAsync(document);

            config.IdentityObject.SetNextVersionNumber();
        }

        public async Task UpdateAsync(ConfigurationRecord config)
        {
            var filter = Builders<ConfigurationRecordDocument>.Filter.Eq(c => c.ApplicationName, config.ApplicationName) &
                         Builders<ConfigurationRecordDocument>.Filter.Eq(c => c.Environment, config.Environment) &
                         Builders<ConfigurationRecordDocument>.Filter.Eq(c => c.Key, config.Key) &
                         Builders<ConfigurationRecordDocument>.Filter.Eq(c => c.Version, config.IdentityObject.Version) &
                         Builders<ConfigurationRecordDocument>.Filter.Eq(c => c.IsActive, true);

            var update = Builders<ConfigurationRecordDocument>.Update
                .Set(c => c.Value, config.Value)
                .Set(c => c.Type, config.Type)
                .Set(c => c.UpdatedAt , DateTime.UtcNow)
                .Set(c => c.IsActive, config.IsActive)
                .Inc(c => c.Version, 1);

            var result = await _configCollection.UpdateOneAsync(filter, update);

            if (result.MatchedCount == 0)
            {
                throw new WrongExpectedVersionException($"The expected version {config.IdentityObject.Version} does not match the current version.");
            }

            config.IdentityObject.SetNextVersionNumber();
        }

        public async Task<ConfigurationRecord> GetAsync(string applicationName, string environment, string key)
        {
            var filter = Builders<ConfigurationRecordDocument>.Filter.Eq(c => c.ApplicationName, applicationName) &
                         Builders<ConfigurationRecordDocument>.Filter.Eq(c => c.Environment, environment) &
                         Builders<ConfigurationRecordDocument>.Filter.Eq(c => c.Key, key) &
                         Builders<ConfigurationRecordDocument>.Filter.Eq(c => c.IsActive, true);

            return await _configCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<ConfigurationRecord> GetByIdAsync(Guid id)
        {
            var filter = Builders<ConfigurationRecordDocument>.Filter.Eq(c => c.Id, id) &
                         Builders<ConfigurationRecordDocument>.Filter.Eq(c => c.IsActive, true);

            return await _configCollection.Find(filter).SingleOrDefaultAsync();
        }

        public async Task<IEnumerable<ConfigurationRecord>> GetAllByEnvironmentAsync(string applicationName, string environment)
        {
            var filter = Builders<ConfigurationRecordDocument>.Filter.Eq(c => c.ApplicationName, applicationName) &
                         Builders<ConfigurationRecordDocument>.Filter.Eq(c => c.Environment, environment) &
                         Builders<ConfigurationRecordDocument>.Filter.Eq(c => c.IsActive, true);

            var items = await _configCollection.Find(filter).ToListAsync();

            return items.Select(x => (ConfigurationRecord)x).ToList();
        }
    }
}
