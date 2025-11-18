// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Viae.Persistence;
using Viae.Testing.Common;

namespace Viae.Services.Tests;

/// <summary>
/// Tests for the MissioService that handles Ping and Pong activities.
/// </summary>
[TestClass]
public class MissioServiceTests
{
    private Func<TestViaeDbContext> _createDbContext = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ViaeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _createDbContext = () => new TestViaeDbContext(options);
    }

    /// <summary>
    /// Simple test-only URI generator.
    /// </summary>
    private sealed class TestUriGenerator : IUriGeneratorService
    {
        public Uri BaseUri => new("https://example.com");

        public Uri GenerateUri(Domain.Models.Admissio entity)
        {
            return new Uri($"{BaseUri.AbsoluteUri.TrimEnd('/')}/ping/{entity.Id}");
        }

        public Uri GenerateUri(Domain.Models.Adreflexio entity)
        {
            return new Uri($"{BaseUri.AbsoluteUri.TrimEnd('/')}/pong/{entity.Id}");
        }
    }

    /// <summary>
    /// Creates a MissioService instance for testing with proper factory setup.
    /// </summary>
    private MissioService CreateService()
    {
        var factoryMock = new Mock<IDbContextFactory<ViaeDbContext>>();
        factoryMock.Setup(f => f.CreateDbContext()).Returns(_createDbContext);
        var loggerMock = new Mock<ILogger<MissioService>>();
        return new MissioService(factoryMock.Object, loggerMock.Object, new TestUriGenerator());
    }

    #region CreateOutboundPingAsync Tests

    [TestMethod]
    public async Task CreateOutboundPingAsync_ShouldCreatePing_WithGeneratedId()
    {
        // Arrange
        var service = CreateService();
        var actor = new Uri("https://example.com/users/alice");
        var to = new Uri("https://remote.com/users/bob");

        // Act
        var result = await service.CreateOutboundPingAsync(actor, to);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.Actor.Should().Be(actor);
        result.To.Should().Be(to);
        result.Uri.ToString().Should().StartWith("https://example.com/ping/");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.SentAt.Should().BeNull();
        result.PongReceived.Should().BeFalse();
        result.PongReceivedAt.Should().BeNull();
    }

    [TestMethod]
    public async Task CreateOutboundPingAsync_ShouldPersistPing_ToDatabase()
    {
        // Arrange
        var service = CreateService();

        var actor = new Uri("https://example.com/users/alice");
        var to = new Uri("https://remote.com/users/bob");

        // Act
        var result = await service.CreateOutboundPingAsync(actor, to);

        // Assert
        using var dbContext = _createDbContext();
        var saved = await dbContext.Admissiones.FindAsync(
            [result.Id],
            TestContext.CancellationToken
        );
        saved.Should().NotBeNull();
        saved!.Id.Should().Be(result.Id);
        saved.Actor.Should().Be(actor);
        saved.To.Should().Be(to);
    }

    [TestMethod]
    public async Task CreateOutboundPingAsync_ShouldGenerateUniqueIds_ForMultiplePings()
    {
        // Arrange
        var service = CreateService();

        var actor = new Uri("https://example.com/users/alice");
        var to = new Uri("https://remote.com/users/bob");

        // Act
        var ping1 = await service.CreateOutboundPingAsync(actor, to);
        var ping2 = await service.CreateOutboundPingAsync(actor, to);
        var ping3 = await service.CreateOutboundPingAsync(actor, to);

        // Assert
        var ids = new[] { ping1.Id, ping2.Id, ping3.Id };
        ids.Should().OnlyHaveUniqueItems();
    }

    #endregion

    #region RecordInboundPingAsync Tests

    [TestMethod]
    public async Task RecordInboundPingAsync_ShouldRecordPing_WithProvidedUri()
    {
        // Arrange
        var service = CreateService();

        var uri = new Uri("https://remote.com/ping/abc123");
        var actor = new Uri("https://remote.com/users/charlie");
        var to = new Uri("https://example.com/users/alice");

        // Act
        var result = await service.RecordInboundPingAsync(uri, actor, to);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.Uri.Should().Be(uri);
        result.Actor.Should().Be(actor);
        result.To.Should().Be(to);
        result.ReceivedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.PongSent.Should().BeFalse();
        result.PongSentAt.Should().BeNull();
    }

    [TestMethod]
    public async Task RecordInboundPingAsync_ShouldPersistPing_ToDatabase()
    {
        // Arrange
        var service = CreateService();

        var uri = new Uri("https://remote.com/ping/abc123");
        var actor = new Uri("https://remote.com/users/charlie");
        var to = new Uri("https://example.com/users/alice");

        // Act
        var result = await service.RecordInboundPingAsync(uri, actor, to);

        // Assert
        using var dbContext = _createDbContext();
        var saved = await dbContext.Abmissiones.FindAsync(
            [result.Id],
            TestContext.CancellationToken
        );
        saved.Should().NotBeNull();
        saved!.Uri.Should().Be(uri);
        saved.Actor.Should().Be(actor);
        saved.To.Should().Be(to);
    }

    #endregion

    #region CreateOutboundPongAsync Tests

    [TestMethod]
    public async Task CreateOutboundPongAsync_ShouldCreatePong_WithPingReference()
    {
        // Arrange
        var service = CreateService();

        var actor = new Uri("https://example.com/users/alice");
        var to = new Uri("https://remote.com/users/bob");
        var pingUri = new Uri("https://remote.com/ping/xyz789");

        // Act
        var result = await service.CreateOutboundPongAsync(actor, to, pingUri);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.Actor.Should().Be(actor);
        result.To.Should().Be(to);
        result.PingUri.Should().Be(pingUri);
        result.Uri.ToString().Should().StartWith("https://example.com/pong/");
        result.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
        result.SentAt.Should().BeNull();
    }

    [TestMethod]
    public async Task CreateOutboundPongAsync_ShouldPersistPong_ToDatabase()
    {
        // Arrange
        var service = CreateService();

        var actor = new Uri("https://example.com/users/alice");
        var to = new Uri("https://remote.com/users/bob");
        var pingUri = new Uri("https://remote.com/ping/xyz789");

        // Act
        var result = await service.CreateOutboundPongAsync(actor, to, pingUri);

        // Assert
        using var dbContext = _createDbContext();
        var saved = await dbContext.Adreflexiones.FindAsync(
            [result.Id],
            TestContext.CancellationToken
        );
        saved.Should().NotBeNull();
        saved!.PingUri.Should().Be(pingUri);
    }

    #endregion

    #region RecordInboundPongAsync Tests

    [TestMethod]
    public async Task RecordInboundPongAsync_ShouldRecordPong_WithPingReference()
    {
        // Arrange
        var service = CreateService();

        var uri = new Uri("https://remote.com/pong/def456");
        var actor = new Uri("https://remote.com/users/charlie");
        var to = new Uri("https://example.com/users/alice");
        var pingUri = new Uri("https://example.com/ping/original123");

        // Act
        var result = await service.RecordInboundPongAsync(uri, actor, to, pingUri);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().NotBeNullOrEmpty();
        result.Uri.Should().Be(uri);
        result.Actor.Should().Be(actor);
        result.To.Should().Be(to);
        result.PingUri.Should().Be(pingUri);
        result.ReceivedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [TestMethod]
    public async Task RecordInboundPongAsync_ShouldPersistPong_ToDatabase()
    {
        // Arrange
        var service = CreateService();

        var uri = new Uri("https://remote.com/pong/def456");
        var actor = new Uri("https://remote.com/users/charlie");
        var to = new Uri("https://example.com/users/alice");
        var pingUri = new Uri("https://example.com/ping/original123");

        // Act
        var result = await service.RecordInboundPongAsync(uri, actor, to, pingUri);

        // Assert
        using var dbContext = _createDbContext();
        var saved = await dbContext.Abreflexiones.FindAsync(
            [result.Id],
            TestContext.CancellationToken
        );
        saved.Should().NotBeNull();
        saved!.PingUri.Should().Be(pingUri);
        saved.Uri.Should().Be(uri);
    }

    #endregion

    #region MarkInboundPingAsPongedAsync Tests

    [TestMethod]
    public async Task MarkInboundPingAsPongedAsync_ShouldMarkPing_AsPonged()
    {
        // Arrange
        var service = CreateService();

        var ping = await service.RecordInboundPingAsync(
            new Uri("https://remote.com/ping/test1"),
            new Uri("https://remote.com/users/bob"),
            new Uri("https://example.com/users/alice")
        );

        // Act
        await service.MarkInboundPingAsPongedAsync(ping.Id);

        // Assert
        using var dbContext = _createDbContext();
        var updated = await dbContext.Abmissiones.FindAsync(
            [ping.Id],
            TestContext.CancellationToken
        );
        updated.Should().NotBeNull();
        updated!.PongSent.Should().BeTrue();
        updated.PongSentAt.Should().NotBeNull();
        updated.PongSentAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [TestMethod]
    public async Task MarkInboundPingAsPongedAsync_ShouldNotThrow_WhenPingNotFound()
    {
        // Arrange
        var service = CreateService();

        var nonExistentId = "nonexistent123";

        // Act
        var act = async () => await service.MarkInboundPingAsPongedAsync(nonExistentId);

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region MarkOutboundPingAsPongReceivedAsync Tests

    [TestMethod]
    public async Task MarkOutboundPingAsPongReceivedAsync_ShouldMarkPing_AsPongReceived()
    {
        // Arrange
        var service = CreateService();

        var ping = await service.CreateOutboundPingAsync(
            new Uri("https://example.com/users/alice"),
            new Uri("https://remote.com/users/bob")
        );

        // Act
        await service.MarkOutboundPingAsPongReceivedAsync(ping.Uri);

        // Assert
        using var dbContext = _createDbContext();
        var updated = await dbContext.Admissiones.FindAsync(
            [ping.Id],
            TestContext.CancellationToken
        );
        updated.Should().NotBeNull();
        updated!.PongReceived.Should().BeTrue();
        updated.PongReceivedAt.Should().NotBeNull();
        updated.PongReceivedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [TestMethod]
    public async Task MarkOutboundPingAsPongReceivedAsync_ShouldNotThrow_WhenPingNotFound()
    {
        // Arrange
        var service = CreateService();

        var nonExistentUri = new Uri("https://example.com/ping/nonexistent");

        // Act
        var act = async () => await service.MarkOutboundPingAsPongReceivedAsync(nonExistentUri);

        // Assert
        await act.Should().NotThrowAsync();
    }

    #endregion

    #region GetOutboundPingAsync Tests

    [TestMethod]
    public async Task GetOutboundPingAsync_ShouldReturnPing_WhenExists()
    {
        // Arrange
        var service = CreateService();

        var ping = await service.CreateOutboundPingAsync(
            new Uri("https://example.com/users/alice"),
            new Uri("https://remote.com/users/bob")
        );

        // Act
        var result = await service.GetOutboundPingAsync(ping.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(ping.Id);
        result.Uri.Should().Be(ping.Uri);
    }

    [TestMethod]
    public async Task GetOutboundPingAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var service = CreateService();

        var nonExistentId = "nonexistent123";

        // Act
        var result = await service.GetOutboundPingAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetInboundPingAsync Tests

    [TestMethod]
    public async Task GetInboundPingAsync_ShouldReturnPing_WhenExists()
    {
        // Arrange
        var service = CreateService();

        var ping = await service.RecordInboundPingAsync(
            new Uri("https://remote.com/ping/test1"),
            new Uri("https://remote.com/users/bob"),
            new Uri("https://example.com/users/alice")
        );

        // Act
        var result = await service.GetInboundPingAsync(ping.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(ping.Id);
        result.Uri.Should().Be(ping.Uri);
    }

    [TestMethod]
    public async Task GetInboundPingAsync_ShouldReturnNull_WhenNotFound()
    {
        // Arrange
        var service = CreateService();

        var nonExistentId = "nonexistent123";

        // Act
        var result = await service.GetInboundPingAsync(nonExistentId);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetOutboundPingsAsync Tests

    [TestMethod]
    public async Task GetOutboundPingsAsync_ShouldReturnPings_OrderedByCreatedAtDescending()
    {
        // Arrange
        var service = CreateService();

        var actor = new Uri("https://example.com/users/alice");
        var to = new Uri("https://remote.com/users/bob");

        var ping1 = await service.CreateOutboundPingAsync(actor, to);
        await Task.Delay(100, TestContext.CancellationToken); // Ensure different timestamps
        var ping2 = await service.CreateOutboundPingAsync(actor, to);
        await Task.Delay(100, TestContext.CancellationToken);
        var ping3 = await service.CreateOutboundPingAsync(actor, to);

        // Act
        var result = await service.GetOutboundPingsAsync();

        // Assert
        result.Should().HaveCount(3);
        result[0].Id.Should().Be(ping3.Id); // Most recent first
        result[1].Id.Should().Be(ping2.Id);
        result[2].Id.Should().Be(ping1.Id);
    }

    [TestMethod]
    public async Task GetOutboundPingsAsync_ShouldRespectLimit()
    {
        // Arrange
        var service = CreateService();

        var actor = new Uri("https://example.com/users/alice");
        var to = new Uri("https://remote.com/users/bob");

        for (int i = 0; i < 5; i++)
        {
            await service.CreateOutboundPingAsync(actor, to);
        }

        // Act
        var result = await service.GetOutboundPingsAsync(limit: 3);

        // Assert
        result.Should().HaveCount(3);
    }

    [TestMethod]
    public async Task GetOutboundPingsAsync_ShouldReturnEmptyList_WhenNoPings()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = await service.GetOutboundPingsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region GetInboundPingsAsync Tests

    [TestMethod]
    public async Task GetInboundPingsAsync_ShouldReturnPings_OrderedByReceivedAtDescending()
    {
        // Arrange
        var service = CreateService();

        var actor = new Uri("https://remote.com/users/bob");
        var to = new Uri("https://example.com/users/alice");

        var ping1 = await service.RecordInboundPingAsync(
            new Uri("https://remote.com/ping/1"),
            actor,
            to
        );
        await Task.Delay(100, TestContext.CancellationToken); // Ensure different timestamps
        var ping2 = await service.RecordInboundPingAsync(
            new Uri("https://remote.com/ping/2"),
            actor,
            to
        );
        await Task.Delay(100, TestContext.CancellationToken);
        var ping3 = await service.RecordInboundPingAsync(
            new Uri("https://remote.com/ping/3"),
            actor,
            to
        );

        // Act
        var result = await service.GetInboundPingsAsync();

        // Assert
        result.Should().HaveCount(3);
        result[0].Id.Should().Be(ping3.Id); // Most recent first
        result[1].Id.Should().Be(ping2.Id);
        result[2].Id.Should().Be(ping1.Id);
    }

    [TestMethod]
    public async Task GetInboundPingsAsync_ShouldRespectLimit()
    {
        // Arrange
        var service = CreateService();

        var actor = new Uri("https://remote.com/users/bob");
        var to = new Uri("https://example.com/users/alice");

        for (int i = 0; i < 5; i++)
        {
            await service.RecordInboundPingAsync(
                new Uri($"https://remote.com/ping/{i}"),
                actor,
                to
            );
        }

        // Act
        var result = await service.GetInboundPingsAsync(limit: 3);

        // Assert
        result.Should().HaveCount(3);
    }

    [TestMethod]
    public async Task GetInboundPingsAsync_ShouldReturnEmptyList_WhenNoPings()
    {
        // Arrange
        var service = CreateService();

        // Act
        var result = await service.GetInboundPingsAsync();

        // Assert
        result.Should().BeEmpty();
    }

    #endregion

    #region Integration Tests

    [TestMethod]
    public async Task PingPongWorkflow_ShouldWork_EndToEnd()
    {
        // Arrange: Create an outbound ping
        var service = CreateService();

        var localActor = new Uri("https://example.com/users/alice");
        var remoteActor = new Uri("https://remote.com/users/bob");

        // Act & Assert: Create outbound ping
        var outboundPing = await service.CreateOutboundPingAsync(localActor, remoteActor);
        outboundPing.PongReceived.Should().BeFalse();

        // Act & Assert: Remote server records inbound ping
        var inboundPing = await service.RecordInboundPingAsync(
            outboundPing.Uri,
            localActor,
            remoteActor
        );
        inboundPing.PongSent.Should().BeFalse();

        // Act & Assert: Remote server creates outbound pong
        var outboundPong = await service.CreateOutboundPongAsync(
            remoteActor,
            localActor,
            outboundPing.Uri
        );
        outboundPong.PingUri.Should().Be(outboundPing.Uri);

        // Act & Assert: Remote server marks inbound ping as ponged
        await service.MarkInboundPingAsPongedAsync(inboundPing.Id);
        var updatedInboundPing = await service.GetInboundPingAsync(inboundPing.Id);
        updatedInboundPing!.PongSent.Should().BeTrue();

        // Act & Assert: Local server records inbound pong
        var inboundPong = await service.RecordInboundPongAsync(
            outboundPong.Uri,
            remoteActor,
            localActor,
            outboundPing.Uri
        );
        inboundPong.PingUri.Should().Be(outboundPing.Uri);

        // Act & Assert: Local server marks outbound ping as pong received
        await service.MarkOutboundPingAsPongReceivedAsync(outboundPing.Uri);
        var updatedOutboundPing = await service.GetOutboundPingAsync(outboundPing.Id);
        updatedOutboundPing!.PongReceived.Should().BeTrue();
        updatedOutboundPing.PongReceivedAt.Should().NotBeNull();
    }

    public TestContext TestContext { get; set; }

    #endregion
}
