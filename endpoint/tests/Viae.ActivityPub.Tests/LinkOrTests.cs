// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using FluentAssertions;

namespace Viae.ActivityPub.Tests;

[TestClass]
public class LinkOrTests
{
    [TestMethod]
    public void Constructor_ShouldCreateLinkOr_WithLink()
    {
        // Arrange
        var uri = new Uri("https://example.com/users/alice");

        // Act
        var linkOr = new LinkOr<Person>(uri);

        // Assert
        linkOr.IsLink.Should().BeTrue();
        linkOr.IsValue.Should().BeFalse();
        linkOr.Link.Should().NotBeNull();
        linkOr.Link!.Value.Href.Should().Be(uri);
        linkOr.Value.Should().BeNull();
    }

    [TestMethod]
    public void Constructor_ShouldCreateLinkOr_WithValue()
    {
        // Arrange
        var person = new Person
        {
            Id = new Uri("https://example.com/users/bob"),
            Inbox = new Uri("https://example.com/users/bob/inbox"),
        };

        // Act
        var linkOr = new LinkOr<Person>(person);

        // Assert
        linkOr.IsLink.Should().BeFalse();
        linkOr.IsValue.Should().BeTrue();
        linkOr.Value.Should().NotBeNull();
        linkOr.Value!.Id.Should().Be(person.Id);
        linkOr.Link.Should().BeNull();
    }

    [TestMethod]
    public void ImplicitConversion_FromUri_ShouldCreateLink()
    {
        // Arrange
        var uri = new Uri("https://example.com/users/charlie");

        // Act
        LinkOr<Person> linkOr = uri;

        // Assert
        linkOr.IsLink.Should().BeTrue();
        linkOr.Link!.Value.Href.Should().Be(uri);
    }

    [TestMethod]
    public void ImplicitConversion_FromValue_ShouldCreateValue()
    {
        // Arrange
        var person = new Person
        {
            Id = new Uri("https://example.com/users/dave"),
            Inbox = new Uri("https://example.com/users/dave/inbox"),
        };

        // Act
        LinkOr<Person> linkOr = person;

        // Assert
        linkOr.IsValue.Should().BeTrue();
        linkOr.Value.Should().Be(person);
    }

    [TestMethod]
    public void Deserialization_ShouldHandleStringAsLink()
    {
        // Arrange
        var json = "\"https://example.com/users/eve\"";

        // Act
        var linkOr = JsonSerializer.Deserialize<LinkOr<Person>>(json);

        // Assert
        linkOr.IsLink.Should().BeTrue();
        linkOr.Link!.Value.Href.Should().Be(new Uri("https://example.com/users/eve"));
    }

    [TestMethod]
    public void Deserialization_ShouldHandleObjectAsValue()
    {
        // Arrange
        var json = """
            {
                "type": "Person",
                "id": "https://example.com/users/frank",
                "inbox": "https://example.com/users/frank/inbox"
            }
            """;

        // Act
        var linkOr = JsonSerializer.Deserialize<LinkOr<Person>>(json);

        // Assert
        linkOr.IsValue.Should().BeTrue();
        linkOr.Value.Should().NotBeNull();
        linkOr.Value!.Id.Should().Be(new Uri("https://example.com/users/frank"));
    }

    [TestMethod]
    public void Serialization_ShouldSerializeLink_AsString()
    {
        // Arrange
        var linkOr = new LinkOr<Person>(new Uri("https://example.com/users/grace"));

        // Act
        var json = JsonSerializer.Serialize(linkOr);

        // Assert
        json.Should().Be("\"https://example.com/users/grace\"");
    }

    [TestMethod]
    public void Serialization_ShouldSerializeValue_AsObject()
    {
        // Arrange
        var person = new Person
        {
            Id = new Uri("https://example.com/users/henry"),
            Inbox = new LinkOr<ActivityObject>(new Uri("https://example.com/users/henry/inbox")),
        };
        var linkOr = new LinkOr<ActivityObject>(person);

        // Act
        var json = JsonSerializer.Serialize(linkOr);
        var deserialized = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json);

        // Assert
        deserialized.Should().ContainKey("type");
        deserialized.Should().ContainKey("id");
        deserialized!["type"].GetString().Should().Be("Person");
        deserialized["id"].GetString().Should().Be("https://example.com/users/henry");
    }

    [TestMethod]
    public void Equality_ShouldWork_ForLinks()
    {
        // Arrange
        var uri = new Uri("https://example.com/test");
        var linkOr1 = new LinkOr<Person>(uri);
        var linkOr2 = new LinkOr<Person>(uri);

        // Act & Assert
        linkOr1.Should().Be(linkOr2);
    }

    [TestMethod]
    public void Equality_ShouldFail_ForDifferentLinks()
    {
        // Arrange
        var linkOr1 = new LinkOr<Person>(new Uri("https://example.com/a"));
        var linkOr2 = new LinkOr<Person>(new Uri("https://example.com/b"));

        // Act & Assert
        linkOr1.Should().NotBe(linkOr2);
    }

    [TestMethod]
    public void NullableLinkOr_ShouldSupportHasValue()
    {
        // Arrange
        LinkOr<Person>? linkOrWithLink = new LinkOr<Person>(
            new Uri("https://example.com/users/iris")
        );
        var person = new Person
        {
            Id = new Uri("https://example.com/users/jack"),
            Inbox = new LinkOr<ActivityObject>(new Uri("https://example.com/users/jack/inbox")),
        };
        LinkOr<Person>? linkOrWithValue = new LinkOr<Person>(person);
        LinkOr<Person>? nullLinkOr = null;

        // Act & Assert
        linkOrWithLink.HasValue.Should().BeTrue();
        linkOrWithValue.HasValue.Should().BeTrue();
        nullLinkOr.HasValue.Should().BeFalse();
    }
}
