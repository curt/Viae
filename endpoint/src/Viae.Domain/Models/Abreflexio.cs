// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// Inbound ActivityPub pongs received from remote instances.
/// </summary>
public class Abreflexio : IdentifiableBase
{
    /// <summary>
    /// Gets or sets the ActivityPub activity URI for this pong.
    /// This is the 'id' field from the received Pong activity.
    /// </summary>
    public Uri Uri { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ActivityPub actor URI who sent this pong.
    /// This is the 'actor' field from the received Pong activity.
    /// </summary>
    public Uri Actor { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ActivityPub actor URI this pong was sent to.
    /// This is the 'to' field from the received Pong activity.
    /// </summary>
    public Uri To { get; set; } = null!;

    /// <summary>
    /// Gets or sets the URI of the original Ping that this Pong is responding to.
    /// This is the 'object' field from the received Pong activity.
    /// </summary>
    public Uri PingUri { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when this pong was received.
    /// </summary>
    public DateTime ReceivedAt { get; set; }
}
