// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Services;

/// <summary>
/// Service interface for managing ActivityPub Ping and Pong activities (Missio/Reflexio).
/// Handles sending pings, receiving pings, and automatically responding with pongs.
/// </summary>
public interface IMissioService
{
    /// <summary>
    /// Creates a new outbound ping to be sent to a remote instance.
    /// </summary>
    /// <param name="actor">The ActivityPub actor URI sending the ping.</param>
    /// <param name="toUri">The ActivityPub actor URI this ping is being sent to.</param>
    /// <returns>The created Admissio entity.</returns>
    Task<Admissio> CreateOutboundPingAsync(Uri actor, Uri toUri);

    /// <summary>
    /// Records an inbound ping received from a remote instance.
    /// </summary>
    /// <param name="uri">The ActivityPub activity URI from the ping's 'id' field.</param>
    /// <param name="actor">The ActivityPub actor URI who sent the ping.</param>
    /// <param name="toUri">The ActivityPub actor URI this ping was sent to.</param>
    /// <returns>The created Abmissio entity.</returns>
    Task<Abmissio> RecordInboundPingAsync(Uri uri, Uri actor, Uri toUri);

    /// <summary>
    /// Creates a new outbound pong in response to a received ping.
    /// </summary>
    /// <param name="actor">The ActivityPub actor URI sending the pong.</param>
    /// <param name="toUri">The ActivityPub actor URI this pong is being sent to.</param>
    /// <param name="pingUri">The URI of the original ping being responded to.</param>
    /// <returns>The created Adreflexio entity.</returns>
    Task<Adreflexio> CreateOutboundPongAsync(Uri actor, Uri toUri, Uri pingUri);

    /// <summary>
    /// Records an inbound pong received from a remote instance.
    /// </summary>
    /// <param name="uri">The ActivityPub activity URI from the pong's 'id' field.</param>
    /// <param name="actor">The ActivityPub actor URI who sent the pong.</param>
    /// <param name="toUri">The ActivityPub actor URI this pong was sent to.</param>
    /// <param name="pingUri">The URI of the original ping being responded to.</param>
    /// <returns>The created Abreflexio entity.</returns>
    Task<Abreflexio> RecordInboundPongAsync(Uri uri, Uri actor, Uri toUri, Uri pingUri);

    /// <summary>
    /// Marks an inbound ping as having been responded to with a pong.
    /// </summary>
    /// <param name="pingId">The database ID of the inbound ping.</param>
    Task MarkInboundPingAsPongedAsync(string pingId);

    /// <summary>
    /// Marks an outbound ping as having received a pong response.
    /// </summary>
    /// <param name="pingUri">The ActivityPub activity URI of the outbound ping.</param>
    Task MarkOutboundPingAsPongReceivedAsync(Uri pingUri);

    /// <summary>
    /// Gets an outbound ping by its database ID.
    /// </summary>
    Task<Admissio?> GetOutboundPingAsync(string id);

    /// <summary>
    /// Gets an inbound ping by its database ID.
    /// </summary>
    Task<Abmissio?> GetInboundPingAsync(string id);

    /// <summary>
    /// Gets all outbound pings, ordered by creation date descending.
    /// </summary>
    Task<List<Admissio>> GetOutboundPingsAsync(int limit = 100);

    /// <summary>
    /// Gets all inbound pings, ordered by received date descending.
    /// </summary>
    Task<List<Abmissio>> GetInboundPingsAsync(int limit = 100);
}
