// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Represents an ActivityPub Create activity.
/// Used when an actor creates a new object (e.g., a Note, Article, etc.).
/// </summary>
public sealed class CreateActivity : Activity
{
    /// <summary>
    /// Gets the object as a Note, if it's an inline Note.
    /// </summary>
    [JsonIgnore]
    public Note? ObjectAsNote => Objekt?.Value as Note;

    /// <summary>
    /// Gets the object URI, if it's a link reference.
    /// </summary>
    [JsonIgnore]
    public Uri? ObjectUri => Objekt?.Link?.Href;
}
