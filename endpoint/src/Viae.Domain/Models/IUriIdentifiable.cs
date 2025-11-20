// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// Marker interface for entities that have a URI property that should be automatically generated
/// based on the entity's ID and type. The URI generation happens in the service layer before
/// the entity is added to the DbContext.
/// </summary>
public interface IUriIdentifiable : IIdentifiable
{
    /// <summary>
    /// Gets or sets the URI for this entity.
    /// This property is automatically populated in the service layer after the ID is assigned.
    /// </summary>
    Uri Uri { get; set; }
}
