// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Viae.Domain.Models;

namespace Viae.Persistence.Interceptors;

public class TimestampsInterceptor : SaveChangesInterceptor
{
    public override InterceptionResult<int> SavingChanges(
        DbContextEventData eventData,
        InterceptionResult<int> result
    )
    {
        UpdateTimestamps(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default
    )
    {
        UpdateTimestamps(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private static void UpdateTimestamps(DbContext? context)
    {
        if (context != null)
        {
            var now = DateTime.UtcNow;

            foreach (var entry in context.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Added)
                {
                    if (entry.Entity is ICreatable entity)
                    {
                        if (entity.CreatedAt == default)
                        {
                            entity.CreatedAt = now;
                        }
                    }
                }
                else if (entry.State == EntityState.Modified)
                {
                    if (entry.Entity is IUpdatable entity)
                    {
                        entity.UpdatedAt = now;
                    }
                }
                else if (entry.State == EntityState.Deleted)
                {
                    if (entry.Entity is ITombstonable entity)
                    {
                        entity.TombstonedAt = now;
                        entry.State = EntityState.Modified;
                    }
                }
            }
        }
    }
}
