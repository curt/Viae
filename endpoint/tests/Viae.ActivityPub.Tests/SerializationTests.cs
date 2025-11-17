// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using FluentAssertions;

namespace Viae.ActivityPub.Tests;

[TestClass]
public class SerializationTests
{
    [TestMethod]
    public void RoundTrip_Ping_ShouldPreserveAllData()
    {
        // Arrange
        var original = new PingActivity
        {
            Context = "https://www.w3.org/ns/activitystreams",
            Id = new Uri("https://example.com/ping/roundtrip"),
            Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/alice")),
            To = new OneOrMany<LinkOr<ActivityObject>>([
                new LinkOr<ActivityObject>(new Uri("https://example.com/users/bob")),
                new LinkOr<ActivityObject>(new Uri("https://example.com/users/charlie")),
            ]),
            Published = DateTime.Parse("2025-10-22T16:00:00Z").ToUniversalTime(),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(original);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json) as PingActivity;

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(original.Id);
        deserialized.ActorUri.Should().Be(new Uri("https://example.com/users/alice"));
        deserialized.To.Should().HaveCount(2);
        deserialized.ToUris.Should().HaveCount(2);
        deserialized.Published.Should().Be(original.Published);
    }

    [TestMethod]
    public void RoundTrip_Pong_ShouldPreserveAllData()
    {
        // Arrange
        var original = new PongActivity
        {
            Context = "https://www.w3.org/ns/activitystreams",
            Id = new Uri("https://example.com/pong/roundtrip"),
            Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/bob")),
            To = new OneOrMany<LinkOr<ActivityObject>>(
                new LinkOr<ActivityObject>(new Uri("https://example.com/users/alice"))
            ),
            Objekt = new LinkOr<ActivityObject>(new Uri("https://example.com/ping/original")),
            Published = DateTime.Parse("2025-10-22T16:05:00Z").ToUniversalTime(),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(original);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json) as PongActivity;

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(original.Id);
        deserialized.ActorUri.Should().Be(new Uri("https://example.com/users/bob"));
        deserialized.PingUri.Should().Be(new Uri("https://example.com/ping/original"));
        deserialized.Published.Should().Be(original.Published);
    }

    [TestMethod]
    public void RoundTrip_CreateWithInlineNote_ShouldPreserveAllData()
    {
        // Arrange
        var note = new Note
        {
            Id = new Uri("https://example.com/notes/roundtrip"),
            Content = "<p>Round trip test content</p>",
            AttributedTo = new LinkOr<Person>(new Uri("https://example.com/users/dave")),
            Published = DateTime.Parse("2025-10-22T16:10:00Z").ToUniversalTime(),
            To = new OneOrMany<LinkOr<ActivityObject>>(
                new LinkOr<ActivityObject>(new Uri("https://www.w3.org/ns/activitystreams#Public"))
            ),
        };

        var original = new CreateActivity
        {
            Context = "https://www.w3.org/ns/activitystreams",
            Id = new Uri("https://example.com/activities/roundtrip"),
            Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/dave")),
            Objekt = new LinkOr<ActivityObject>(note),
            Published = DateTime.Parse("2025-10-22T16:10:00Z").ToUniversalTime(),
            To = new OneOrMany<LinkOr<ActivityObject>>(
                new LinkOr<ActivityObject>(new Uri("https://www.w3.org/ns/activitystreams#Public"))
            ),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(original);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json) as CreateActivity;

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(original.Id);
        deserialized.Objekt.Should().NotBeNull();
        deserialized.Objekt!.Value.IsValue.Should().BeTrue();
        deserialized.Objekt.Value.Value.Should().BeOfType<Note>();

        var deserializedNote = (Note)deserialized.Objekt.Value.Value!;
        deserializedNote.Content.Should().Be(note.Content);
        deserializedNote.Id.Should().Be(note.Id);
    }

    [TestMethod]
    public void RoundTrip_Person_ShouldPreserveAllData()
    {
        // Arrange
        var original = new Person
        {
            Context = "https://www.w3.org/ns/activitystreams",
            Id = new Uri("https://example.com/users/roundtrip"),
            Inbox = new LinkOr<ActivityObject>(
                new Uri("https://example.com/users/roundtrip/inbox")
            ),
            Outbox = new LinkOr<ActivityObject>(
                new Uri("https://example.com/users/roundtrip/outbox")
            ),
            PublicKey = new PublicKeyInfo
            {
                Id = new Uri("https://example.com/users/roundtrip#main-key"),
                Owner = new Uri("https://example.com/users/roundtrip"),
                PublicKeyPem = "-----BEGIN PUBLIC KEY-----\nMIIBIjAN...\n-----END PUBLIC KEY-----",
            },
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(original);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json) as Person;

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(original.Id);
        deserialized.Inbox.Should().NotBeNull();
        deserialized.Outbox.Should().NotBeNull();
        deserialized.PublicKey.Should().NotBeNull();
        deserialized.PublicKey!.Id.Should().Be(original.PublicKey.Id);
        deserialized.PublicKey.PublicKeyPem.Should().Be(original.PublicKey.PublicKeyPem);
    }

