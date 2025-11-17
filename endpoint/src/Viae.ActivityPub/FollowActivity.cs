// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Represents an ActivityPub Follow activity.
/// Used when an actor wants to follow another actor.
/// </summary>
public sealed class FollowActivity : Activity
{
    /// <summary>
    /// Gets the URI of the actor being followed, if the object is a link reference.
    /// </summary>
    [JsonIgnore]
    public Uri? FollowedActorUri => Objekt?.Link?.Href;

    /// <summary>
    /// Gets the inline Person object being followed, if provided.
    /// </summary>
    [JsonIgnore]
    public Person? FollowedActorInline => Objekt?.Value as Person;
}
