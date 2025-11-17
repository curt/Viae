// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Represents a union type that can be either a <see cref="LinkRef"/> (IRI) or an inline object of type <typeparamref name="T"/>.
/// This handles the ActivityStreams pattern where properties can be either a URI string or a full object.
/// </summary>
/// <typeparam name="T">The type of the inline object.</typeparam>
[JsonConverter(typeof(LinkOrJsonConverterFactory))]
public readonly struct LinkOr<T>
{
    private readonly object? _value;

    /// <summary>
    /// Gets a value indicating whether this instance contains a link reference.
    /// </summary>
    [JsonIgnore]
    public bool IsLink => _value is LinkRef;

    /// <summary>
    /// Gets a value indicating whether this instance contains an inline value.
    /// </summary>
    [JsonIgnore]
    public bool IsValue => _value is T;

    /// <summary>
    /// Gets the link reference, or null if this instance contains an inline value.
    /// </summary>
    [JsonIgnore]
    public LinkRef? Link => _value as LinkRef?;

    /// <summary>
    /// Gets the inline value, or default if this instance contains a link reference.
    /// </summary>
    [JsonIgnore]
    public T? Value => _value is T t ? t : default;

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkOr{T}"/> struct with a link reference.
    /// </summary>
    public LinkOr(LinkRef link)
    {
        _value = link;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkOr{T}"/> struct with an inline value.
    /// </summary>
    public LinkOr(T value)
    {
        _value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    /// Implicitly converts a <see cref="LinkRef"/> to a <see cref="LinkOr{T}"/>.
    /// </summary>
    public static implicit operator LinkOr<T>(LinkRef link) => new(link);

    /// <summary>
    /// Implicitly converts a <typeparamref name="T"/> to a <see cref="LinkOr{T}"/>.
    /// </summary>
    public static implicit operator LinkOr<T>(T value) => new(value);

    /// <summary>
    /// Implicitly converts a <see cref="Uri"/> to a <see cref="LinkOr{T}"/>.
    /// </summary>
    public static implicit operator LinkOr<T>(Uri uri) => new(new LinkRef(uri));

    /// <summary>
    /// Implicitly converts a string URI to a <see cref="LinkOr{T}"/>.
    /// </summary>
    public static implicit operator LinkOr<T>(string uri) => new(new LinkRef(uri));

    /// <inheritdoc />
    public override string ToString() =>
        _value switch
        {
            LinkRef link => link.ToString(),
            T value => value.ToString() ?? string.Empty,
            _ => string.Empty,
        };
}
