using ConfigurationLibrary.Entities;
using ConfigurationLibrary.Interfaces;
using MongoDB.Driver;
using System.Threading.Tasks;

namespace ConfigurationLibrary.Repositories
{
    internal class ConfigurationRepository : IConfigurationRepository
    {

        private readonly IMongoCollection<ConfigurationRecord> _configCollection;

        public ConfigurationRepository(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("ConfigDB");
            _configCollection = database.GetCollection<ConfigurationRecord>("Configurations");
        }

        public async Task<ConfigurationRecord> GetConfigValueAsync(string environment, string applicationName, string key)
        {
            var filter = Builders<ConfigurationRecord>.Filter.Eq(c => c.Environment, environment) &
                         Builders<ConfigurationRecord>.Filter.Eq(c => c.ApplicationName, applicationName) &
                         Builders<ConfigurationRecord>.Filter.Eq(c => c.Key, key) &
                         Builders<ConfigurationRecord>.Filter.Eq(c => c.IsActive, true);

            var config = await _configCollection.Find(filter).FirstOrDefaultAsync();

            return config;
        }
    }
}
