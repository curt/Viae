// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// Represents a strongly typed identifier for an OpenStreetMap element.
/// </summary>
public readonly struct OsmId(OsmType type, ulong id) : IEquatable<OsmId>, IComparable<OsmId>
{
    /// <summary>
    /// Gets the element type component of the identifier.
    /// </summary>
    public OsmType Type { get; } = type;

    /// <summary>
    /// Gets the numeric identifier assigned by OpenStreetMap.
    /// </summary>
    public ulong Id { get; } = id;

    /// <summary>
    /// Returns the canonical <c>type/id</c> string representation used by Overpass queries.
    /// </summary>
    public override string ToString() => $"{Type.ToString().ToLowerInvariant()}/{Id}";

    /// <summary>
    /// Attempts to parse a <c>type/id</c> string into an <see cref="OsmId"/> value.
    /// </summary>
    /// <param name="input">The string representation to parse.</param>
    /// <param name="result">When the method returns, contains the parsed identifier or the default value on failure.</param>
    /// <returns><c>true</c> when parsing succeeds; otherwise, <c>false</c>.</returns>
    public static bool TryParse(string? input, out OsmId result)
    {
        result = default;

        if (string.IsNullOrWhiteSpace(input))
        {
            return false;
        }

        var parts = input.Split('/');

        if (parts.Length != 2)
        {
            return false;
        }

        if (!Enum.TryParse<OsmType>(parts[0], ignoreCase: true, out var type))
        {
            return false;
        }

        if (!ulong.TryParse(parts[1], out var id))
        {
            return false;
        }

        result = new OsmId(type, id);

        return true;
    }

    /// <summary>
    /// Returns a hash code for the current identifier.
    /// </summary>
    public override int GetHashCode() => HashCode.Combine(Type, Id);

    /// <summary>
    /// Determines whether the specified object is equal to the current identifier.
    /// </summary>
    public override bool Equals(object? obj) => obj is OsmId other && Equals(other);

    /// <summary>
    /// Determines whether the specified identifier equals the current identifier.
    /// </summary>
    /// <param name="other">The identifier to compare with the current instance.</param>
    /// <returns><c>true</c> when both identifiers match; otherwise, <c>false</c>.</returns>
    public bool Equals(OsmId other) => Type == other.Type && Id == other.Id;

    /// <summary>
    /// Compares the current identifier with another for sorting purposes.
    /// </summary>
    /// <param name="other">The identifier to compare.</param>
    /// <returns>A value less than zero when this identifier precedes <paramref name="other"/>; zero when equal; otherwise a value greater than zero.</returns>
    public int CompareTo(OsmId other)
    {
        var typeCmp = Type.CompareTo(other.Type);

        return typeCmp != 0 ? typeCmp : Id.CompareTo(other.Id);
    }

    /// <summary>
    /// Determines whether two identifiers are equal.
    /// </summary>
    public static bool operator ==(OsmId left, OsmId right) => left.Equals(right);

    /// <summary>
    /// Determines whether two identifiers are not equal.
    /// </summary>
    public static bool operator !=(OsmId left, OsmId right) => !(left == right);

    public static bool operator <(OsmId left, OsmId right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator <=(OsmId left, OsmId right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >(OsmId left, OsmId right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator >=(OsmId left, OsmId right)
    {
        return left.CompareTo(right) >= 0;
    }
}
