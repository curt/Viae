// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// Outbound ActivityPub pings sent to remote instances.
/// </summary>
public class Admissio : IdentifiableBase, IUriIdentifiable
{
    /// <summary>
    /// Gets or sets the ActivityPub activity URI for this ping.
    /// This is the 'id' field in the Ping activity sent to the remote instance.
    /// Auto-generated in the service layer based on the entity's ID.
    /// </summary>
    public Uri Uri { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ActivityPub actor URI sending this ping.
    /// This is the 'actor' field in the Ping activity.
    /// </summary>
    public Uri Actor { get; set; } = null!;

    /// <summary>
    /// Gets or sets the ActivityPub actor URI this ping is being sent to.
    /// This is the 'to' field in the Ping activity.
    /// </summary>
    public Uri To { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when this ping was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this ping was sent.
    /// </summary>
    public DateTime? SentAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether a Pong has been received in response.
    /// </summary>
    public bool PongReceived { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the Pong was received, if applicable.
    /// </summary>
    public DateTime? PongReceivedAt { get; set; }
}
