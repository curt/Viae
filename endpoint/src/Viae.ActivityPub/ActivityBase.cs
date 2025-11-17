// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Base class for ActivityPub activities (as opposed to objects).
/// Activities represent actions taken by actors, such as Create, Follow, Like, Ping, Pong, etc.
/// </summary>
public abstract class Activity : ActivityObject
{
    /// <summary>
    /// Gets or sets the actor who performed this activity.
    /// Can be either a URI reference or an inline object.
    /// </summary>
    [JsonPropertyName("actor")]
    public LinkOr<ActivityObject>? Actor { get; set; }

    /// <summary>
    /// Gets or sets the object of this activity.
    /// Can be either a URI reference or an inline object.
    /// </summary>
    [JsonPropertyName("object")]
    public LinkOr<ActivityObject>? Objekt { get; set; }

    /// <summary>
    /// Gets or sets the target of this activity.
    /// Can be either a URI reference or an inline object.
    /// </summary>
    [JsonPropertyName("target")]
    public LinkOr<ActivityObject>? Target { get; set; }

    /// <summary>
    /// Gets or sets the primary recipients of this activity.
    /// Can be a single value or an array, and each can be a URI or inline object.
    /// </summary>
    [JsonPropertyName("to")]
    public OneOrMany<LinkOr<ActivityObject>>? To { get; set; }

    /// <summary>
    /// Gets or sets the secondary recipients (carbon copy) of this activity.
    /// Can be a single value or an array, and each can be a URI or inline object.
    /// </summary>
    [JsonPropertyName("cc")]
    public OneOrMany<LinkOr<ActivityObject>>? Cc { get; set; }

    /// <summary>
    /// Gets or sets when this activity was published.
    /// </summary>
    [JsonPropertyName("published")]
    public DateTimeOffset? Published { get; set; }
}
