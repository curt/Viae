// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Viae.Domain.Models;

namespace Viae.Persistence.Configurations;

/// <summary>
/// Entity Framework Core configuration for Persona entity.
/// </summary>
public class PersonaConfiguration : IEntityTypeConfiguration<Persona>
{
    public void Configure(EntityTypeBuilder<Persona> builder)
    {
        builder.HasIndex(u => u.Username).IsUnique();
        builder.HasIndex(u => u.Uri).IsUnique();
        builder.HasIndex(u => u.CognitoUserId).IsUnique();
        builder.HasIndex(u => u.Email);
    }
}
