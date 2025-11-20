// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Represents an ActivityPub Note object (a short text post).
/// </summary>
public sealed class Note : ActivityObject
{
    /// <summary>
    /// Gets or sets the HTML content of this note.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the actor who created this note.
    /// </summary>
    [JsonPropertyName("attributedTo")]
    public LinkOr<Person>? AttributedTo { get; set; }

    /// <summary>
    /// Gets or sets the primary recipients of this note.
    /// </summary>
    [JsonPropertyName("to")]
    public OneOrMany<LinkOr<ActivityObject>>? To { get; set; }

    /// <summary>
    /// Gets or sets the secondary recipients (carbon copy) of this note.
    /// </summary>
    [JsonPropertyName("cc")]
    public OneOrMany<LinkOr<ActivityObject>>? Cc { get; set; }

    /// <summary>
    /// Gets or sets when this note was published.
    /// </summary>
    [JsonPropertyName("published")]
    public DateTimeOffset? Published { get; set; }

    /// <summary>
    /// Gets or sets where this note happened.
    /// Used for vestigia.
    /// </summary>
    [JsonPropertyName("location")]
    public OneOrMany<LinkOr<ActivityObject>>? Location { get; set; }

    /// <summary>
    /// Gets or sets when this note started.
    /// Used for vestigia.
    /// </summary>
    [JsonPropertyName("startTime")]
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// Gets or sets when this note ended.
    /// Used for vestigia.
    /// </summary>
    [JsonPropertyName("endTime")]
    public DateTimeOffset? EndedAt { get; set; }
}
