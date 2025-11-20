// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using FluentAssertions;

namespace Viae.ActivityPub.Tests;

/// <summary>
/// Tests to verify that ActivityObject deserialization works regardless of JSON property order.
/// This is critical for ActivityPub compatibility since real ActivityPub JSON has @context first,
/// but System.Text.Json's built-in polymorphic deserialization requires type first.
/// </summary>
[TestClass]
public class PropertyOrderTests
{
    [TestMethod]
    public void Deserialization_ShouldWork_WhenTypeIsFirst()
    {
        // Arrange - type discriminator before @context
        var json = """
            {
              "type": "Ping",
              "@context": "https://www.w3.org/ns/activitystreams",
              "id": "https://example.com/ping/1",
              "actor": "https://example.com/users/alice",
              "to": "https://example.com/users/bob"
            }
            """;

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        activity.Should().NotBeNull();
        activity.Should().BeOfType<PingActivity>();

        var ping = (PingActivity)activity!;
        ping.Id.Should().Be(new Uri("https://example.com/ping/1"));
        ping.ActorUri.Should().Be(new Uri("https://example.com/users/alice"));
        ping.ToUri.Should().Be(new Uri("https://example.com/users/bob"));
    }

    [TestMethod]
    public void Deserialization_ShouldWork_WhenContextIsFirst()
    {
        // Arrange - @context before type discriminator (real ActivityPub order)
        var json = """
            {
              "@context": "https://www.w3.org/ns/activitystreams",
              "type": "Ping",
              "id": "https://example.com/ping/2",
              "actor": "https://example.com/users/charlie",
              "to": "https://example.com/users/dave"
            }
            """;

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        activity.Should().NotBeNull();
        activity.Should().BeOfType<PingActivity>();

        var ping = (PingActivity)activity!;
        ping.Id.Should().Be(new Uri("https://example.com/ping/2"));
        ping.ActorUri.Should().Be(new Uri("https://example.com/users/charlie"));
        ping.ToUri.Should().Be(new Uri("https://example.com/users/dave"));
    }

    [TestMethod]
    public void Deserialization_ShouldWork_WhenTypeIsLast()
    {
        // Arrange - type discriminator at the end
        var json = """
            {
              "@context": "https://www.w3.org/ns/activitystreams",
              "id": "https://example.com/ping/3",
              "actor": "https://example.com/users/eve",
              "to": "https://example.com/users/frank",
              "type": "Ping"
            }
            """;

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        activity.Should().NotBeNull();
        activity.Should().BeOfType<PingActivity>();

        var ping = (PingActivity)activity!;
        ping.Id.Should().Be(new Uri("https://example.com/ping/3"));
        ping.ActorUri.Should().Be(new Uri("https://example.com/users/eve"));
        ping.ToUri.Should().Be(new Uri("https://example.com/users/frank"));
    }

    [TestMethod]
    public void Deserialization_ShouldWork_ForAllActivityTypes()
    {
        // Test that property order independence works for all activity types

        // Ping
        var pingJson = """{"@context": "...", "type": "Ping", "id": "https://example.com/1"}""";
        JsonSerializer.Deserialize<ActivityObject>(pingJson).Should().BeOfType<PingActivity>();

        // Pong
        var pongJson = """{"@context": "...", "type": "Pong", "id": "https://example.com/2"}""";
        JsonSerializer.Deserialize<ActivityObject>(pongJson).Should().BeOfType<PongActivity>();

        // Create
        var createJson = """{"@context": "...", "type": "Create", "id": "https://example.com/3"}""";
        JsonSerializer.Deserialize<ActivityObject>(createJson).Should().BeOfType<CreateActivity>();

        // Follow
        var followJson = """{"@context": "...", "type": "Follow", "id": "https://example.com/4"}""";
        JsonSerializer.Deserialize<ActivityObject>(followJson).Should().BeOfType<FollowActivity>();

        // Like
        var likeJson = """{"@context": "...", "type": "Like", "id": "https://example.com/5"}""";
        JsonSerializer.Deserialize<ActivityObject>(likeJson).Should().BeOfType<LikeActivity>();

        // Person
        var personJson = """{"@context": "...", "type": "Person", "id": "https://example.com/6"}""";
        JsonSerializer.Deserialize<ActivityObject>(personJson).Should().BeOfType<Person>();

        // Note
        var noteJson = """{"@context": "...", "type": "Note", "id": "https://example.com/7"}""";
        JsonSerializer.Deserialize<ActivityObject>(noteJson).Should().BeOfType<Note>();
    }

