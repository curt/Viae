// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Viae.Domain.Models;
using Viae.Persistence.Generators;

namespace Viae.Persistence;

/// <summary>
/// EF Core interceptor that automatically generates URIs for IUriIdentifiable entities
/// during SaveChanges, after IDs have been generated but before saving to the database.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UriGenerationInterceptor"/> class.
/// </remarks>
/// <param name="uriGenerator">The URI generator service.</param>
public class UriGenerationInterceptor(IUriGeneratorService uriGenerator) : SaveChangesInterceptor
{
    /// <inheritdoc />
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        GenerateUris(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <inheritdoc />
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        GenerateUris(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void GenerateUris(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        // Ensure all pending changes are detected
        context.ChangeTracker.DetectChanges();

        // Get all Added IUriIdentifiable entities that don't have URIs yet
        var entries = context
            .ChangeTracker.Entries<IUriIdentifiable>()
            .Where(e => e.State == EntityState.Added && e.Entity.Uri == null)
            .ToList(); // Materialize to avoid collection modified during iteration

        foreach (var entry in entries)
        {
            // The ID should have been generated when the entity was added
            // Check if it's been set
            var id = entry.Entity.Id;

            // If still empty, generate ID manually
            // EF Core InMemory doesn't always trigger value generators at the right time
            if (string.IsNullOrEmpty(id))
            {
                // Generate ID using the same generator
                id = Base58IdGenerator.Generate();
                entry.Entity.Id = id;

                // Mark the ID property as modified so EF Core knows to save it
                entry.Property(nameof(IIdentifiable.Id)).IsModified = true;
            }

            // Generate URI based on entity type
            var uri = entry.Entity switch
            {
                Admissio admissio => uriGenerator.GenerateUri(admissio),
                Adreflexio adreflexio => uriGenerator.GenerateUri(adreflexio),
                _ => throw new NotSupportedException(
                    $"URI generation not supported for type {entry.Entity.GetType().Name}"
                ),
            };

            entry.Entity.Uri = uri;
            // Mark the URI property as modified
            entry.Property(nameof(IUriIdentifiable.Uri)).IsModified = true;
        }
    }
}
