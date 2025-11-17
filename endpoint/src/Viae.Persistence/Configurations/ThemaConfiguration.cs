// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viae.Domain.Models;

namespace Viae.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for Tag entity.
/// </summary>
public class ThemaConfiguration : IEntityTypeConfiguration<Thema>
{
    public void Configure(EntityTypeBuilder<Thema> builder)
    {
        builder.HasIndex(t => t.Slug).IsUnique();
        builder.HasIndex(t => t.Name);
    }
}