    [TestMethod]
    public void Deserialization_ShouldThrow_WhenTypeIsMissing()
    {
        // Arrange - no type discriminator
        var json = """
            {
              "@context": "https://www.w3.org/ns/activitystreams",
              "id": "https://example.com/ping/4",
              "actor": "https://example.com/users/grace"
            }
            """;

        // Act & Assert
        var act = () => JsonSerializer.Deserialize<ActivityObject>(json);

        act.Should().Throw<JsonException>().WithMessage("*missing required 'type' property*");
    }

    [TestMethod]
    public void Deserialization_ShouldThrow_WhenTypeIsEmpty()
    {
        // Arrange - empty type
        var json = """
            {
              "type": "",
              "@context": "https://www.w3.org/ns/activitystreams",
              "id": "https://example.com/ping/5"
            }
            """;

        // Act & Assert
        var act = () => JsonSerializer.Deserialize<ActivityObject>(json);

        act.Should()
            .Throw<JsonException>()
            .WithMessage("*'type' property cannot be null or empty*");
    }

    [TestMethod]
    public void Deserialization_ShouldThrow_WhenTypeIsUnknown()
    {
        // Arrange - unknown type
        var json = """
            {
              "type": "UnknownActivityType",
              "@context": "https://www.w3.org/ns/activitystreams",
              "id": "https://example.com/unknown/1"
            }
            """;

        // Act & Assert
        var act = () => JsonSerializer.Deserialize<ActivityObject>(json);

        act.Should()
            .Throw<JsonException>()
            .WithMessage("*Unknown ActivityPub type 'UnknownActivityType'*")
            .And.Message.Should()
            .Contain("Supported types:");
    }

    [TestMethod]
    public void Deserialization_ShouldBeCaseInsensitive_ForType()
    {
        // Arrange - lowercase "ping" instead of "Ping"
        var json = """
            {
              "@context": "https://www.w3.org/ns/activitystreams",
              "type": "ping",
              "id": "https://example.com/ping/6",
              "actor": "https://example.com/users/henry"
            }
            """;

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        activity.Should().NotBeNull();
        activity.Should().BeOfType<PingActivity>();
    }

    [TestMethod]
    public void Serialization_ShouldIncludeTypeDiscriminator()
    {
        // Arrange
        var ping = new PingActivity
        {
            Id = new Uri("https://example.com/ping/7"),
            Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/iris")),
            To = new OneOrMany<LinkOr<ActivityObject>>(
                new LinkOr<ActivityObject>(new Uri("https://example.com/users/jack"))
            ),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(ping);
        var jsonDoc = JsonDocument.Parse(json);

        // Assert
        jsonDoc.RootElement.TryGetProperty("type", out var typeProperty).Should().BeTrue();
        typeProperty.GetString().Should().Be("Ping");
    }

    [TestMethod]
    public void RoundTrip_ShouldPreserveType_RegardlessOfPropertyOrder()
    {
        // Arrange - original has @context first (real ActivityPub order)
        var originalJson = """
            {
              "@context": "https://www.w3.org/ns/activitystreams",
              "type": "Create",
              "id": "https://example.com/create/1",
              "actor": "https://example.com/users/kate",
              "object": "https://example.com/notes/1"
            }
            """;

        // Act - deserialize and re-serialize
        var activity = JsonSerializer.Deserialize<ActivityObject>(originalJson);
        var roundTripJson = JsonSerializer.Serialize(activity!);
        var roundTripActivity = JsonSerializer.Deserialize<ActivityObject>(roundTripJson);

        // Assert - type is preserved through the round trip
        activity.Should().BeOfType<CreateActivity>();
        roundTripActivity.Should().BeOfType<CreateActivity>();

        var original = (CreateActivity)activity!;
        var roundTrip = (CreateActivity)roundTripActivity!;

        roundTrip.Id.Should().Be(original.Id);
    }
}
