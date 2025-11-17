// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Domain.Criteria;

/// <summary>
/// Fluent builder for VestigiumCriteria.
/// </summary>
public class VestigiumCriteriaBuilder
{
    private readonly VestigiumCriteria _criteria = new();

    /// <summary>
    /// Filter by persona ID.
    /// </summary>
    public VestigiumCriteriaBuilder ForPersona(string personaId)
    {
        _criteria.PersonaId = personaId;
        return this;
    }

    /// <summary>
    /// Filter by locus ID.
    /// </summary>
    public VestigiumCriteriaBuilder AtLocus(string locusId)
    {
        _criteria.LocusId = locusId;
        return this;
    }

    /// <summary>
    /// Filter by origo.
    /// </summary>
    public VestigiumCriteriaBuilder FromOrigo(Origo origo)
    {
        _criteria.Origo = origo;
        return this;
    }

    /// <summary>
    /// Filter to events that happened after the specified date/time.
    /// </summary>
    public VestigiumCriteriaBuilder After(DateTime dateTime)
    {
        _criteria.HappenedAfter = dateTime;
        return this;
    }

    /// <summary>
    /// Filter to events that happened before the specified date/time.
    /// </summary>
    public VestigiumCriteriaBuilder Before(DateTime dateTime)
    {
        _criteria.HappenedBefore = dateTime;
        return this;
    }

    /// <summary>
    /// Filter to events within the specified date/time range.
    /// </summary>
    public VestigiumCriteriaBuilder Between(DateTime start, DateTime end)
    {
        _criteria.HappenedAfter = start;
        _criteria.HappenedBefore = end;
        return this;
    }

    /// <summary>
    /// Include latens vestigia in results.
    /// </summary>
    public VestigiumCriteriaBuilder IncludeLatens()
    {
        _criteria.IncludeLatens = true;
        return this;
    }

    /// <summary>
    /// Exclude manifestus vestigia in results.
    /// </summary>
    public VestigiumCriteriaBuilder ExcludeManifestus()
    {
        _criteria.IncludeManifestus = false;
        return this;
    }

    /// <summary>
    /// Include deletus vestigia in results.
    /// </summary>
    public VestigiumCriteriaBuilder IncludeDeletus()
    {
        _criteria.IncludeDeletus = true;
        return this;
    }

    /// <summary>
    /// Sort by when the event happened.
    /// </summary>
    public VestigiumCriteriaBuilder SortByHappenedAt()
    {
        _criteria.SortBy = VestigiumSortField.HappenedAt;
        return this;
    }

    /// <summary>
    /// Sort by persona ID.
    /// </summary>
    public VestigiumCriteriaBuilder SortByPersona()
    {
        _criteria.SortBy = VestigiumSortField.PersonaId;
        return this;
    }

    /// <summary>
    /// Sort by locus ID.
    /// </summary>
    public VestigiumCriteriaBuilder SortByLocus()
    {
        _criteria.SortBy = VestigiumSortField.LocusId;
        return this;
    }

    /// <summary>
    /// Sort in ascending order.
    /// </summary>
    public VestigiumCriteriaBuilder Ascending()
    {
        _criteria.SortDirection = SortDirection.Ascending;
        return this;
    }

    /// <summary>
    /// Sort in descending order.
    /// </summary>
    public VestigiumCriteriaBuilder Descending()
    {
        _criteria.SortDirection = SortDirection.Descending;
        return this;
    }

    /// <summary>
    /// Configure pagination.
    /// </summary>
    public VestigiumCriteriaBuilder Page(int pageNumber, int pageSize)
    {
        _criteria.PageNumber = pageNumber;
        _criteria.PageSize = pageSize;
        return this;
    }

    /// <summary>
    /// Build the criteria object.
    /// </summary>
    public VestigiumCriteria Build() => _criteria;
}
