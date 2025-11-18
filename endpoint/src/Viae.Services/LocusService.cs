// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore;
using Viae.Domain.Criteria;
using Viae.Domain.Models;
using Viae.Domain.Services;
using Viae.Persistence;

namespace Viae.Services;

/// <summary>
/// Service for managing Locus entities.
/// </summary>
public class LocusService(IDbContextFactory<ViaeDbContext> ctxFactory) : ILocusService
{
    /// <inheritdoc />
    public async Task<IReadOnlyList<Locus>> GetLocaAsync(
        LocusCriteria criteria,
        CancellationToken ct = default
    )
    {
        using var ctx = ctxFactory.CreateDbContext();

        var query = ApplyCriteria(ctx.Loca, criteria);

        return await query.ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyDictionary<string, double>> GetLocaDistancesAsync(
        IEnumerable<Locus> loca,
        Locus referenceLocus,
        CancellationToken ct = default
    )
    {
        using var ctx = ctxFactory.CreateDbContext();

        var ids = loca.Select(l => l.Id);
        return await ctx
            .Loca.Where(l => ids.Contains(l.Id))
            .Select(l => new
            {
                l.Id,
                Distance = l.Coordinates.Distance(referenceLocus.Coordinates),
            })
            .ToDictionaryAsync(o => o.Id, o => o.Distance, ct);
    }

    public async Task<IEnumerable<LocusDistance>> GetLocaDistancesAsync(
        LocusCriteria criteria,
        Locus referenceLocus,
        CancellationToken ct = default
    )
    {
        using var ctx = ctxFactory.CreateDbContext();
        var query = ApplyCriteria(ctx.Loca, criteria);
        return await query
            .Select(l => new LocusDistance
            {
                Locus = l,
                ReferenceLocus = referenceLocus,
                Distance = l.Coordinates.Distance(referenceLocus.Coordinates),
            })
            .ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<Locus?> GetLocusByIdAsync(string id, CancellationToken ct = default)
    {
        using var ctx = ctxFactory.CreateDbContext();

        // TODO: Use criteria for Vestigia.
        return await ctx
            .Loca.Include(l => l.Vestigia.OrderByDescending(v => v.HappenedAt))
            .FirstOrDefaultAsync(l => l.Id == id, ct);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Vestigium>> GetVestigiaAsync(
        VestigiumCriteria criteria,
        CancellationToken ct = default
    )
    {
        using var ctx = ctxFactory.CreateDbContext();

        var query = ApplyVestigiumCriteria(ctx.Vestigia.Include(v => v.Locus), criteria);

        return await query.ToListAsync(ct);
    }

    /// <inheritdoc />
    public async Task<Vestigium?> GetVestigiumByIdAsync(string id, CancellationToken ct = default)
    {
        using var ctx = ctxFactory.CreateDbContext();

        return await ctx.Vestigia.Include(v => v.Locus).FirstOrDefaultAsync(v => v.Id == id, ct);
    }

    /// <inheritdoc />
    public async Task<int> GetVestigiaCountAsync(
        VestigiumCriteria criteria,
        CancellationToken ct = default
    )
    {
        using var ctx = ctxFactory.CreateDbContext();

        var query = ApplyVestigiumCriteria(ctx.Vestigia, criteria);

        return await query.CountAsync(ct);
    }

    private static IQueryable<Locus> ApplyCriteria(IQueryable<Locus> query, LocusCriteria criteria)
    {
        // Geospatial filter - distance from point
        if (criteria.NearPoint != null && criteria.MaxDistanceMeters.HasValue)
        {
            query = query.Where(l =>
                l.Coordinates.Distance(criteria.NearPoint) <= criteria.MaxDistanceMeters.Value
            );
        }

        // Filter by minimum themata count
        if (criteria.MinThemataCount.HasValue)
        {
            query = query.Where(l => l.Themata.Count >= criteria.MinThemataCount.Value);
        }

        // Filter by having a specific thema
        if (!string.IsNullOrEmpty(criteria.HasThemaId))
        {
            query = query.Where(l => l.Themata.Any(t => t.Id == criteria.HasThemaId));
        }

        // Apply sorting
        query = ApplySorting(query, criteria);

        // Apply pagination
        if (criteria.PageNumber.HasValue && criteria.PageSize.HasValue)
        {
            query = query
                .Skip((criteria.PageNumber.Value - 1) * criteria.PageSize.Value)
                .Take(criteria.PageSize.Value);
        }

        // Apply associations
        if (criteria.IncludesVestigia != null)
        {
            query.Include(l => l.Vestigia); // TODO: Apply criteria
        }

        return query;
    }

    private static IQueryable<Locus> ApplySorting(IQueryable<Locus> query, LocusCriteria criteria)
    {
        var isAscending = criteria.SortDirection == SortDirection.Ascending;

        return criteria.SortBy switch
        {
            LocusSortField.Name => isAscending
                ? query.OrderBy(l => l.Name)
                : query.OrderByDescending(l => l.Name),

            LocusSortField.Distance => criteria.NearPoint != null
                ? (
                    isAscending
                        ? query.OrderBy(l => l.Coordinates.Distance(criteria.NearPoint))
                        : query.OrderByDescending(l => l.Coordinates.Distance(criteria.NearPoint))
                )
                : query.OrderBy(l => l.Name), // Fallback if no point specified

            LocusSortField.CreatedAt => isAscending
                ? query.OrderBy(l => l.CreatedAt)
                : query.OrderByDescending(l => l.CreatedAt),

            LocusSortField.ThemataCount => isAscending
                ? query.OrderBy(l => l.Themata.Count)
                : query.OrderByDescending(l => l.Themata.Count),

            _ => query.OrderBy(l => l.Name),
        };
    }

    private static IQueryable<Vestigium> ApplyVestigiumCriteria(
        IQueryable<Vestigium> query,
        VestigiumCriteria criteria
    )
    {
        // Filter by persona ID
        if (!string.IsNullOrEmpty(criteria.PersonaId))
        {
            query = query.Where(v => v.PersonaId == criteria.PersonaId);
        }

        // Filter by locus ID
        if (!string.IsNullOrEmpty(criteria.LocusId))
        {
            query = query.Where(v => v.LocusId == criteria.LocusId);
        }

        // Filter by origo
        if (criteria.Origo.HasValue)
        {
            query = query.Where(v => v.Origo == criteria.Origo.Value);
        }

        // Filter by date range
        if (criteria.HappenedAfter.HasValue)
        {
            query = query.Where(v => v.HappenedAt >= criteria.HappenedAfter.Value);
        }

        if (criteria.HappenedBefore.HasValue)
        {
            query = query.Where(v => v.HappenedAt <= criteria.HappenedBefore.Value);
        }

        // Filter by status
        query = query.Where(v =>
            (criteria.IncludeLatens && v.PublishedAt == null && v.TombstonedAt == null)
            || (criteria.IncludeManifestus && v.PublishedAt != null && v.TombstonedAt == null)
            || (criteria.IncludeDeletus && v.TombstonedAt != null)
        );

        // Apply sorting
        query = ApplyVestigiumSorting(query, criteria);

        // Apply pagination
        if (criteria.PageNumber.HasValue && criteria.PageSize.HasValue)
        {
            query = query
                .Skip((criteria.PageNumber.Value - 1) * criteria.PageSize.Value)
                .Take(criteria.PageSize.Value);
        }

        return query;
    }

    private static IQueryable<Vestigium> ApplyVestigiumSorting(
        IQueryable<Vestigium> query,
        VestigiumCriteria criteria
    )
    {
        var isAscending = criteria.SortDirection == SortDirection.Ascending;

        return criteria.SortBy switch
        {
            VestigiumSortField.HappenedAt => isAscending
                ? query.OrderBy(v => v.HappenedAt)
                : query.OrderByDescending(v => v.HappenedAt),

            VestigiumSortField.PersonaId => isAscending
                ? query.OrderBy(v => v.PersonaId)
                : query.OrderByDescending(v => v.PersonaId),

            VestigiumSortField.LocusId => isAscending
                ? query.OrderBy(v => v.LocusId)
                : query.OrderByDescending(v => v.LocusId),

            _ => query.OrderByDescending(v => v.HappenedAt),
        };
    }

    /// <inheritdoc />
    public async Task<Locus> AddLocusAsync(Locus locus, CancellationToken ct = default)
    {
        using var ctx = ctxFactory.CreateDbContext();

        ctx.Add(locus);
        await ctx.SaveChangesAsync(ct);

        return locus;
    }

    /// <inheritdoc />
    public async Task<Locus?> UpdateLocusAsync(Locus locus, CancellationToken ct = default)
    {
        using var ctx = ctxFactory.CreateDbContext();

        var found = await ctx.Loca.FindAsync([locus.Id], ct);

        if (found != null)
        {
            found.Name = locus.Name;
            found.Coordinates = locus.Coordinates;
            found.OsmId = locus.OsmId;
            found.Slug = locus.Slug;
            found.Content = locus.Content;

            ctx.Update(found);
            await ctx.SaveChangesAsync(ct);

            return found;
        }

        return null;
    }

    /// <inheritdoc />
    public async Task<Vestigium> AddVestigiumAsync(
        Vestigium vestigium,
        CancellationToken ct = default
    )
    {
        using var ctx = ctxFactory.CreateDbContext();

        // Clear navigation properties to avoid tracking issues with related entities
        vestigium.Persona = null!;
        vestigium.Locus = null!;

        ctx.Vestigia.Add(vestigium);
        await ctx.SaveChangesAsync(ct);

        return vestigium;
    }

    /// <inheritdoc />
    public async Task<Vestigium?> UpdateVestigiumAsync(
        string id,
        DateTime happenedAt,
        string content,
        CancellationToken ct = default
    )
    {
        using var ctx = ctxFactory.CreateDbContext();

        var vestigium = await ctx.Vestigia.FindAsync([id], ct);

        if (vestigium != null)
        {
            vestigium.HappenedAt = happenedAt;
            vestigium.Content = content;

            await ctx.SaveChangesAsync(ct);

            return vestigium;
        }

        return null;
    }

    /// <inheritdoc />
    public async Task<Vestigium> PublishVestigiumAsync(string id, CancellationToken ct = default)
    {
        using var ctx = ctxFactory.CreateDbContext();

        var vestigium =
            await ctx.Vestigia.FindAsync([id], ct)
            ?? throw new InvalidOperationException($"Vestigium with ID '{id}' not found.");

        vestigium.PublishedAt = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);

        return vestigium;
    }

    /// <inheritdoc />
    public async Task<Vestigium> TombstoneVestigiumAsync(string id, CancellationToken ct = default)
    {
        using var ctx = ctxFactory.CreateDbContext();

        var vestigium =
            await ctx.Vestigia.FindAsync([id], ct)
            ?? throw new InvalidOperationException($"Vestigium with ID '{id}' not found.");

        vestigium.TombstonedAt = DateTime.UtcNow;

        await ctx.SaveChangesAsync(ct);

        return vestigium;
    }
}
