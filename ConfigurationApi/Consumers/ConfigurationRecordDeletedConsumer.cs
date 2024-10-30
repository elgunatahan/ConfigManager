using ConfigurationApi.Events;
using ConfigurationApi.Interfaces;
using MassTransit;
using System.Threading.Tasks;

namespace ConfigurationApi.Consumers
{
    public class ConfigurationRecordDeletedConsumer : IConsumer<ConfigurationRecordDeleted>
    {
        private readonly ICacheService _cacheService;
        public ConfigurationRecordDeletedConsumer(ICacheService cacheService)
        {
            _cacheService = cacheService;
        }
        public async Task Consume(ConsumeContext<ConfigurationRecordDeleted> context)
        {
            var cacheKey = $"{context.Message.Environment}_{context.Message.ApplicationName}_{context.Message.Key}";

            await _cacheService.RemoveCacheValueAsync(cacheKey);
        }
    }
}
