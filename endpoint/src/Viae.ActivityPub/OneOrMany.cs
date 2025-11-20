// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Represents a collection that can be deserialized from either a single value or an array.
/// This handles the ActivityStreams pattern where properties like "to" or "cc" can be a single string or an array.
/// </summary>
/// <typeparam name="T">The type of elements in the collection.</typeparam>
[JsonConverter(typeof(OneOrManyJsonConverterFactory))]
public sealed class OneOrMany<T> : List<T>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="OneOrMany{T}"/> class.
    /// </summary>
    public OneOrMany() { }

    /// <summary>
    /// Initializes a new instance of the <see cref="OneOrMany{T}"/> class with a single item.
    /// </summary>
    public OneOrMany(T item)
    {
        Add(item);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OneOrMany{T}"/> class with multiple items.
    /// </summary>
    public OneOrMany(IEnumerable<T> items)
        : base(items) { }

    /// <summary>
    /// Implicitly converts a single item to a <see cref="OneOrMany{T}"/>.
    /// </summary>
    public static implicit operator OneOrMany<T>(T item) => new(item);

    /// <summary>
    /// Implicitly converts an array to a <see cref="OneOrMany{T}"/>.
    /// </summary>
    public static implicit operator OneOrMany<T>(T[] items) => [.. items];
}
