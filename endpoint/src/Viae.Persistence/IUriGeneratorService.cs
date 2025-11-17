// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Persistence;

/// <summary>
/// Service for generating URIs for entities based on their type and ID.
/// This is used by the URI value generator to create ActivityPub-compliant URIs.
/// </summary>
public interface IUriGeneratorService
{
    /// <summary>
    /// Generates a URI for an outbound ping (Admissio).
    /// </summary>
    Uri GenerateUri(Admissio entity);

    /// <summary>
    /// Generates a URI for an outbound pong (Adreflexio).
    /// </summary>
    Uri GenerateUri(Adreflexio entity);

    /// <summary>
    /// Gets the base URI (scheme + authority) for generating entity URIs.
    /// For example: "https://example.com"
    /// </summary>
    Uri BaseUri { get; }
}
