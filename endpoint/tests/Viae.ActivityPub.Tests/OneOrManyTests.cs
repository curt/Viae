// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using FluentAssertions;

namespace Viae.ActivityPub.Tests;

[TestClass]
public class OneOrManyTests
{
    [TestMethod]
    public void Constructor_ShouldCreateEmpty()
    {
        // Act
        var collection = new OneOrMany<string>();

        // Assert
        collection.Should().BeEmpty();
    }

    [TestMethod]
    public void Constructor_ShouldCreateWithSingleItem()
    {
        // Arrange
        var item = "test";

        // Act
        var collection = new OneOrMany<string>(item);

        // Assert
        collection.Should().HaveCount(1);
        collection[0].Should().Be(item);
    }

    [TestMethod]
    public void Constructor_ShouldCreateWithMultipleItems()
    {
        // Arrange
        var items = new[] { "one", "two", "three" };

        // Act
        var collection = new OneOrMany<string>(items);

        // Assert
        collection.Should().HaveCount(3);
        collection.Should().ContainInOrder(items);
    }

    [TestMethod]
    public void ImplicitConversion_FromSingleItem_ShouldWork()
    {
        // Arrange
        var item = "single";

        // Act
        OneOrMany<string> collection = item;

        // Assert
        collection.Should().HaveCount(1);
        collection[0].Should().Be(item);
    }

    [TestMethod]
    public void ImplicitConversion_FromArray_ShouldWork()
    {
        // Arrange
        var items = new[] { "a", "b", "c" };

        // Act
        OneOrMany<string> collection = items;

        // Assert
        collection.Should().HaveCount(3);
        collection.Should().ContainInOrder(items);
    }

    [TestMethod]
    public void Deserialization_ShouldHandleSingleValue()
    {
        // Arrange
        var json = "\"single-value\"";

        // Act
        var collection = JsonSerializer.Deserialize<OneOrMany<string>>(json);

        // Assert
        collection.Should().NotBeNull();
        collection.Should().HaveCount(1);
        collection![0].Should().Be("single-value");
    }

    [TestMethod]
    public void Deserialization_ShouldHandleArray()
    {
        // Arrange
        var json = "[\"first\", \"second\", \"third\"]";

        // Act
        var collection = JsonSerializer.Deserialize<OneOrMany<string>>(json);

        // Assert
        collection.Should().NotBeNull();
        collection.Should().HaveCount(3);
        collection!.Should().ContainInOrder("first", "second", "third");
    }

    [TestMethod]
    public void Deserialization_ShouldHandleSingleUri()
    {
        // Arrange
        var json = "\"https://example.com/single\"";

        // Act
        var collection = JsonSerializer.Deserialize<OneOrMany<Uri>>(json);

        // Assert
        collection.Should().NotBeNull();
        collection.Should().HaveCount(1);
        collection![0].Should().Be(new Uri("https://example.com/single"));
    }

    [TestMethod]
    public void Deserialization_ShouldHandleUriArray()
    {
        // Arrange
        var json = "[\"https://example.com/one\", \"https://example.com/two\"]";

        // Act
        var collection = JsonSerializer.Deserialize<OneOrMany<Uri>>(json);

        // Assert
        collection.Should().NotBeNull();
        collection.Should().HaveCount(2);
        collection![0].Should().Be(new Uri("https://example.com/one"));
        collection[1].Should().Be(new Uri("https://example.com/two"));
    }

    [TestMethod]
    public void Serialization_ShouldSerializeSingleItem_AsArray()
    {
        // Arrange
        var collection = new OneOrMany<string>("only-one");

        // Act
        var json = JsonSerializer.Serialize(collection);

        // Assert
        // Note: For consistency, OneOrMany always serializes as an array
        json.Should().Be("[\"only-one\"]");
    }

    [TestMethod]
    public void Serialization_ShouldSerializeMultipleItems_AsArray()
    {
        // Arrange
        var collection = new OneOrMany<string>(["first", "second"]);

        // Act
        var json = JsonSerializer.Serialize(collection);

        // Assert
        json.Should().Be("[\"first\",\"second\"]");
    }

    [TestMethod]
    public void Serialization_ShouldSerializeEmpty_AsNull()
    {
        // Arrange
        var collection = new OneOrMany<string>();

        // Act
        var json = JsonSerializer.Serialize(collection);

        // Assert
        // Note: Empty OneOrMany serializes as null
        json.Should().Be("null");
    }

    [TestMethod]
    public void WithLinkOr_ShouldDeserializeSingleLink()
    {
        // Arrange
        var json = "\"https://example.com/actor\"";

        // Act
        var collection = JsonSerializer.Deserialize<OneOrMany<LinkOr<ActivityObject>>>(json);

        // Assert
        collection.Should().NotBeNull();
        collection.Should().HaveCount(1);
        collection![0].IsLink.Should().BeTrue();
        collection[0].Link!.Value.Href.Should().Be(new Uri("https://example.com/actor"));
    }

    [TestMethod]
    public void WithLinkOr_ShouldDeserializeArrayOfLinks()
    {
        // Arrange
        var json = "[\"https://example.com/one\", \"https://example.com/two\"]";

        // Act
        var collection = JsonSerializer.Deserialize<OneOrMany<LinkOr<ActivityObject>>>(json);

        // Assert
        collection.Should().NotBeNull();
        collection.Should().HaveCount(2);
        collection![0].IsLink.Should().BeTrue();
        collection[0].Link!.Value.Href.Should().Be(new Uri("https://example.com/one"));
        collection[1].IsLink.Should().BeTrue();
        collection[1].Link!.Value.Href.Should().Be(new Uri("https://example.com/two"));
    }

    [TestMethod]
    public void ListOperations_ShouldWork()
    {
        // Arrange
        var collection = new OneOrMany<string>
        {
            // Act
            "first",
            "second",
            "third",
        };

        // Assert
        collection.Should().HaveCount(3);
        collection.Should().ContainInOrder("first", "second", "third");
        collection.Contains("second").Should().BeTrue();
        collection.IndexOf("third").Should().Be(2);
    }

    [TestMethod]
    public void FirstOrDefault_ShouldReturnFirstItem_WhenExists()
    {
        // Arrange
        var collection = new OneOrMany<string>(["alpha", "beta", "gamma"]);

        // Act
        var first = collection.FirstOrDefault();

        // Assert
        first.Should().Be("alpha");
    }

    [TestMethod]
    public void FirstOrDefault_ShouldReturnDefault_WhenEmpty()
    {
        // Arrange
        var collection = new OneOrMany<string>();

        // Act
        var first = collection.FirstOrDefault();

        // Assert
        first.Should().BeNull();
    }
}
