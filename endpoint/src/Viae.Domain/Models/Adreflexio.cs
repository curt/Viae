// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// Outbound ActivityPub pongs sent to remote instances.
/// </summary>
public class Adreflexio : IdentifiableBase, IUriIdentifiable
{
    /// <summary>
    /// Gets or sets the ActivityPub activity URI for this pong.
    /// This is the 'id' field in the Pong activity sent to the remote instance.
    /// Auto-generated in the service layer based on the entity's ID.
    /// </summary>
    public Uri Uri { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ActivityPub actor URI sending this pong.
    /// This is the 'actor' field in the Pong activity.
    /// </summary>
    public Uri Actor { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ActivityPub actor URI this pong is being sent to.
    /// This is the 'to' field in the Pong activity.
    /// </summary>
    public Uri To { get; set; } = null!;

    /// <summary>
    /// Gets or sets the URI of the original Ping that this Pong is responding to.
    /// This is the 'object' field in the Pong activity.
    /// </summary>
    public Uri PingUri { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when this pong was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this pong was sent.
    /// </summary>
    public DateTime? SentAt { get; set; }
}
