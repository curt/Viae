// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Represents an ActivityPub Like activity.
/// Used when an actor likes an object (e.g., a Note, Article, etc.).
/// </summary>
public sealed class LikeActivity : Activity
{
    /// <summary>
    /// Gets the URI of the object being liked, if it's a link reference.
    /// </summary>
    [JsonIgnore]
    public Uri? LikedObjectUri => Objekt?.Link?.Href;

    /// <summary>
    /// Gets the inline object being liked, if provided.
    /// </summary>
    [JsonIgnore]
    public ActivityObject? LikedInline => Objekt?.Value;
}
