// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using FluentAssertions;

namespace Viae.ActivityPub.Tests;

[TestClass]
public class PingPongTests
{
    [TestMethod]
    public void Ping_ShouldDeserialize_FromSimpleJson()
    {
        // Arrange
        var json = File.ReadAllText("TestData/ping-simple.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        activity.Should().NotBeNull();
        activity.Should().BeOfType<PingActivity>();

        var ping = (PingActivity)activity!;
        ping.Id.Should().Be(new Uri("https://example.com/ping/abc123"));
        ping.ActorUri.Should().Be(new Uri("https://example.com/users/alice"));
        ping.ToUri.Should().Be(new Uri("https://social.example/users/bob"));
    }

    [TestMethod]
    public void Ping_ShouldDeserialize_WithMultipleRecipients()
    {
        // Arrange
        var json = File.ReadAllText("TestData/ping-with-multiple-recipients.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        activity.Should().NotBeNull();
        activity.Should().BeOfType<PingActivity>();

        var ping = (PingActivity)activity!;
        ping.Id.Should().Be(new Uri("https://example.com/ping/xyz789"));
        ping.ActorUri.Should().Be(new Uri("https://example.com/users/charlie"));
        ping.To.Should().NotBeNull();
        ping.To.Should().HaveCount(3);
        ping.ToUris.Should().HaveCount(3);
        ping.ToUris.Should().Contain(new Uri("https://social.example/users/alice"));
        ping.ToUris.Should().Contain(new Uri("https://social.example/users/bob"));
        ping.ToUris.Should().Contain(new Uri("https://mastodon.social/users/dave"));
    }

    [TestMethod]
    public void Ping_ActorObject_ShouldBeNull_WhenActorIsLink()
    {
        // Arrange
        var json = File.ReadAllText("TestData/ping-simple.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);
        var ping = (PingActivity)activity!;

        // Assert
        ping.ActorObject.Should().BeNull();
        ping.ActorUri.Should().NotBeNull();
    }

    [TestMethod]
    public void Pong_ShouldDeserialize_FromSimpleJson()
    {
        // Arrange
        var json = File.ReadAllText("TestData/pong-simple.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        activity.Should().NotBeNull();
        activity.Should().BeOfType<PongActivity>();

        var pong = (PongActivity)activity!;
        pong.Id.Should().Be(new Uri("https://social.example/pong/def456"));
        pong.ActorUri.Should().Be(new Uri("https://social.example/users/bob"));
        pong.ToUri.Should().Be(new Uri("https://example.com/users/alice"));
        pong.PingUri.Should().Be(new Uri("https://example.com/ping/abc123"));
        pong.Published.Should().Be(DateTime.Parse("2025-10-22T14:25:15Z").ToUniversalTime());
    }

    [TestMethod]
    public void Pong_PingUri_ShouldExtract_FromObjectProperty()
    {
        // Arrange
        var json = File.ReadAllText("TestData/pong-simple.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);
        var pong = (PongActivity)activity!;

        // Assert
        pong.Objekt.Should().NotBeNull();
        pong.Objekt!.Value.IsLink.Should().BeTrue();
        pong.PingUri.Should().Be(new Uri("https://example.com/ping/abc123"));
    }

    [TestMethod]
    public void Ping_ShouldSerialize_ToValidJson()
    {
        // Arrange
        var ping = new PingActivity
        {
            Id = new Uri("https://example.com/ping/test123"),
            Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/alice")),
            To = new OneOrMany<LinkOr<ActivityObject>>(
                new LinkOr<ActivityObject>(new Uri("https://social.example/users/bob"))
            ),
            Published = DateTime.Parse("2025-10-22T10:00:00Z").ToUniversalTime(),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(ping);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<PingActivity>();

        var deserializedPing = (PingActivity)deserialized!;
        deserializedPing.Id.Should().Be(ping.Id);
        deserializedPing.ActorUri.Should().Be(new Uri("https://example.com/users/alice"));
        deserializedPing.ToUri.Should().Be(new Uri("https://social.example/users/bob"));
    }

    [TestMethod]
    public void Pong_ShouldSerialize_ToValidJson()
    {
        // Arrange
        var pong = new PongActivity
        {
            Id = new Uri("https://social.example/pong/test789"),
            Actor = new LinkOr<ActivityObject>(new Uri("https://social.example/users/bob")),
            To = new OneOrMany<LinkOr<ActivityObject>>(
                new LinkOr<ActivityObject>(new Uri("https://example.com/users/alice"))
            ),
            Objekt = new LinkOr<ActivityObject>(new Uri("https://example.com/ping/abc123")),
            Published = DateTime.Parse("2025-10-22T11:00:00Z").ToUniversalTime(),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(pong);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<PongActivity>();

        var deserializedPong = (PongActivity)deserialized!;
        deserializedPong.Id.Should().Be(pong.Id);
        deserializedPong.ActorUri.Should().Be(new Uri("https://social.example/users/bob"));
        deserializedPong.ToUri.Should().Be(new Uri("https://example.com/users/alice"));
        deserializedPong.PingUri.Should().Be(new Uri("https://example.com/ping/abc123"));
    }

    [TestMethod]
    public void Ping_Context_ShouldDeserialize()
    {
        // Arrange
        var json = File.ReadAllText("TestData/ping-simple.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);
        var ping = (PingActivity)activity!;

        // Assert
        ping.Context.Should().NotBeNull();

        // Context is deserialized as a JsonElement, check its string value
        var contextElement = (JsonElement)ping.Context!;
        contextElement.GetString().Should().Be("https://www.w3.org/ns/activitystreams");
    }
}
