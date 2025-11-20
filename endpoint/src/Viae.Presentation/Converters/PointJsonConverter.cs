// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using System.Text.Json.Serialization;
using NetTopologySuite.Geometries;

namespace Viae.Presentation.Converters;

/// <summary>
/// JSON converter for NetTopologySuite Point objects.
/// Serializes Point to [X, Y] array and deserializes from [X, Y] array.
/// </summary>
public class PointJsonConverter : JsonConverter<Point>
{
    public override Point? Read(
        ref Utf8JsonReader reader,
        Type typeToConvert,
        JsonSerializerOptions options
    )
    {
        if (reader.TokenType == JsonTokenType.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonTokenType.StartArray)
        {
            throw new JsonException("Expected array for Point deserialization.");
        }

        reader.Read(); // Move to first element
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException("Expected number for X coordinate.");
        }
        double x = reader.GetDouble();

        reader.Read(); // Move to second element
        if (reader.TokenType != JsonTokenType.Number)
        {
            throw new JsonException("Expected number for Y coordinate.");
        }
        double y = reader.GetDouble();

        reader.Read(); // Move to end of array
        if (reader.TokenType != JsonTokenType.EndArray)
        {
            throw new JsonException("Expected end of array after Y coordinate.");
        }

        // Create Point using default GeometryFactory
        return new Point(new Coordinate(x, y));
    }

    public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options)
    {
        if (value == null)
        {
            writer.WriteNullValue();
            return;
        }

        writer.WriteStartArray();
        writer.WriteNumberValue(value.X);
        writer.WriteNumberValue(value.Y);
        writer.WriteEndArray();
    }
}
