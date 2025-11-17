// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using FluentAssertions;

namespace Viae.ActivityPub.Tests;

[TestClass]
public class ExtensionDataTests
{
    [TestMethod]
    public void UnknownProperties_ShouldBePreserved_InExtra()
    {
        // Arrange
        var json = File.ReadAllText("TestData/activity-with-unknown-properties.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);

        // Assert
        activity.Should().NotBeNull();
        activity.Should().BeOfType<PingActivity>();

        var ping = (PingActivity)activity!;
        ping.Extra.Should().NotBeNull();
        ping.Extra.Should().ContainKey("customProperty");
        ping.Extra.Should().ContainKey("extension");
        ping.Extra.Should().ContainKey("someArray");
    }

    [TestMethod]
    public void UnknownStringProperty_ShouldBeAccessible()
    {
        // Arrange
        var json = File.ReadAllText("TestData/activity-with-unknown-properties.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);
        var ping = (PingActivity)activity!;

        // Assert
        ping.Extra!["customProperty"].GetString().Should().Be("custom value");
    }

    [TestMethod]
    public void UnknownObjectProperty_ShouldBeAccessible()
    {
        // Arrange
        var json = File.ReadAllText("TestData/activity-with-unknown-properties.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);
        var ping = (PingActivity)activity!;

        // Assert
        var extension = ping.Extra!["extension"];
        extension.ValueKind.Should().Be(JsonValueKind.Object);
        extension.GetProperty("foo").GetString().Should().Be("bar");
        extension.GetProperty("baz").GetInt32().Should().Be(42);
    }

    [TestMethod]
    public void UnknownArrayProperty_ShouldBeAccessible()
    {
        // Arrange
        var json = File.ReadAllText("TestData/activity-with-unknown-properties.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);
        var ping = (PingActivity)activity!;

        // Assert
        var someArray = ping.Extra!["someArray"];
        someArray.ValueKind.Should().Be(JsonValueKind.Array);
        someArray.GetArrayLength().Should().Be(3);
        someArray[0].GetInt32().Should().Be(1);
        someArray[1].GetInt32().Should().Be(2);
        someArray[2].GetInt32().Should().Be(3);
    }

    [TestMethod]
    public void RoundTrip_ShouldPreserveUnknownProperties()
    {
        // Arrange
        var json = File.ReadAllText("TestData/activity-with-unknown-properties.json");
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);

        // Act
        var serialized = JsonSerializer.Serialize(activity!);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(serialized);
        var ping = (PingActivity)deserialized!;

        // Assert
        ping.Extra.Should().NotBeNull();
        ping.Extra.Should().ContainKey("customProperty");
        ping.Extra!["customProperty"].GetString().Should().Be("custom value");
        ping.Extra.Should().ContainKey("extension");
        ping.Extra.Should().ContainKey("someArray");
    }

    [TestMethod]
    public void Extra_ShouldBeMinimal_WhenNoUnknownProperties()
    {
        // Arrange
        var json = File.ReadAllText("TestData/ping-simple.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);
        var ping = (PingActivity)activity!;

        // Assert
        // Extra may contain "type" since it's used by the converter for deserialization
        // but it should not contain other unknown properties
        ping.Extra?.Keys.Should().OnlyContain(k => k == "type");
    }

    [TestMethod]
    public void AddingUnknownProperties_ShouldWork()
    {
        // Arrange
        var ping = new PingActivity
        {
            Id = new Uri("https://example.com/ping/custom"),
            Actor = new LinkOr<ActivityObject>(new Uri("https://example.com/users/test")),
            Extra = new Dictionary<string, JsonElement>
            {
                ["customField"] = JsonDocument.Parse("\"custom value\"").RootElement,
                ["customNumber"] = JsonDocument.Parse("123").RootElement,
            },
        };

        // Act
        var json = JsonSerializer.Serialize<ActivityObject>(ping);
        var deserialized = JsonSerializer.Deserialize<ActivityObject>(json) as PingActivity;

        // Assert
        deserialized.Should().NotBeNull();
        deserialized!.Extra.Should().NotBeNull();
        deserialized.Extra.Should().ContainKey("customField");
        deserialized.Extra.Should().ContainKey("customNumber");
        deserialized.Extra!["customField"].GetString().Should().Be("custom value");
        deserialized.Extra["customNumber"].GetInt32().Should().Be(123);
    }

    [TestMethod]
    public void UnknownProperties_InNestedObjects_ShouldBePreserved()
    {
        // Arrange
        var json = """
            {
                "@context": "https://www.w3.org/ns/activitystreams",
                "type": "Create",
                "id": "https://example.com/create/test",
                "actor": "https://example.com/users/test",
                "object": {
                    "type": "Note",
                    "id": "https://example.com/notes/test",
                    "content": "<p>Test</p>",
                    "customNoteField": "note custom value",
                    "nestedCustom": {
                        "x": 1,
                        "y": 2
                    }
                },
                "customCreateField": "create custom value"
            }
            """;

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json) as CreateActivity;

        // Assert
        activity.Should().NotBeNull();
        activity!.Extra.Should().NotBeNull();
        activity.Extra.Should().ContainKey("customCreateField");
        activity.Extra!["customCreateField"].GetString().Should().Be("create custom value");

        activity.Objekt!.Value.IsValue.Should().BeTrue();
        var note = activity.Objekt.Value.Value as Note;
        note.Should().NotBeNull();
        note!.Extra.Should().NotBeNull();
        note.Extra.Should().ContainKey("customNoteField");
        note.Extra!["customNoteField"].GetString().Should().Be("note custom value");
        note.Extra.Should().ContainKey("nestedCustom");
    }

    [TestMethod]
    public void KnownProperties_ShouldNotAppear_InExtra()
    {
        // Arrange
        var json = File.ReadAllText("TestData/ping-simple.json");

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json);
        var ping = (PingActivity)activity!;

        // Assert - All known properties should be in their proper places, not in Extra
        ping.Id.Should().NotBeNull();
        ping.Actor.Should().NotBeNull();
        ping.To.Should().NotBeNull();

        // Extra may contain "type" (used for deserialization) but should not contain other known properties
        if (ping.Extra != null)
        {
            ping.Extra.Should().NotContainKey("id");
            ping.Extra.Should().NotContainKey("actor");
            ping.Extra.Should().NotContainKey("to");
            ping.Extra.Should().NotContainKey("@context");
            // Note: "type" may appear in Extra as it's used by the custom converter
        }
    }

    [TestMethod]
    public void ComplexExtension_WithMultipleLevels_ShouldWork()
    {
        // Arrange
        var json = """
            {
                "@context": "https://www.w3.org/ns/activitystreams",
                "type": "Ping",
                "id": "https://example.com/ping/complex",
                "actor": "https://example.com/users/test",
                "customExtension": {
                    "level1": {
                        "level2": {
                            "level3": "deep value"
                        },
                        "array": [1, 2, 3]
                    }
                }
            }
            """;

        // Act
        var activity = JsonSerializer.Deserialize<ActivityObject>(json) as PingActivity;

        // Assert
        activity.Should().NotBeNull();
        activity!.Extra.Should().NotBeNull();
        activity.Extra.Should().ContainKey("customExtension");

        var extension = activity.Extra!["customExtension"];
        var level1 = extension.GetProperty("level1");
        var level2 = level1.GetProperty("level2");
        var level3 = level2.GetProperty("level3");
        level3.GetString().Should().Be("deep value");

        var array = level1.GetProperty("array");
        array.GetArrayLength().Should().Be(3);
    }
}
