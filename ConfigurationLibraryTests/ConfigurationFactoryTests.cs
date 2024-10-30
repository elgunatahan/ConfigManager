using ConfigurationLibrary.Factories;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Threading.Tasks;
using Testcontainers.MongoDb;
using Testcontainers.Redis;

namespace ConfigurationLibraryTests
{
    [TestFixture]
    public class ConfigurationFactoryTests
    {
        private readonly MongoDbContainer _mongoDbContainer;
        private readonly RedisContainer _redisContainer;

        public ConfigurationFactoryTests()
        {
            _mongoDbContainer = new MongoDbBuilder()
                .Build();

            _redisContainer = new RedisBuilder()
                .WithImage("redis:latest")
                .WithCleanUp(true)
                .Build();
        }

        [OneTimeSetUp]
        public async Task OneTimeSetUp()
        {
            await _mongoDbContainer.StartAsync();
            await _redisContainer.StartAsync();

            Environment.SetEnvironmentVariable("MONGO_CONNECTION_STRING", _mongoDbContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("REDIS_CONNECTION_STRING", _redisContainer.GetConnectionString());
            Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Development");
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            await _mongoDbContainer.StopAsync();
            await _redisContainer.StopAsync();
        }

        [Test]
        public void Create_ShouldReturnConfigurationReader_WhenEnvironmentVariablesAreSet()
        {
            // Act
            var configurationReader = ConfigurationFactory.Create("TestApp", 1000);

            // Assert
            configurationReader.Should().NotBeNull();
        }

        [Test]
        public void Create_ShouldThrowInvalidOperationException_WhenEnvironmentVariablesAreMissing()
        {
            // Arrange
            Environment.SetEnvironmentVariable("MONGO_CONNECTION_STRING", null);
            Environment.SetEnvironmentVariable("REDIS_CONNECTION_STRING", null);

            // Act
            Action act = () => ConfigurationFactory.Create("TestApp", 1000);

            // Assert
            act.Should().Throw<InvalidOperationException>()
               .WithMessage("Configuration environment variables are not set.");
        }

        public async ValueTask DisposeAsync()
        {
            if (_mongoDbContainer != null)
            {
                await _mongoDbContainer.DisposeAsync();
            }

            if (_redisContainer != null)
            {
                await _redisContainer.DisposeAsync();
            }
        }
    }
}
