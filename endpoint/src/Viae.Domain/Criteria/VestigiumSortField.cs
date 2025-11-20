// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Criteria;

/// <summary>
/// Defines the available sort fields for Vestigium queries.
/// </summary>
public enum VestigiumSortField
{
    /// <summary>Sort by when the event happened.</summary>
    HappenedAt,

    /// <summary>Sort by persona ID.</summary>
    PersonaId,

    /// <summary>Sort by locus ID.</summary>
    LocusId,
}
