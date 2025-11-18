// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using FluentAssertions;
using Moq;
using StackExchange.Redis;

namespace Viae.Services.Tests;

[TestClass]
[SuppressMessage(
    "Design",
    "MSTEST0016:Test class should have test method",
    Justification = "<Pending>"
)]
public class RedisCacheServiceTests
{
    private Mock<IConnectionMultiplexer> _mockRedis = null!;
    private Mock<IDatabase> _mockDatabase = null!;
    private RedisCacheService _sut = null!;

    [TestInitialize]
    public void Setup()
    {
        _mockRedis = new Mock<IConnectionMultiplexer>();
        _mockDatabase = new Mock<IDatabase>();
        _mockRedis
            .Setup(r => r.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_mockDatabase.Object);

        _sut = new RedisCacheService(_mockRedis.Object);
    }

    [TestClass]
    public class Constructor : RedisCacheServiceTests
    {
        [TestMethod]
        public void ThrowsArgumentNullException_WhenRedisIsNull()
        {
            // Act
            var act = () => new RedisCacheService(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>().WithParameterName("redis");
        }

        [TestMethod]
        public void AcceptsCustomJsonOptions()
        {
            // Arrange
            var customOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
            };

            // Act
            var service = new RedisCacheService(_mockRedis.Object, customOptions);

            // Assert
            service.Should().NotBeNull();
        }
    }

    [TestClass]
    public class GetAsync : RedisCacheServiceTests
    {
        [TestMethod]
        public async Task ReturnsNull_WhenKeyDoesNotExist()
        {
            // Arrange
            _mockDatabase
                .Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(RedisValue.Null);

            // Act
            var result = await _sut.GetAsync<TestObject>(
                "nonexistent-key",
                TestContext.CancellationToken
            );

            // Assert
            result.Should().BeNull();
        }

