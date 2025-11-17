// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// Represents a model that has a unique string identifier.
/// </summary>
public interface IIdentifiable
{
    /// <summary>
    /// Gets or sets the unique identifier.
    /// </summary>
    /// <remarks>
    /// Identifier is nullable so generator will work correctly.
    /// </remarks>
    string Id { get; set; }
}
