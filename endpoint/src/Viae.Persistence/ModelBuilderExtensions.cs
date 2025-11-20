// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore;
using Viae.Domain.Models;
using Viae.Persistence.Generators;

namespace Viae.Persistence;

public static class ModelBuilderExtensions
{
    /// <summary>
    /// For every entity that implements IIdentifiable, configures:
    ///   - Id column type: base58id
    ///   - Client-side value generation via Base58IdValueGenerator
    ///   - Required
    /// </summary>
    public static ModelBuilder ApplyIdentifiableConvention(this ModelBuilder modelBuilder)
    {
        foreach (
            var et in modelBuilder
                .Model.GetEntityTypes()
                .Where(et => typeof(IIdentifiable).IsAssignableFrom(et.ClrType) && !et.IsOwned())
        )
        {
            // Must be the interface member name, and we expect string
            var idProp = et.FindProperty(nameof(IIdentifiable.Id));
            if (idProp is null || idProp.ClrType != typeof(string))
            {
                continue;
            }

            modelBuilder
                .Entity(et.ClrType)
                .Property(nameof(IIdentifiable.Id))
                .HasColumnType("char(11)")
                .HasValueGenerator<Base58IdValueGenerator>();
        }

        return modelBuilder;
    }

    /// <summary>
    /// For every entity that implements IUriIdentifiable, configures the Uri property.
    /// The actual URI generation is handled by the UriGenerationInterceptor.
    /// This should be called after ApplyIdentifiableConvention.
    /// </summary>
    public static ModelBuilder ApplyUriIdentifiableConvention(this ModelBuilder modelBuilder)
    {
        // No special configuration needed - the UriGenerationInterceptor handles everything
        // This method exists for consistency and potential future configuration needs
        return modelBuilder;
    }
}
