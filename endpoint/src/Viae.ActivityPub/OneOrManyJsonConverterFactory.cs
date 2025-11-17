// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// JSON converter factory for <see cref="OneOrMany{T}"/>.
/// Creates converters that can deserialize either a single value or an array.
/// </summary>
public class OneOrManyJsonConverterFactory : JsonConverterFactory
{
    /// <inheritdoc />
    public override bool CanConvert(Type typeToConvert)
    {
        return typeToConvert.IsGenericType
            && typeToConvert.GetGenericTypeDefinition() == typeof(OneOrMany<>);
    }

    /// <inheritdoc />
    public override JsonConverter? CreateConverter(
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        var itemType = typeToConvert.GetGenericArguments()[0];
        var converterType = typeof(OneOrManyJsonConverter<>).MakeGenericType(itemType);
        return (JsonConverter?)Activator.CreateInstance(converterType);
    }
}

/// <summary>
/// JSON converter for <see cref="OneOrMany{T}"/>.
/// </summary>
/// <typeparam name="T">The type of items in the collection.</typeparam>
public class OneOrManyJsonConverter<T> : JsonConverter<OneOrMany<T>>
{
    /// <inheritdoc />
    public override OneOrMany<T> Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        // If it's an array, deserialize normally
        if (reader.TokenType == JsonTokenType.StartArray)
        {
            var list = JsonSerializer.Deserialize<List<T>>(ref reader, options);
            return [.. list ?? []];
        }

        // If it's a single value, wrap it in a list
        var item = JsonSerializer.Deserialize<T>(ref reader, options);
        return item == null ? [] : new OneOrMany<T>(item);
    }

    /// <inheritdoc />
    public override void Write(
        Utf8JsonWriter writer,
        OneOrMany<T> value,
        JsonSerializerOptions options
    )
    {
        if (value == null || value.Count == 0)
        {
            writer.WriteNullValue();
            return;
        }

        // Always serialize as an array for consistency
        JsonSerializer.Serialize(writer, (List<T>)value, options);
    }
}
