// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Viae.Domain.Models;

namespace Viae.Persistence;

/// <summary>
/// Design-time factory for creating ViaeDbContext instances for EF Core tools.
/// This is used by migrations and other design-time tools.
/// </summary>
public class ViaeDbContextFactory : IDesignTimeDbContextFactory<ViaeDbContext>
{
    public ViaeDbContext CreateDbContext(string[] args)
    {
        var postgresPort = Environment.GetEnvironmentVariable("Postgres__Port") ?? "5432";
        var postgresPassword = Environment.GetEnvironmentVariable("Postgres__Password") ?? "viae";

        var optionsBuilder = new DbContextOptionsBuilder<ViaeDbContext>();

        // Use a default connection string for migrations
        // This will be overridden at runtime by the actual application configuration
        optionsBuilder.UseNpgsql(
            $"Host=localhost;Port={postgresPort};Database=viae;Username=viae;Password={postgresPassword}",
            npgsqlOptions =>
            {
                npgsqlOptions.UseNetTopologySuite();
                npgsqlOptions.MapEnum<Origo>();
            }
        );

        return new ViaeDbContext(optionsBuilder.Options);
    }
}
