using ConfigurationApi.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigurationApi.Interfaces
{
    public interface IConfigurationRepository
    {
        Task CreateAsync(ConfigurationRecord config);
        Task UpdateAsync(ConfigurationRecord config);
        Task<ConfigurationRecord> GetByIdAsync(Guid id);
        Task<ConfigurationRecord> GetAsync(string applicationName, string environment, string key);
        Task<IEnumerable<ConfigurationRecord>> GetAllByEnvironmentAsync(string applicationName, string environment);
    }
}
