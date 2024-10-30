using ConfigurationApi.Events;
using ConfigurationApi.Interfaces;
using MassTransit;
using System;
using System.Threading.Tasks;

namespace ConfigurationApi.Consumers
{
    public class ConfigurationRecordUpdatedConsumer : IConsumer<ConfigurationRecordUpdated>
    {
        private readonly ICacheService _cacheService;
        private readonly IConfigurationRepository _configurationRepository;
        public ConfigurationRecordUpdatedConsumer(ICacheService cacheService, IConfigurationRepository configurationRepository)
        {
            _cacheService = cacheService;
            _configurationRepository = configurationRepository;
        }
        public async Task Consume(ConsumeContext<ConfigurationRecordUpdated> context)
        {
            var config = await _configurationRepository.GetByIdAsync(context.Message.ConfigurationRecordId);

            if (config == null || config.IdentityObject.Version > context.Message.Version)
            {
                throw new ApplicationException();
            }

            var cacheKey = $"{config.Environment}_{config.ApplicationName}_{config.Key}";

            await _cacheService.SetCacheValueAsync(cacheKey, config.Value, TimeSpan.FromMilliseconds(300000));
        }
    }
}
