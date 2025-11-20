// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Domain.Criteria;

/// <summary>
/// Criteria for querying Vestigium entities.
/// </summary>
public class VestigiumCriteria
{
    /// <summary>
    /// Filter by persona ID.
    /// </summary>
    public string? PersonaId { get; set; }

    /// <summary>
    /// Filter by locus ID.
    /// </summary>
    public string? LocusId { get; set; }

    /// <summary>
    /// Filter by origo.
    /// </summary>
    public Origo? Origo { get; set; }

    /// <summary>
    /// Filter to events that happened after this date/time.
    /// </summary>
    public DateTime? HappenedAfter { get; set; }

    /// <summary>
    /// Filter to events that happened before this date/time.
    /// </summary>
    public DateTime? HappenedBefore { get; set; }

    /// <summary>
    /// Filter to include latens vestigia.
    /// </summary>
    public bool IncludeLatens { get; set; }

    /// <summary>
    /// Filter to include manifestus vestigia.
    /// </summary>
    public bool IncludeManifestus { get; set; } = true;

    /// <summary>
    /// Filter to include deletus vestigia.
    /// </summary>
    public bool IncludeDeletus { get; set; }

    /// <summary>
    /// Specifies how to sort the results.
    /// </summary>
    public VestigiumSortField SortBy { get; set; } = VestigiumSortField.HappenedAt;

    /// <summary>
    /// Sort direction.
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Descending;

    /// <summary>
    /// Page number (1-based). Used with PageSize for pagination.
    /// </summary>
    public int? PageNumber { get; set; }

    /// <summary>
    /// Number of results per page. Used with PageNumber for pagination.
    /// </summary>
    public int? PageSize { get; set; }
}
