// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using NetTopologySuite.Geometries;

namespace Viae.Domain.Criteria;

/// <summary>
/// Criteria for querying Locus entities.
/// </summary>
public class LocusCriteria
{
    /// <summary>
    /// Filter by proximity to a point. Only loca within MaxDistanceMeters of this point will be returned.
    /// Must be used together with MaxDistanceMeters.
    /// </summary>
    public Point? NearPoint { get; set; }

    /// <summary>
    /// Maximum distance in meters from NearPoint. Only used when NearPoint is specified.
    /// </summary>
    public double? MaxDistanceMeters { get; set; }

    /// <summary>
    /// Filter to only loca that have at least this many associated themata.
    /// </summary>
    public int? MinThemataCount { get; set; }

    /// <summary>
    /// Filter to only loca that have a specific thema by ID.
    /// </summary>
    public string? HasThemaId { get; set; }

    /// <summary>
    /// Specifies how to sort the results. Default is by name.
    /// </summary>
    public LocusSortField SortBy { get; set; } = LocusSortField.Name;

    /// <summary>
    /// Sort direction. Default is ascending.
    /// </summary>
    public SortDirection SortDirection { get; set; } = SortDirection.Ascending;

    /// <summary>
    /// Page number (1-based). Used with PageSize for pagination.
    /// </summary>
    public int? PageNumber { get; set; }

    /// <summary>
    /// Number of results per page. Used with PageNumber for pagination.
    /// </summary>
    public int? PageSize { get; set; }

    /// <summary>
    /// Specifies whether the results include associated vestigia. Default is false.
    /// </summary>
    public VestigiumCriteria? IncludesVestigia { get; set; }
}
