// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// JSON converter factory for <see cref="LinkOr{T}"/>.
/// Creates converters that can deserialize either a URI string or an inline object.
/// </summary>
public class LinkOrJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType
            && typeToConvert.GetGenericTypeDefinition() == typeof(LinkOr<>);
    }

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var valueType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(LinkOrJsonConverter<>).MakeGenericType(valueType);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

/// <summary>
/// JSON converter for <see cref="LinkOr{T}"/>.
/// </summary>
/// <typeparam name="T">The type of the inline value.</typeparam>
public class LinkOrJsonConverter<T> : JsonConverter<LinkOr<T>>
{
    /// <inheritdoc />
    public override LinkOr<T> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        // If it's a string, treat it as a LinkRef
        if (reader.TokenType == JsonTokenType.String)
        {
            var uriString = reader.GetString();
            return string.IsNullOrWhiteSpace(uriString)
                ? throw new JsonException("URI string cannot be null or whitespace.")
                : new LinkOr<T>(new LinkRef(uriString));
        }

        // Otherwise, deserialize as T
        if (reader.TokenType is JsonTokenType.StartObject or JsonTokenType.StartArray)
        {
            var value = JsonSerializer.Deserialize<T>(ref reader, options);
            return value == null
                ? throw new JsonException($"Failed to deserialize object as {typeof(T).Name}.")
                : new LinkOr<T>(value);
        }

        throw new JsonException(
            $"Unexpected token type {reader.TokenType} when deserializing LinkOr<{typeof(T).Name}>."
        );
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer,
        LinkOr<T> value,
        JsonSerializerOptions options
    )
    {
        if (value.IsLink && value.Link.HasValue)
        {
            writer.WriteStringValue(value.Link.Value.Href.ToString());
        }
        else if (value.IsValue)
        {
            JsonSerializer.Serialize(writer, value.Value, options);
        }
        else
        {
            writer.WriteNullValue();
        }
    }
}
