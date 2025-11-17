// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viae.Domain.Models;

namespace Viae.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for Folium entity.
/// </summary>
public class FoliumConfiguration : IEntityTypeConfiguration<Folium>
{
    public void Configure(EntityTypeBuilder<Folium> builder)
    {
        builder.HasIndex(p => p.PublishedAt);
        builder.HasIndex(p => p.IsDraft);
    }
}
