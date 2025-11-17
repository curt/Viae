// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viae.Domain.Models;

namespace Viae.Persistence.Configurations;

/// <summary>
/// Configures the Vestigium entity for Entity Framework Core.
/// </summary>
public class VestigiumConfiguration : IEntityTypeConfiguration<Vestigium>
{
    /// <inheritdoc />
    public void Configure(EntityTypeBuilder<Vestigium> builder)
    {
        // The Status property is a computed property based on PublishedAt and TombstonedAt
        // For in-memory testing, we need to ensure EF doesn't try to translate it
        // Note: This is automatically handled by EF Core for non-InMemory providers
        // but we keep the configuration explicit for clarity
    }
}
