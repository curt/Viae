// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore;
using Viae.Domain.Models;
using Viae.Persistence.Converters;
using Viae.Persistence.Interceptors;

namespace Viae.Persistence;

/// <summary>
/// Entity Framework Core database context for Viae.
/// Configured for PostgreSQL with PostGIS spatial support.
/// </summary>
public class ViaeDbContext(DbContextOptions<ViaeDbContext> options) : DbContext(options)
{
    public DbSet<Folium> Folios => Set<Folium>();
    public DbSet<Persona> Personae => Set<Persona>();
    public DbSet<Locus> Loca => Set<Locus>();
    public DbSet<Vestigium> Vestigia => Set<Vestigium>();
    public DbSet<Thema> Themata => Set<Thema>();
    public DbSet<Abmissio> Abmissiones => Set<Abmissio>();
    public DbSet<Admissio> Admissiones => Set<Admissio>();
    public DbSet<Abreflexio> Abreflexiones => Set<Abreflexio>();
    public DbSet<Adreflexio> Adreflexiones => Set<Adreflexio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Enable PostGIS extension
        modelBuilder.HasPostgresExtension("postgis");

        modelBuilder.HasCollation(
            name: "und_nodiac",
            provider: "icu",
            locale: "und-u-ks-level1",
            deterministic: false
        );

        // Apply configurations from this assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ViaeDbContext).Assembly);
        modelBuilder.ApplyIdentifiableConvention();
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        // For non-nullable OsmId properties
        configurationBuilder.Properties<OsmId>().HaveConversion<OsmIdConverter>();

        // For nullable OsmId? properties
        configurationBuilder.Properties<OsmId?>().HaveConversion<NullableOsmIdConverter>();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.AddInterceptors(new TimestampsInterceptor());
    }
}
