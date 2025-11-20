// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using System.Text.Json.Serialization;
using Viae.Domain.Models;

namespace Viae.Presentation.Converters;

public class OsmTypeJsonConverter : JsonConverter<OsmType>
{
    public override OsmType Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType != JsonTokenType.String)
        {
            throw new JsonException($"Expected string value for OsmType, got {reader.TokenType}.");
        }

        var value = reader.GetString();

        return string.IsNullOrWhiteSpace(value)
                ? throw new JsonException("OsmType value cannot be null or empty.")
            : Enum.TryParse<OsmType>(value, ignoreCase: true, out var result) ? result
            : throw new JsonException($"Unable to parse '{value}' as OsmType.");
    }

    public override void Write(Utf8JsonWriter writer, OsmType value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString().ToLowerInvariant());
    }
}
