using ConfigurationLibrary.Entities;
using System.Threading.Tasks;

namespace ConfigurationLibrary.Interfaces
{
    public interface IConfigurationRepository
    {
        Task<ConfigurationRecord> GetConfigValueAsync(string environment, string applicationName, string key);
    }
}
