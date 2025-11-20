// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;
using Viae.Persistence;
using Viae.Persistence.Generators;

namespace Viae.Services;

/// <summary>
/// Abstract base class for services that work with identifiable entities.
/// Provides common functionality for ID and URI generation.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="EntityServiceBase"/> class.
/// </remarks>
/// <param name="uriGenerator">Optional URI generator service for entities that need URIs.</param>
public abstract class EntityServiceBase(IUriGeneratorService? uriGenerator = null)
{
    /// <summary>
    /// Prepares an entity for insertion by generating its ID and URI (if applicable).
    /// Call this method before adding an entity to the DbContext.
    /// </summary>
    /// <typeparam name="TEntity">The entity type, must implement IIdentifiable.</typeparam>
    /// <param name="entity">The entity to prepare.</param>
    protected void PrepareEntityForInsertion<TEntity>(TEntity entity)
        where TEntity : IIdentifiable
    {
        // Generate ID if not already set
        if (string.IsNullOrEmpty(entity.Id))
        {
            entity.Id = Base58IdGenerator.Generate();
        }

        // Generate URI if entity implements IUriIdentifiable
        if (entity is IUriIdentifiable uriIdentifiable && uriGenerator != null)
        {
            if (uriIdentifiable.Uri == null)
            {
                // Use dynamic dispatch based on concrete type
                uriIdentifiable.Uri = entity switch
                {
                    Admissio admissio => uriGenerator.GenerateUri(admissio),
                    Adreflexio adreflexio => uriGenerator.GenerateUri(adreflexio),
                    // Add other IUriIdentifiable types here as needed
                    _ => throw new NotSupportedException(
                        $"URI generation not implemented for type {entity.GetType().Name}. "
                            + $"Add a case for this type in EntityServiceBase.PrepareEntityForInsertion."
                    ),
                };
            }
        }
    }
}
