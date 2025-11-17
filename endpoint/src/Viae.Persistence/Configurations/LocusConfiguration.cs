// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viae.Domain.Models;

namespace Viae.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for Locus entity.
/// </summary>
public class LocusConfiguration : IEntityTypeConfiguration<Locus>
{
    public void Configure(EntityTypeBuilder<Locus> builder)
    {
        // Ignore convenience properties - they use Coordinates internally
        builder.Ignore(l => l.Latitude);
        builder.Ignore(l => l.Longitude);

        // PostGIS Point column for coordinates
        builder.Property(l => l.Coordinates).HasColumnType("geography(Point, 4326)");
        builder.HasIndex(l => l.Coordinates).HasMethod("gist");

        builder.Property(l => l.Name).UseCollation("und_nodiac");
        builder.HasIndex(l => l.Name);
    }
}
