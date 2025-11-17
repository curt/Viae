// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using FluentAssertions;

namespace Viae.ActivityPub.Tests;

[TestClass]
public class ObjectTests
{
    [TestMethod]
    public void Person_ShouldDeserialize_FromSimpleJson()
    {
        // Arrange
        var json = File.ReadAllText("TestData/person-simple.json");

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<Person>();

        var person = (Person)obj!;
        person.Id.Should().Be(new Uri("https://mastodon.social/@alice"));
        person.Inbox.Should().NotBeNull();
        person.Inbox!.Value.IsLink.Should().BeTrue();
        person
            .Inbox.Value.Link!.Value.Href.Should()
            .Be(new Uri("https://mastodon.social/users/alice/inbox"));
        person.Outbox.Should().NotBeNull();
        person.Outbox!.Value.IsLink.Should().BeTrue();
        person
            .Outbox.Value.Link!.Value.Href.Should()
            .Be(new Uri("https://mastodon.social/users/alice/outbox"));
    }

    [TestMethod]
    public void Person_PublicKey_ShouldDeserialize()
    {
        // Arrange
        var json = File.ReadAllText("TestData/person-simple.json");

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);
        var person = (Person)obj!;

        // Assert
        person.PublicKey.Should().NotBeNull();
        person.PublicKey!.Id.Should().Be(new Uri("https://mastodon.social/@alice#main-key"));
        person.PublicKey.Owner.Should().Be(new Uri("https://mastodon.social/@alice"));
        person.PublicKey.PublicKeyPem.Should().Contain("BEGIN PUBLIC KEY");
    }

    [TestMethod]
    public void Person_ShouldSerialize_WithRequiredProperties()
    {
        // Arrange
        var person = new Person
        {
            Id = new Uri("https://example.com/users/bob"),
            Inbox = new LinkOr<ActivityObject>(new Uri("https://example.com/users/bob/inbox")),
            Outbox = new LinkOr<ActivityObject>(new Uri("https://example.com/users/bob/outbox")),
            PublicKey = new PublicKeyInfo
            {
                Id = new Uri("https://example.com/users/bob#main-key"),
                Owner = new Uri("https://example.com/users/bob"),
                PublicKeyPem = "-----BEGIN PUBLIC KEY-----\ntest\n-----END PUBLIC KEY-----",
            },
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(person);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<Person>();

        var deserializedPerson = (Person)deserialized!;
        deserializedPerson.Id.Should().Be(person.Id);
        deserializedPerson.Inbox.Should().NotBeNull();
        deserializedPerson.Outbox.Should().NotBeNull();
        deserializedPerson.PublicKey.Should().NotBeNull();
    }

    [TestMethod]
    public void Note_ShouldDeserialize_FromSimpleJson()
    {
        // Arrange
        var json = File.ReadAllText("TestData/note-simple.json");

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        obj.Should().NotBeNull();
        obj.Should().BeOfType<Note>();

        var note = (Note)obj!;
        note.Id.Should().Be(new Uri("https://mastodon.social/users/alice/statuses/12345"));
        note.Content.Should().Contain("Hello, federated world!");
        note.Published.Should().Be(DateTime.Parse("2025-10-22T10:00:00Z").ToUniversalTime());
    }

    [TestMethod]
    public void Note_AttributedTo_ShouldDeserialize_AsLink()
    {
        // Arrange
        var json = File.ReadAllText("TestData/note-simple.json");

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);
        var note = (Note)obj!;

        // Assert
        note.AttributedTo.Should().NotBeNull();
        note.AttributedTo!.Value.IsLink.Should().BeTrue();
        note.AttributedTo.Value.Link!.Value.Href.Should()
            .Be(new Uri("https://mastodon.social/@alice"));
    }

    [TestMethod]
    public void Note_Recipients_ShouldDeserialize()
    {
        // Arrange
        var json = File.ReadAllText("TestData/note-simple.json");

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);
        var note = (Note)obj!;

        // Assert
        note.To.Should().NotBeNull();
        note.To.Should().HaveCount(1);
        note.To![0].IsLink.Should().BeTrue();
        note.To[0]
            .Link!.Value.Href.Should()
            .Be(new Uri("https://www.w3.org/ns/activitystreams#Public"));

        note.Cc.Should().NotBeNull();
        note.Cc.Should().HaveCount(1);
        note.Cc![0].IsLink.Should().BeTrue();
        note.Cc[0]
            .Link!.Value.Href.Should()
            .Be(new Uri("https://mastodon.social/users/alice/followers"));
    }

    [TestMethod]
    public void Note_ShouldSerialize()
    {
        // Arrange
        var note = new Note
        {
            Id = new Uri("https://example.com/notes/xyz"),
            Content = "<p>This is a test note.</p>",
            AttributedTo = new LinkOr<Person>(new Uri("https://example.com/users/charlie")),
            Published = DateTime.Parse("2025-10-22T15:00:00Z").ToUniversalTime(),
            To = new OneOrMany<LinkOr<ActivityObject>>(
                new LinkOr<ActivityObject>(new Uri("https://www.w3.org/ns/activitystreams#Public"))
            ),
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(note);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized.Should().BeOfType<Note>();

        var deserializedNote = (Note)deserialized!;
        deserializedNote.Id.Should().Be(note.Id);
        deserializedNote.Content.Should().Be(note.Content);
        deserializedNote.AttributedTo!.Value.IsLink.Should().BeTrue();
    }

    [TestMethod]
    public void Note_WithInlinePerson_ShouldDeserialize()
    {
        // Arrange
        var json = """
            {
                "@context": "https://www.w3.org/ns/activitystreams",
                "type": "Note",
                "id": "https://example.com/notes/123",
                "content": "<p>Test</p>",
                "attributedTo": {
                    "type": "Person",
                    "id": "https://example.com/users/dave",
                    "inbox": "https://example.com/users/dave/inbox"
                }
            }
            """;

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);
        var note = (Note)obj!;

        // Assert
        note.AttributedTo.Should().NotBeNull();
        note.AttributedTo!.Value.IsValue.Should().BeTrue();
        note.AttributedTo.Value.Value.Should().BeOfType<Person>();

        var person = note.AttributedTo.Value.Value!;
        person.Id.Should().Be(new Uri("https://example.com/users/dave"));
        person.Inbox.Should().NotBeNull();
        person.Inbox!.Value.IsLink.Should().BeTrue();
        person
            .Inbox.Value.Link!.Value.Href.Should()
            .Be(new Uri("https://example.com/users/dave/inbox"));
    }

    [TestMethod]
    public void ActivityObject_Context_ShouldDeserialize_AsString()
    {
        // Arrange
        var json = File.ReadAllText("TestData/note-simple.json");

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        obj.Should().NotBeNull();
        obj!.Context.Should().NotBeNull();

        // Context is deserialized as a JsonElement, check its string value
        var contextElement = (JsonElement)obj.Context!;
        contextElement.GetString().Should().Be("https://www.w3.org/ns/activitystreams");
    }

    [TestMethod]
    public void ActivityObject_Context_ShouldDeserialize_AsArray()
    {
        // Arrange
        var json = File.ReadAllText("TestData/person-simple.json");

        // Act
        var obj = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        obj.Should().NotBeNull();
        obj!.Context.Should().NotBeNull();

        // Context can be a complex object/array - just verify it deserializes
        var contextElement = (JsonElement)obj.Context!;
        contextElement.ValueKind.Should().Be(JsonValueKind.Array);
    }

    [TestMethod]
    public void PublicKeyInfo_ShouldSerialize_AndDeserialize()
    {
        // Arrange
        var publicKey = new PublicKeyInfo
        {
            Id = new Uri("https://example.com/users/eve#main-key"),
            Owner = new Uri("https://example.com/users/eve"),
            PublicKeyPem = "-----BEGIN PUBLIC KEY-----\nMIIB...\n-----END PUBLIC KEY-----",
        };

        // Act
        var json = JsonSerializer.Serialize(publicKey);
        var deserialized = JsonSerializer.Deserialize<PublicKeyInfo>(json);

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be(publicKey.Id);
        deserialized.Owner.Should().Be(publicKey.Owner);
        deserialized.PublicKeyPem.Should().Be(publicKey.PublicKeyPem);
    }
}
