// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Criteria;
using Viae.Domain.Models;

namespace Viae.Domain.Services;

/// <summary>
/// Service for managing Locus entities.
/// </summary>
public interface ILocusService
{
    /// <summary>
    /// Gets loca based on the specified criteria.
    /// </summary>
    /// <param name="criteria">The search criteria.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A list of loca matching the criteria.</returns>
    Task<IReadOnlyList<Locus>> GetLocaAsync(LocusCriteria criteria, CancellationToken ct = default);

    /// <summary>
    /// Gets distances of specified loca from a specified point locus.
    /// </summary>
    /// <param name="criteria">The search criteria.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A dictionary of computed distances keyed by locus ID.</returns>
    Task<IReadOnlyDictionary<string, double>> GetLocaDistancesAsync(
        IEnumerable<Locus> loca,
        Locus referenceLocus,
        CancellationToken ct = default
    );

    Task<IEnumerable<LocusDistance>> GetLocaDistancesAsync(
        LocusCriteria criteria,
        Locus referenceLocus,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a single locus by ID.
    /// </summary>
    /// <param name="id">The unique identifier of the locus.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The locus if found, null otherwise.</returns>
    Task<Locus?> GetLocusByIdAsync(string id, CancellationToken ct = default);

    /// <summary>
    /// Gets a list of Vestigium entities based on the specified criteria.
    /// </summary>
    /// <param name="criteria">The criteria to filter and sort vestigia.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>A read-only list of Vestigium entities matching the criteria.</returns>
    Task<IReadOnlyList<Vestigium>> GetVestigiaAsync(
        VestigiumCriteria criteria,
        CancellationToken ct = default
    );

    /// <summary>
    /// Gets a single Vestigium by its ID.
    /// </summary>
    /// <param name="id">The vestigium ID.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The Vestigium if found, otherwise null.</returns>
    Task<Vestigium?> GetVestigiumByIdAsync(string id, CancellationToken ct = default);

    /// <summary>
    /// Gets the count of Vestigium entities matching the specified criteria.
    /// </summary>
    /// <param name="criteria">The criteria to filter vestigia.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The count of matching vestigia.</returns>
    Task<int> GetVestigiaCountAsync(VestigiumCriteria criteria, CancellationToken ct = default);

    /// <summary>
    /// Adds a new Locus to the database.
    /// </summary>
    /// <param name="locus">The locus to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The added locus.</returns>
    Task<Locus> AddLocusAsync(Locus locus, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing Locus in the database.
    /// </summary>
    /// <param name="locus">The locus with updated values.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated locus.</returns>
    Task<Locus?> UpdateLocusAsync(Locus locus, CancellationToken ct = default);

    /// <summary>
    /// Adds a new Vestigium to the database.
    /// </summary>
    /// <param name="vestigium">The vestigium to add.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The added vestigium.</returns>
    Task<Vestigium> AddVestigiumAsync(Vestigium vestigium, CancellationToken ct = default);

    /// <summary>
    /// Updates an existing Vestigium in the database.
    /// Only HappenedAt and Content can be updated.
    /// </summary>
    /// <param name="id">The ID of the vestigium to update.</param>
    /// <param name="happenedAt">The new happened at date/time.</param>
    /// <param name="content">The new content.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The updated vestigium.</returns>
    Task<Vestigium?> UpdateVestigiumAsync(
        string id,
        DateTime happenedAt,
        string content,
        CancellationToken ct = default
    );

    /// <summary>
    /// Publishes a Vestigium by setting its PublishedAt timestamp.
    /// </summary>
    /// <param name="id">The ID of the vestigium to publish.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The published vestigium.</returns>
    Task<Vestigium> PublishVestigiumAsync(string id, CancellationToken ct = default);

    /// <summary>
    /// Tombstones a Vestigium by setting its TombstonedAt timestamp.
    /// </summary>
    /// <param name="id">The ID of the vestigium to tombstone.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The tombstoned vestigium.</returns>
    Task<Vestigium> TombstoneVestigiumAsync(string id, CancellationToken ct = default);
}
