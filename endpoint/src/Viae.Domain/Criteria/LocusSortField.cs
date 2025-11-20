// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Criteria;

/// <summary>
/// Defines the available sort fields for Locus queries.
/// </summary>
public enum LocusSortField
{
    /// <summary>Sort by name alphabetically.</summary>
    Name,

    /// <summary>Sort by distance from NearPoint (only valid when NearPoint is specified).</summary>
    Distance,

    /// <summary>Sort by creation date.</summary>
    CreatedAt,

    /// <summary>Sort by number of associated themata.</summary>
    ThemataCount,
}