        [TestMethod]
        [SuppressMessage(
            "Performance",
            "CA1869:Cache and reuse 'JsonSerializerOptions' instances",
            Justification = "<Pending>"
        )]
        public async Task ReturnsDeserializedObject_WhenKeyExists()
        {
            // Arrange
            var testObject = new TestObject
            {
                Id = 42,
                Name = "Test",
                CreatedAt = DateTime.UtcNow,
            };
            var json = JsonSerializer.Serialize(
                testObject,
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );

            _mockDatabase
                .Setup(db => db.StringGetAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(new RedisValue(json));

            // Act
            var result = await _sut.GetAsync<TestObject>("test-key", TestContext.CancellationToken);

            // Assert
            result.Should().NotBeNull();
            result!.Id.Should().Be(testObject.Id);
            result.Name.Should().Be(testObject.Name);
            result.CreatedAt.Should().BeCloseTo(testObject.CreatedAt, TimeSpan.FromSeconds(1));
        }

        [TestMethod]
        public async Task ThrowsArgumentException_WhenKeyIsNull()
        {
            // Act
            var act = async () =>
                await _sut.GetAsync<TestObject>(null!, TestContext.CancellationToken);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        public async Task ThrowsArgumentException_WhenKeyIsEmpty()
        {
            // Act
            var act = async () =>
                await _sut.GetAsync<TestObject>("", TestContext.CancellationToken);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        public async Task ThrowsArgumentException_WhenKeyIsWhitespace()
        {
            // Act
            var act = async () =>
                await _sut.GetAsync<TestObject>("   ", TestContext.CancellationToken);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        public TestContext TestContext { get; set; }
    }

    [TestClass]
    public class SetAsync : RedisCacheServiceTests
    {
        [TestMethod]
        [SuppressMessage(
            "Performance",
            "CA1869:Cache and reuse 'JsonSerializerOptions' instances",
            Justification = "<Pending>"
        )]
        public async Task StoresSerializedObject_WithExpiration()
        {
            // Arrange
            var testObject = new TestObject
            {
                Id = 123,
                Name = "Cache Test",
                CreatedAt = DateTime.UtcNow,
            };
            var expiration = TimeSpan.FromMinutes(30);
            RedisValue capturedValue = default;
            TimeSpan? capturedExpiry = null;

            _mockDatabase
                .Setup(db =>
                    db.StringSetAsync(
                        It.IsAny<RedisKey>(),
                        It.IsAny<RedisValue>(),
                        It.IsAny<TimeSpan?>(),
                        It.IsAny<bool>(),
                        It.IsAny<When>(),
                        It.IsAny<CommandFlags>()
                    )
                )
                .Callback<RedisKey, RedisValue, TimeSpan?, bool, When, CommandFlags>(
                    (key, value, expiry, keepTtl, when, flags) =>
                    {
                        capturedValue = value;
                        capturedExpiry = expiry;
                    }
                )
                .ReturnsAsync(true);

            // Act
            await _sut.SetAsync("test-key", testObject, expiration, TestContext.CancellationToken);

            // Assert
            _mockDatabase.Verify(
                db =>
                    db.StringSetAsync(
                        "test-key",
                        It.IsAny<RedisValue>(),
                        expiration,
                        It.IsAny<bool>(),
                        It.IsAny<When>(),
                        It.IsAny<CommandFlags>()
                    ),
                Times.Once
            );

            capturedExpiry.Should().Be(expiration);

            var deserializedObject = JsonSerializer.Deserialize<TestObject>(
                capturedValue!.ToString(),
                new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }
            );
            deserializedObject.Should().NotBeNull();
            deserializedObject!.Id.Should().Be(testObject.Id);
            deserializedObject.Name.Should().Be(testObject.Name);
        }

        [TestMethod]
        public async Task ThrowsArgumentException_WhenKeyIsNull()
        {
            // Act
            var act = async () =>
                await _sut.SetAsync(
                    null!,
                    new TestObject(),
                    TimeSpan.FromMinutes(5),
                    TestContext.CancellationToken
                );

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        public async Task ThrowsArgumentException_WhenKeyIsEmpty()
        {
            // Act
            var act = async () =>
                await _sut.SetAsync(
                    "",
                    new TestObject(),
                    TimeSpan.FromMinutes(5),
                    TestContext.CancellationToken
                );

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        public async Task ThrowsArgumentNullException_WhenValueIsNull()
        {
            // Act
            var act = async () =>
                await _sut.SetAsync<TestObject>(
                    "test-key",
                    null!,
                    TimeSpan.FromMinutes(5),
                    TestContext.CancellationToken
                );

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [TestMethod]
        public async Task ThrowsArgumentOutOfRangeException_WhenExpirationIsZero()
        {
            // Act
            var act = async () =>
                await _sut.SetAsync(
                    "test-key",
                    new TestObject(),
                    TimeSpan.Zero,
                    TestContext.CancellationToken
                );

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentOutOfRangeException>()
                .WithParameterName("expiration");
        }

        [TestMethod]
        public async Task ThrowsArgumentOutOfRangeException_WhenExpirationIsNegative()
        {
            // Act
            var act = async () =>
                await _sut.SetAsync(
                    "test-key",
                    new TestObject(),
                    TimeSpan.FromMinutes(-1),
                    TestContext.CancellationToken
                );

            // Assert
            await act.Should()
                .ThrowAsync<ArgumentOutOfRangeException>()
                .WithParameterName("expiration");
        }

        public TestContext TestContext { get; set; }
    }

    [TestClass]
    public class InvalidateAsync : RedisCacheServiceTests
    {
        [TestMethod]
        public async Task ReturnsTrue_WhenKeyIsDeleted()
        {
            // Arrange
            _mockDatabase
                .Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.InvalidateAsync("test-key", TestContext.CancellationToken);

            // Assert
            result.Should().BeTrue();
            _mockDatabase.Verify(
                db => db.KeyDeleteAsync("test-key", It.IsAny<CommandFlags>()),
                Times.Once
            );
        }

        [TestMethod]
        public async Task ReturnsFalse_WhenKeyDoesNotExist()
        {
            // Arrange
            _mockDatabase
                .Setup(db => db.KeyDeleteAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(false);

            // Act
            var result = await _sut.InvalidateAsync(
                "nonexistent-key",
                TestContext.CancellationToken
            );

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task ThrowsArgumentException_WhenKeyIsNull()
        {
            // Act
            var act = async () => await _sut.InvalidateAsync(null!, TestContext.CancellationToken);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        [TestMethod]
        public async Task ThrowsArgumentException_WhenKeyIsEmpty()
        {
            // Act
            var act = async () => await _sut.InvalidateAsync("", TestContext.CancellationToken);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        public TestContext TestContext { get; set; }
    }

    [TestClass]
    public class ExistsAsync : RedisCacheServiceTests
    {
        [TestMethod]
        public async Task ReturnsTrue_WhenKeyExists()
        {
            // Arrange
            _mockDatabase
                .Setup(db => db.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(true);

            // Act
            var result = await _sut.ExistsAsync("existing-key", TestContext.CancellationToken);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task ReturnsFalse_WhenKeyDoesNotExist()
        {
            // Arrange
            _mockDatabase
                .Setup(db => db.KeyExistsAsync(It.IsAny<RedisKey>(), It.IsAny<CommandFlags>()))
                .ReturnsAsync(false);

            // Act
            var result = await _sut.ExistsAsync("nonexistent-key", TestContext.CancellationToken);

            // Assert
            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task ThrowsArgumentException_WhenKeyIsNull()
        {
            // Act
            var act = async () => await _sut.ExistsAsync(null!, TestContext.CancellationToken);

            // Assert
            await act.Should().ThrowAsync<ArgumentException>();
        }

        public TestContext TestContext { get; set; }
    }

    // Test model class
    private sealed class TestObject
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
