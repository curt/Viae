// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore;
using Viae.Domain.Models;
using Viae.Persistence;
using Viae.Persistence.Generators;

namespace Viae.Testing.Common;

/// <summary>
/// Test-only DbContext that overrides ViaeDbContext to work with InMemory database.
/// </summary>
public class TestViaeDbContext(DbContextOptions<ViaeDbContext> options) : ViaeDbContext(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Skip the base.OnModelCreating to avoid PostgreSQL-specific configuration
        // Apply only the minimal configurations needed for testing entities

        // Configure Persona entity
        modelBuilder.Entity<Persona>(builder =>
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Id).IsRequired();
            builder.Property(u => u.Username).IsRequired();
            builder.Property(u => u.DisplayName).IsRequired();
            builder.Property(u => u.CreatedAt).IsRequired();
            builder.HasIndex(u => u.CognitoUserId).IsUnique();
        });

        // Configure Thema entity (required by ViaeDbContext)
        modelBuilder.Entity<Thema>(builder =>
        {
            builder.HasKey(t => t.Id);
            builder.Property(t => t.Id).IsRequired();
            builder.Property(t => t.Name).IsRequired();
            builder.Property(t => t.Slug).IsRequired();
        });

        // Configure Missio entities (Ping/Pong models)
        modelBuilder.Entity<Admissio>(builder =>
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).IsRequired();
            builder.Property(a => a.Uri).IsRequired();
            builder.Property(a => a.Actor).IsRequired();
            builder.Property(a => a.To).IsRequired();
            builder.Property(a => a.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Abmissio>(builder =>
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).IsRequired();
            builder.Property(a => a.Uri).IsRequired();
            builder.Property(a => a.Actor).IsRequired();
            builder.Property(a => a.To).IsRequired();
            builder.Property(a => a.ReceivedAt).IsRequired();
        });

        modelBuilder.Entity<Adreflexio>(builder =>
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).IsRequired();
            builder.Property(a => a.Uri).IsRequired();
            builder.Property(a => a.Actor).IsRequired();
            builder.Property(a => a.To).IsRequired();
            builder.Property(a => a.PingUri).IsRequired();
            builder.Property(a => a.CreatedAt).IsRequired();
        });

        modelBuilder.Entity<Abreflexio>(builder =>
        {
            builder.HasKey(a => a.Id);
            builder.Property(a => a.Id).IsRequired();
            builder.Property(a => a.Uri).IsRequired();
            builder.Property(a => a.Actor).IsRequired();
            builder.Property(a => a.To).IsRequired();
            builder.Property(a => a.PingUri).IsRequired();
            builder.Property(a => a.ReceivedAt).IsRequired();
        });

        // Configure Locus entity for InMemory database (spatial support)
        modelBuilder.Entity<Locus>(builder =>
        {
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id).IsRequired();
            builder.Property(l => l.Name).IsRequired();
            builder.Property(l => l.CreatedAt).IsRequired();
            // InMemory database supports basic spatial types from NetTopologySuite
            builder.Property(l => l.Coordinates).IsRequired();
            // Many-to-many relationship with Thema
            builder.HasMany(l => l.Themata).WithMany(t => t.Loca);
        });

        // Apply identifiable convention for InMemory database (no column type override)
        modelBuilder.ApplyIdentifiableConventionForTesting();
    }
}

/// <summary>
/// Extension methods for test-specific model configuration.
/// </summary>
public static class TestModelBuilderExtensions
{
    /// <summary>
    /// Test version of ApplyIdentifiableConvention that doesn't set column types
    /// (InMemory database doesn't support custom column types).
    /// </summary>
    public static void ApplyIdentifiableConventionForTesting(this ModelBuilder modelBuilder)
    {
        foreach (
            var et in modelBuilder
                .Model.GetEntityTypes()
                .Where(et => typeof(IIdentifiable).IsAssignableFrom(et.ClrType) && !et.IsOwned())
        )
        {
            var idProp = et.FindProperty(nameof(IIdentifiable.Id));
            if (idProp is null || idProp.ClrType != typeof(string))
            {
                continue;
            }

            // Configure value generator without column type (InMemory doesn't support it)
            modelBuilder
                .Entity(et.ClrType)
                .Property(nameof(IIdentifiable.Id))
                .HasValueGenerator(static (_, __) => new Base58IdValueGenerator());
        }
    }
}
