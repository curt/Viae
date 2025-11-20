// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// Inbound ActivityPub pings received from remote instances.
/// </summary>
public class Abmissio : IdentifiableBase
{
    /// <summary>
    /// Gets or sets the ActivityPub activity URI for this ping.
    /// This is the 'id' field from the received Ping activity.
    /// </summary>
    public Uri Uri { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ActivityPub actor URI who sent this ping.
    /// This is the 'actor' field from the received Ping activity.
    /// </summary>
    public Uri Actor { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ActivityPub actor URI this ping was sent to.
    /// This is the 'to' field from the received Ping activity.
    /// </summary>
    public Uri To { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when this ping was received.
    /// </summary>
    public DateTime ReceivedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a Pong has been sent in response.
    /// </summary>
    public bool PongSent { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the Pong was sent, if applicable.
    /// </summary>
    public DateTime? PongSentAt { get; set; }
}
