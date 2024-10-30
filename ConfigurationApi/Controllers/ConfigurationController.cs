using ConfigurationApi.Common;
using ConfigurationApi.Entities;
using ConfigurationApi.Events;
using ConfigurationApi.Exceptions;
using ConfigurationApi.Interfaces;
using ConfigurationApi.Models.Requests;
using ConfigurationApi.Models.Responses;
using MassTransit;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ConfigurationApi.Controllers
{
    [ApiController]
    [Route("api/v1/configurations")]
    public class ConfigurationController : ControllerBase
    {
        private readonly IConfigurationRepository _configRepository;
        private readonly ISendEndpointProvider _sendEndpointProvider;

        public ConfigurationController(IConfigurationRepository configRepository, ISendEndpointProvider sendEndpointProvider)
        {
            _configRepository = configRepository;
            _sendEndpointProvider = sendEndpointProvider;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(CreateConfigurationRecordResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultExceptionDto))]
        public async Task<IActionResult> Create([FromBody] CreateConfigurationRecordRequest input)
        {
            if (!IsValidValue(input.Value, input.Type))
            {
                throw new ValidationException("Invalid value or type provided");
            }

            var config = new ConfigurationRecord
            (
                input.Environment,
                input.ApplicationName,
                input.Key,
                input.Value,
                input.Type
            );

            await _configRepository.CreateAsync(config);

            ConfigurationRecordCreated configurationRecordCreated = new ConfigurationRecordCreated()
            {
                ConfigurationRecordId = config.IdentityObject.Id
            };

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:config_created"));
            await endpoint.Send(configurationRecordCreated);

            return Created($"api/v1/configurations/{config.IdentityObject.Id}", new CreateConfigurationRecordResponse { Id = config.IdentityObject.Id });

        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status202Accepted, Type = typeof(UpdateConfigurationRecordResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(DefaultExceptionDto))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(DefaultExceptionDto))]
        public async Task<IActionResult> Update([FromRoute] Guid id, [FromBody] UpdateConfigurationRecordRequest input)
        {
            if (!IsValidValue(input.Value, input.Type))
            {
                throw new ValidationException("Invalid value or type provided");
            }

            var config = await _configRepository.GetByIdAsync(id);


            if (config == null)
            {
                throw new NotFoundException($"Config with given {id} not found");
            }

            config.Update(input.Value, input.Type);

            await _configRepository.UpdateAsync(config);

            ConfigurationRecordUpdated configurationRecordUpdated = new ConfigurationRecordUpdated()
            {
                ConfigurationRecordId = config.IdentityObject.Id,
                Version = config.IdentityObject.Version
            };

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:config_updated"));
            await endpoint.Send(configurationRecordUpdated);

            return Accepted($"api/v1/configurations/{config.IdentityObject.Id}", new UpdateConfigurationRecordResponse { Id = config.IdentityObject.Id });
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetConfigurationRecordResponse))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(DefaultExceptionDto))]
        public async Task<IActionResult> Get([FromRoute] Guid id)
        {
            var config = await _configRepository.GetByIdAsync(id);

            if (config == null)
            {
                throw new NotFoundException($"Config with given {id} not found");
            }

            return Ok(new GetConfigurationRecordResponse()
            {
                Id = config.IdentityObject.Id,
                Version = config.IdentityObject.Version,
                ApplicationName = config.ApplicationName,
                Key = config.Key,
                Environment = config.Environment,
                Type = config.Type,
                Value = config.Value
            });
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(QueryConfigurationRecordsResponse))]
        public async Task<IActionResult> GetAll([FromQuery] string environment, [FromQuery] string applicationName)
        {
            var configs = await _configRepository.GetAllByEnvironmentAsync(applicationName, environment);

            return Ok(new QueryConfigurationRecordsResponse()
            {
                Items = configs.Select(x => new ConfigurationRecordItemResponse()
                {
                    Id = x.IdentityObject.Id,
                    Version = x.IdentityObject.Version,
                    ApplicationName = x.ApplicationName,
                    Key = x.Key,
                    Environment = x.Environment,
                    Type = x.Type,
                    Value = x.Value
                }).ToList(),
                TotalRecords = configs.Count()
            });
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(DefaultExceptionDto))]
        public async Task<IActionResult> Delete([FromRoute] Guid id)
        {
            var config = await _configRepository.GetByIdAsync(id);

            if (config == null)
            {
                throw new NotFoundException($"Config with given {id} not found");
            }

            config.Delete();

            await _configRepository.UpdateAsync(config);

            ConfigurationRecordDeleted configurationRecordDeleted = new ConfigurationRecordDeleted()
            {
                ConfigurationRecordId = config.IdentityObject.Id,
                ApplicationName = config.ApplicationName,
                Key = config.Key,
                Environment = config.Environment,
            };

            var endpoint = await _sendEndpointProvider.GetSendEndpoint(new Uri($"queue:config_deleted"));

            await endpoint.Send(configurationRecordDeleted);

            return Ok();
        }


        private bool IsValidValue(string value, string type)
        {
            if (string.IsNullOrWhiteSpace(value) || string.IsNullOrWhiteSpace(type)) return false;

            return type.ToLower() switch
            {
                "int" => int.TryParse(value, out _),
                "double" => double.TryParse(value, out _),
                "bool" => bool.TryParse(value, out _),
                "string" => true,
                _ => false
            };
        }
    }
}
