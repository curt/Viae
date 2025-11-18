// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Services;

/// <summary>
/// Service for discovering ActivityPub actor information from remote instances.
/// </summary>
public interface IActivityPubDiscoveryService
{
    /// <summary>
    /// Discovers the inbox URL for a given ActivityPub actor URI.
    /// </summary>
    /// <param name="actorUri">The ActivityPub actor URI to discover the inbox for.</param>
    /// <returns>The inbox URL, or null if discovery fails.</returns>
    Task<Uri?> DiscoverInboxAsync(Uri actorUri);
}
