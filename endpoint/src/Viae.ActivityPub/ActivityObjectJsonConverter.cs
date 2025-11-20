// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// JSON converter for <see cref="ActivityObject"/> that handles polymorphic deserialization
/// regardless of property order in the JSON document.
/// This is necessary because System.Text.Json's built-in polymorphic deserialization requires
/// the type discriminator to be the first property, but real ActivityPub JSON has @context first.
/// </summary>
public class ActivityObjectJsonConverter : JsonConverter<ActivityObject>
{
    /// <summary>
    /// Maps ActivityPub type names to their corresponding .NET types.
    /// This must be kept in sync with the derived types list in the ActivityObject XML documentation.
    /// </summary>
    private static readonly Dictionary<string, Type> TypeMappings = new(
        StringComparer.OrdinalIgnoreCase
    )
    {
        ["Ping"] = typeof(PingActivity),
        ["Pong"] = typeof(PongActivity),
        ["Person"] = typeof(Person),
        ["Place"] = typeof(Place),
        ["Note"] = typeof(Note),
        ["Create"] = typeof(CreateActivity),
        ["Follow"] = typeof(FollowActivity),
        ["Like"] = typeof(LikeActivity),
        ["Collection"] = typeof(Collection),
        ["OrderedCollection"] = typeof(OrderedCollection),
        ["CollectionPage"] = typeof(CollectionPage),
        ["OrderedCollectionPage"] = typeof(OrderedCollectionPage),
    };

    /// <summary>
    /// Reverse mapping from .NET types to ActivityPub type names for serialization.
    /// </summary>
    private static readonly Dictionary<Type, string> ReverseTypeMappings =
        TypeMappings.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);

    /// <inheritdoc />
    public override ActivityObject? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        // Read the entire JSON object into a JsonDocument so we can inspect it
        using var doc = JsonDocument.ParseValue(ref reader);
        var root = doc.RootElement;

        // Extract the type discriminator property
        if (!root.TryGetProperty("type", out var typeElement))
        {
            throw new JsonException(
                "ActivityPub object is missing required 'type' property. "
                    + "All ActivityPub objects must include a 'type' field specifying the object type."
            );
        }

        var typeName = typeElement.GetString();
        if (string.IsNullOrEmpty(typeName))
        {
            throw new JsonException(
                "ActivityPub 'type' property cannot be null or empty. "
                    + "It must be a valid ActivityStreams type name (e.g., 'Note', 'Create', 'Person')."
            );
        }

        // Look up the corresponding .NET type
        if (!TypeMappings.TryGetValue(typeName, out var targetType))
        {
            throw new JsonException(
                $"Unknown ActivityPub type '{typeName}'. "
                    + $"Supported types: {string.Join(", ", TypeMappings.Keys)}. "
                    + "If this is a valid ActivityPub type that should be supported, "
                    + "add it to the TypeMappings dictionary in ActivityObjectJsonConverter."
            );
        }

        // Deserialize to the specific derived type
        // We use the raw JSON text to avoid double-parsing
        var rawJson = root.GetRawText();
        return (ActivityObject?)JsonSerializer.Deserialize(rawJson, targetType, options);
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer,
        ActivityObject value,
        JsonSerializerOptions options
    )
    {
        // Get the ActivityPub type name for this object
        var concreteType = value.GetType();
        if (!ReverseTypeMappings.TryGetValue(concreteType, out var typeName))
        {
            throw new JsonException(
                $"Cannot serialize type '{concreteType.Name}' as ActivityObject. "
                    + "The type is not registered in the ActivityObjectJsonConverter type mappings."
            );
        }

        // Create new options without this converter to avoid infinite recursion
        var optionsWithoutConverter = new JsonSerializerOptions(options);
        optionsWithoutConverter.Converters.Clear();
        foreach (var converter in options.Converters)
        {
            if (converter is not ActivityObjectJsonConverter)
            {
                optionsWithoutConverter.Converters.Add(converter);
            }
        }

        // Serialize the object and manually inject the "type" discriminator
        // We need to serialize to a JsonDocument first, then add the type property
        var json = JsonSerializer.Serialize(value, concreteType, optionsWithoutConverter);
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        // Check if we're nested BEFORE writing the start object
        // At the root level, CurrentDepth is 0. Inside a nested object, it's > 0.
        var isNested = writer.CurrentDepth > 0;

        writer.WriteStartObject();

        // Write @context first (only for root-level objects)
        // This is standard practice in ActivityPub/JSON-LD
        if (!isNested)
        {
            // Find and write @context property if it exists
            foreach (var property in root.EnumerateObject())
            {
                if (property.Name == "@context")
                {
                    property.WriteTo(writer);
                    break;
                }
            }
        }

        // Write the type discriminator second (standard practice, though our deserializer handles any order)
        writer.WriteString("type", typeName);

        // Write all other properties in alphabetical order, excluding @context and type
        // We skip @context for nested objects per ActivityPub spec
        var properties = root.EnumerateObject()
            .Where(p => p.Name is not "@context" and not "type")
            .OrderBy(p => p.Name, StringComparer.Ordinal);

        foreach (var property in properties)
        {
            property.WriteTo(writer);
        }

        writer.WriteEndObject();
    }

    /// <inheritdoc />
    public override ActivityObject ReadAsPropertyName(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        // ActivityObject should never be used as a property name
        throw new NotSupportedException(
            "ActivityObject cannot be deserialized from a property name. "
                + "ActivityPub objects should only appear as JSON object values, not as keys."
        );
    }

    /// <inheritdoc />
    public override void WriteAsPropertyName(
        Utf8JsonWriter writer,
        ActivityObject value,
        JsonSerializerOptions options
    )
    {
        // ActivityObject should never be used as a property name
        throw new NotSupportedException(
            "ActivityObject cannot be serialized as a property name. "
                + "ActivityPub objects should only appear as JSON object values, not as keys."
        );
    }
}
