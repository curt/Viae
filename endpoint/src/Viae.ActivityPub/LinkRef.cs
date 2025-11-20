// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.ActivityPub;

/// <summary>
/// Represents a reference to another ActivityPub object via its IRI (URI).
/// Used when an ActivityStreams property contains just a string URI rather than an inline object.
/// </summary>
public readonly struct LinkRef : IEquatable<LinkRef>
{
    /// <summary>
    /// Gets the URI of the referenced object.
    /// </summary>
    public Uri Href { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkRef"/> struct.
    /// </summary>
    /// <param name="href">The URI of the referenced object.</param>
    public LinkRef(Uri href)
    {
        Href = href ?? throw new ArgumentNullException(nameof(href));
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="LinkRef"/> struct from a string URI.
    /// </summary>
    /// <param name="href">The string URI of the referenced object.</param>
    public LinkRef(string href)
    {
        if (string.IsNullOrWhiteSpace(href))
        {
            throw new ArgumentException("URI cannot be null or whitespace.", nameof(href));
        }

        Href = new Uri(href, UriKind.Absolute);
    }

    /// <summary>
    /// Implicitly converts a <see cref="Uri"/> to a <see cref="LinkRef"/>.
    /// </summary>
    public static implicit operator LinkRef(Uri uri) => new(uri);

    /// <summary>
    /// Implicitly converts a string to a <see cref="LinkRef"/>.
    /// </summary>
    public static implicit operator LinkRef(string uri) => new(uri);

    /// <summary>
    /// Implicitly converts a <see cref="LinkRef"/> to a <see cref="Uri"/>.
    /// </summary>
    public static implicit operator Uri(LinkRef linkRef) => linkRef.Href;

    /// <inheritdoc />
    public bool Equals(LinkRef other) => Href.Equals(other.Href);

    /// <inheritdoc />
    public override bool Equals(object? obj) => obj is LinkRef other && Equals(other);

    /// <inheritdoc />
    public override int GetHashCode() => Href.GetHashCode();

    /// <inheritdoc />
    public override string ToString() => Href.ToString();

    public static bool operator ==(LinkRef left, LinkRef right) => left.Equals(right);

    public static bool operator !=(LinkRef left, LinkRef right) => !left.Equals(right);
}
