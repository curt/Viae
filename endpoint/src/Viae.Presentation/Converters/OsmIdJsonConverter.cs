// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using System.Text.Json.Serialization;
using Viae.Domain.Models;

namespace Viae.Presentation.Converters;

public class OsmIdJsonConverter : JsonConverter<OsmId>
{
    public override OsmId Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            throw new JsonException("OsmId value cannot be null.");
        }

        if (reader.TokenType == JsonTokenType.StartObject)
        {
            OsmType type = default;
            ulong id = 0;
            bool hasType = false;
            bool hasId = false;

            while (reader.Read())
            {
                if (reader.TokenType == JsonTokenType.EndObject)
                {
                    return !hasType || !hasId
                        ? throw new JsonException(
                            "OsmId must have both 'Type' and 'Id' properties."
                        )
                        : new OsmId(type, id);
                }

                if (reader.TokenType == JsonTokenType.PropertyName)
                {
                    var propertyName = reader.GetString();
                    reader.Read();

                    if (string.Equals(propertyName, "Type", StringComparison.OrdinalIgnoreCase))
                    {
                        type = JsonSerializer.Deserialize<OsmType>(ref reader, options);
                        hasType = true;
                    }
                    else if (string.Equals(propertyName, "Id", StringComparison.OrdinalIgnoreCase))
                    {
                        id = reader.GetUInt64();
                        hasId = true;
                    }
                }
            }

            throw new JsonException("Unexpected end of JSON while reading OsmId.");
        }

        if (reader.TokenType == JsonTokenType.String)
        {
            var value = reader.GetString();

            return OsmId.TryParse(value, out var result)
                ? result
                : throw new JsonException($"Unable to parse '{value}' as OsmId.");
        }

        throw new JsonException(
            $"Unexpected token type {reader.TokenType} when deserializing OsmId."
        );
    }

    public override void Write(Utf8JsonWriter writer, OsmId value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("Type");
        JsonSerializer.Serialize(writer, value.Type, options);
        writer.WritePropertyName("Id");
        writer.WriteNumberValue(value.Id);
        writer.WriteEndObject();
    }
}
