// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Domain.Tests.Models;

[TestClass]
public class ActivityPubEntitiesTests
{
    [TestMethod]
    public void Admissio_ShouldBeCreated_WithRequiredProperties()
    {
        // Arrange
        var uri = new Uri("https://example.com/ping/123");
        var actor = new Uri("https://local.example.com/users/alice");
        var to = new Uri("https://remote.example.com/users/bob");

        // Act
        var admissio = new Admissio
        {
            Id = Guid.NewGuid().ToString(),
            Uri = uri,
            Actor = actor,
            To = to,
            CreatedAt = DateTime.UtcNow,
        };

        // Assert
        admissio.Id.Should().NotBeNullOrEmpty();
        admissio.Uri.Should().Be(uri);
        admissio.Actor.Should().Be(actor);
        admissio.To.Should().Be(to);
        admissio.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void Admissio_SentAt_ShouldBeNull_Initially()
    {
        // Arrange & Act
        var admissio = new Admissio
        {
            Id = Guid.NewGuid().ToString(),
            Uri = new Uri("https://example.com/ping/123"),
            Actor = new Uri("https://actor.example.com"),
            To = new Uri("https://recipient.example.com"),
            CreatedAt = DateTime.UtcNow,
        };

        // Assert
        admissio.SentAt.Should().BeNull();
    }

    [TestMethod]
    public void Admissio_PongReceived_ShouldBeFalse_Initially()
    {
        // Arrange & Act
        var admissio = new Admissio
        {
            Id = Guid.NewGuid().ToString(),
            Uri = new Uri("https://example.com/ping/123"),
            Actor = new Uri("https://actor.example.com"),
            To = new Uri("https://recipient.example.com"),
            CreatedAt = DateTime.UtcNow,
        };

        // Assert
        admissio.PongReceived.Should().BeFalse();
        admissio.PongReceivedAt.Should().BeNull();
    }

    [TestMethod]
    public void Admissio_CanTrackPongReceipt()
    {
        // Arrange
        var admissio = new Admissio
        {
            Id = Guid.NewGuid().ToString(),
            Uri = new Uri("https://example.com/ping/123"),
            Actor = new Uri("https://actor.example.com"),
            To = new Uri("https://recipient.example.com"),
            CreatedAt = DateTime.UtcNow,
            // Act - Simulate receiving pong
            PongReceived = true,
            PongReceivedAt = DateTime.UtcNow,
        };

        // Assert
        admissio.PongReceived.Should().BeTrue();
        admissio.PongReceivedAt.Should().NotBeNull();
        admissio.PongReceivedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void Abmissio_ShouldBeCreated_WithRequiredProperties()
    {
        // Arrange
        var uri = new Uri("https://remote.example.com/ping/456");
        var actor = new Uri("https://remote.example.com/users/bob");
        var to = new Uri("https://local.example.com/users/alice");

        // Act
        var abmissio = new Abmissio
        {
            Id = Guid.NewGuid().ToString(),
            Uri = uri,
            Actor = actor,
            To = to,
            ReceivedAt = DateTime.UtcNow,
        };

        // Assert
        abmissio.Id.Should().NotBeNullOrEmpty();
        abmissio.Uri.Should().Be(uri);
        abmissio.Actor.Should().Be(actor);
        abmissio.To.Should().Be(to);
        abmissio.ReceivedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void Abmissio_PongSent_ShouldBeFalse_Initially()
    {
        // Arrange & Act
        var abmissio = new Abmissio
        {
            Id = Guid.NewGuid().ToString(),
            Uri = new Uri("https://example.com/ping/456"),
            Actor = new Uri("https://actor.example.com"),
            To = new Uri("https://recipient.example.com"),
            ReceivedAt = DateTime.UtcNow,
        };

        // Assert
        abmissio.PongSent.Should().BeFalse();
        abmissio.PongSentAt.Should().BeNull();
    }

    [TestMethod]
    public void Abmissio_CanTrackPongSent()
    {
        // Arrange
        var abmissio = new Abmissio
        {
            Id = Guid.NewGuid().ToString(),
            Uri = new Uri("https://example.com/ping/456"),
            Actor = new Uri("https://actor.example.com"),
            To = new Uri("https://recipient.example.com"),
            ReceivedAt = DateTime.UtcNow,
            // Act - Simulate sending pong
            PongSent = true,
            PongSentAt = DateTime.UtcNow,
        };

        // Assert
        abmissio.PongSent.Should().BeTrue();
        abmissio.PongSentAt.Should().NotBeNull();
        abmissio.PongSentAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void Adreflexio_ShouldBeCreated_WithRequiredProperties()
    {
        // Arrange
        var uri = new Uri("https://local.example.com/pong/789");
        var actor = new Uri("https://local.example.com/users/alice");
        var to = new Uri("https://remote.example.com/users/bob");
        var pingUri = new Uri("https://remote.example.com/ping/456");

        // Act
        var adreflexio = new Adreflexio
        {
            Id = Guid.NewGuid().ToString(),
            Uri = uri,
            Actor = actor,
            To = to,
            PingUri = pingUri,
            CreatedAt = DateTime.UtcNow,
        };

        // Assert
        adreflexio.Id.Should().NotBeNullOrEmpty();
        adreflexio.Uri.Should().Be(uri);
        adreflexio.Actor.Should().Be(actor);
        adreflexio.To.Should().Be(to);
        adreflexio.PingUri.Should().Be(pingUri);
        adreflexio.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void Adreflexio_SentAt_ShouldBeNull_Initially()
    {
        // Arrange & Act
        var adreflexio = new Adreflexio
        {
            Id = Guid.NewGuid().ToString(),
            Uri = new Uri("https://example.com/pong/789"),
            Actor = new Uri("https://actor.example.com"),
            To = new Uri("https://recipient.example.com"),
            PingUri = new Uri("https://example.com/ping/456"),
            CreatedAt = DateTime.UtcNow,
        };

        // Assert
        adreflexio.SentAt.Should().BeNull();
    }

    [TestMethod]
    public void Abreflexio_ShouldBeCreated_WithRequiredProperties()
    {
        // Arrange
        var uri = new Uri("https://remote.example.com/pong/999");
        var actor = new Uri("https://remote.example.com/users/bob");
        var to = new Uri("https://local.example.com/users/alice");
        var pingUri = new Uri("https://local.example.com/ping/123");

        // Act
        var abreflexio = new Abreflexio
        {
            Id = Guid.NewGuid().ToString(),
            Uri = uri,
            Actor = actor,
            To = to,
            PingUri = pingUri,
            ReceivedAt = DateTime.UtcNow,
        };

        // Assert
        abreflexio.Id.Should().NotBeNullOrEmpty();
        abreflexio.Uri.Should().Be(uri);
        abreflexio.Actor.Should().Be(actor);
        abreflexio.To.Should().Be(to);
        abreflexio.PingUri.Should().Be(pingUri);
        abreflexio.ReceivedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void PingPongFlow_Outbound_ShouldTrackCompleteLifecycle()
    {
        // Arrange - Create outbound ping
        var admissio = new Admissio
        {
            Id = Guid.NewGuid().ToString(),
            Uri = new Uri("https://local.example.com/ping/123"),
            Actor = new Uri("https://local.example.com/users/alice"),
            To = new Uri("https://remote.example.com/users/bob"),
            CreatedAt = DateTime.UtcNow,
            // Act - Send the ping
            SentAt = DateTime.UtcNow,
        };

        // Act - Receive pong response
        var abreflexio = new Abreflexio
        {
            Id = Guid.NewGuid().ToString(),
            Uri = new Uri("https://remote.example.com/pong/999"),
            Actor = new Uri("https://remote.example.com/users/bob"),
            To = new Uri("https://local.example.com/users/alice"),
            PingUri = admissio.Uri,
            ReceivedAt = DateTime.UtcNow,
        };

        admissio.PongReceived = true;
        admissio.PongReceivedAt = DateTime.UtcNow;

        // Assert
        admissio.SentAt.Should().NotBeNull();
        admissio.PongReceived.Should().BeTrue();
        admissio.PongReceivedAt.Should().NotBeNull();
        abreflexio.PingUri.Should().Be(admissio.Uri);
    }
}
