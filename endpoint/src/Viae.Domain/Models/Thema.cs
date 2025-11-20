// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// Represents a theme or subject tag.
/// </summary>
public class Thema : IdentifiableBase
{
    /// <summary>
    /// The display name of the Thema (e.g., "San Francisco").
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// URL-friendly slug for the Thema (e.g., "san-francisco").
    /// Used in routes like /themata/san-francisco
    /// </summary>
    public string Slug { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the date and time when this Thema was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Navigation property for Folios that have this Thema.
    /// </summary>
    public ICollection<Folium> Folios { get; set; } = [];

    /// <summary>
    /// Navigation property for Loca that have this Thema.
    /// </summary>
    public ICollection<Locus> Loca { get; set; } = [];
}