    [TestMethod]
    public void RoundTrip_Follow_ShouldPreserveAllData()
    {
        // Arrange
        var original = new FollowActivity
        {
            Context = "https://www.w3.org/ns/activitystreams",
            Id = new Uri("https://example.com/follows/roundtrip"),
            Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/eve")),
            Objekt = new LinkOr<ActivityObject>(new Uri("https://example.com/users/frank")),
            Published = DateTime.Parse("2025-10-22T16:15:00Z").ToUniversalTime(),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(original);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json) as FollowActivity;

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(original.Id);
        deserialized.Actor.Should().NotBeNull();
        deserialized.Actor!.Value.IsLink.Should().BeTrue();
        deserialized
            .Actor.Value.Link!.Value.Href.Should()
            .Be(new Uri("https://example.com/users/eve"));
        deserialized.Objekt!.Value.IsLink.Should().BeTrue();
        deserialized
            .Objekt.Value.Link!.Value.Href.Should()
            .Be(new Uri("https://example.com/users/frank"));
    }

    [TestMethod]
    public void RoundTrip_Like_ShouldPreserveAllData()
    {
        // Arrange
        var original = new LikeActivity
        {
            Context = "https://www.w3.org/ns/activitystreams",
            Id = new Uri("https://example.com/likes/roundtrip"),
            Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/grace")),
            Objekt = new LinkOr<ActivityObject>(new Uri("https://example.com/notes/456")),
            Published = DateTime.Parse("2025-10-22T16:20:00Z").ToUniversalTime(),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(original);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json) as LikeActivity;

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(original.Id);
        deserialized.Actor.Should().NotBeNull();
        deserialized.Actor!.Value.IsLink.Should().BeTrue();
        deserialized
            .Actor.Value.Link!.Value.Href.Should()
            .Be(new Uri("https://example.com/users/grace"));
        deserialized.Objekt!.Value.IsLink.Should().BeTrue();
    }

    [TestMethod]
    public void RoundTrip_ComplexCreate_WithMultipleRecipients()
    {
        // Arrange
        var original = new CreateActivity
        {
            Context = "https://www.w3.org/ns/activitystreams",
            Id = new Uri("https://example.com/activities/complex"),
            Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/henry")),
            Objekt = new LinkOr<ActivityObject>(new Uri("https://example.com/notes/789")),
            Published = DateTime.Parse("2025-10-22T16:25:00Z").ToUniversalTime(),
            To = new OneOrMany<LinkOr<ActivityObject>>([
                new LinkOr<ActivityObject>(new Uri("https://www.w3.org/ns/activitystreams#Public")),
                new LinkOr<ActivityObject>(new Uri("https://example.com/users/iris")),
            ]),
            Cc = new OneOrMany<LinkOr<ActivityObject>>([
                new LinkOr<ActivityObject>(new Uri("https://example.com/users/henry/followers")),
                new LinkOr<ActivityObject>(new Uri("https://example.com/users/jack")),
            ]),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(original);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json) as CreateActivity;

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.To.Should().HaveCount(2);
        deserialized.Cc.Should().HaveCount(2);
        deserialized.To![0].IsLink.Should().BeTrue();
        deserialized.To[1].IsLink.Should().BeTrue();
        deserialized.Cc![0].IsLink.Should().BeTrue();
        deserialized.Cc[1].IsLink.Should().BeTrue();
    }

    [TestMethod]
    public void TypeDiscriminator_ShouldBePresent_InSerializedJson()
    {
        // Arrange
        var ping = new PingActivity
        {
            Id = new Uri("https://example.com/ping/test"),
            Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/test")),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(ping);
        var jsonElement = JsonSerializer.Deserialize<JsonElement>(json);

        // Assert
        jsonElement.TryGetProperty("type", out var typeProperty).Should().BeTrue();
        typeProperty.GetString().Should().Be("Ping");
    }

    [TestMethod]
    public void MultipleTypes_ShouldDeserialize_ToCorrectTypes()
    {
        // Arrange
        var activities = new List<ActivityObject>
        {
            new PingActivity
            {
                Id = new Uri("https://example.com/ping/1"),
                Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/a")),
            },
            new CreateActivity
            {
                Id = new Uri("https://example.com/create/1"),
                Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/b")),
            },
            new FollowActivity
            {
                Id = new Uri("https://example.com/follow/1"),
                Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/c")),
            },
        };

        // Act
        var json = JsonSerializer.Serialize(activities);
        var deserialized = JsonSerializer.Deserialize<List<ActivityObject>>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().HaveCount(3);
        deserialized![0].Should().BeOfType<PingActivity>();
        deserialized[1].Should().BeOfType<CreateActivity>();
        deserialized[2].Should().BeOfType<FollowActivity>();
    }
}
