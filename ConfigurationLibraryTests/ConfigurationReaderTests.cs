using ConfigurationLibrary;
using ConfigurationLibrary.Interfaces;
using FluentAssertions;
using Moq;
using NUnit.Framework;
using RedLockNet;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ConfigurationLibraryTests
{
    [TestFixture]
    public class ConfigurationReaderTests
    {
        private ConfigurationReader _configReader;
        private Mock<ICacheService> _mockCacheService;
        private Mock<IConfigurationRepository> _mockConfigRepository;
        private Mock<IDistributedLockFactory> _mockRedLockFactory;
        private Mock<ITypeParser> _mockTypeParser;
        private Mock<IRedLock> _mockRedLock;

        [SetUp]
        public void Setup()
        {
            _mockCacheService = new Mock<ICacheService>();
            _mockConfigRepository = new Mock<IConfigurationRepository>();
            _mockRedLockFactory = new Mock<IDistributedLockFactory>();
            _mockTypeParser = new Mock<ITypeParser>();
            _mockRedLock = new Mock<IRedLock>();

            _mockRedLock.Setup(r => r.IsAcquired).Returns(true);
            _mockRedLockFactory.Setup(f => f.CreateLockAsync(It.IsAny<string>(), It.IsAny<TimeSpan>()))
                .ReturnsAsync(_mockRedLock.Object);

            _configReader = new ConfigurationReader(
                "TestApp",
                "TestEnv",
                1000,
                _mockCacheService.Object,
                _mockConfigRepository.Object,
                _mockRedLockFactory.Object,
                _mockTypeParser.Object);
        }

        [Test]
        public async Task GetValueAsync_ShouldReturnCachedValue_WhenCacheIsHit()
        {
            // Arrange
            string key = "TestKey";
            string cacheKey = "TestEnv_TestApp_TestKey";
            string cachedValue = "TestValue";
            _mockCacheService.Setup(s => s.GetCacheValueAsync(cacheKey)).ReturnsAsync(cachedValue);
            _mockTypeParser.Setup(p => p.Parse<string>(cachedValue)).Returns(cachedValue);

            // Act
            var result = await _configReader.GetValueAsync<string>(key);

            // Assert
            result.Should().Be(cachedValue);
            _mockCacheService.Verify(s => s.GetCacheValueAsync(cacheKey), Times.Once);
            _mockConfigRepository.Verify(r => r.GetConfigValueAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public async Task GetValueAsync_ShouldRetrieveFromDatabaseAndUpdateCache_WhenCacheIsMissed()
        {
            // Arrange
            string key = "TestKey";
            string cacheKey = "TestEnv_TestApp_TestKey";
            string dbValue = "TestValue";
            _mockCacheService.Setup(s => s.GetCacheValueAsync(cacheKey)).ReturnsAsync((string)null);
            _mockConfigRepository.Setup(r => r.GetConfigValueAsync("TestEnv", "TestApp", key))
                .ReturnsAsync(new ConfigurationLibrary.Entities.ConfigurationRecord { Value = dbValue });
            _mockTypeParser.Setup(p => p.Parse<string>(dbValue)).Returns(dbValue);

            // Act
            var result = await _configReader.GetValueAsync<string>(key);

            // Assert
            result.Should().Be(dbValue);
            _mockCacheService.Verify(s => s.GetCacheValueAsync(cacheKey), Times.Exactly(2));
            _mockConfigRepository.Verify(r => r.GetConfigValueAsync("TestEnv", "TestApp", key), Times.Once);
            _mockCacheService.Verify(s => s.SetCacheValueAsync(cacheKey, dbValue, It.IsAny<TimeSpan>()), Times.Once);
        }

        [Test]
        public void GetValueAsync_ShouldThrowException_WhenLockIsNotAcquired()
        {
            // Arrange
            string key = "TestKey";
            _mockRedLock.Setup(r => r.IsAcquired).Returns(false);

            // Act
            Func<Task> act = async () => await _configReader.GetValueAsync<string>(key);

            // Assert
            act.Should().ThrowAsync<Exception>()
               .WithMessage("Could not acquire lock to retrieve configuration.");
        }

        [Test]
        public void GetValueAsync_ShouldThrowKeyNotFoundException_WhenKeyDoesNotExistInCacheOrDb()
        {
            // Arrange
            string key = "MissingKey";
            string cacheKey = "TestEnv_TestApp_MissingKey";
            _mockCacheService.Setup(s => s.GetCacheValueAsync(cacheKey)).ReturnsAsync((string)null);
            _mockConfigRepository.Setup(r => r.GetConfigValueAsync("TestEnv", "TestApp", key))
                .ReturnsAsync((ConfigurationLibrary.Entities.ConfigurationRecord)null);

            // Act
            Func<Task> act = async () => await _configReader.GetValueAsync<string>(key);

            // Assert
            act.Should().ThrowAsync<KeyNotFoundException>()
               .WithMessage($"The configuration key '{key}' was not found.");
        }
    }
}
