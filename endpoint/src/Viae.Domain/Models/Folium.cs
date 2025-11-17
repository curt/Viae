// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// Represents a post.
/// </summary>
public class Folium : IdentifiableBase
{
    /// <summary>
    /// Gets or sets the origin of this Folium.
    /// </summary>
    public Origo Origo { get; set; }

    /// <summary>
    /// Gets or sets the title of the Folium.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the content/body of the Folium.
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the foreign key to the Persona who authored this Folium.
    /// </summary>
    public string PersonaId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the navigation property to the Persona who authored this Folium.
    /// </summary>
    public Persona Persona { get; set; } = null!;

    /// <summary>
    /// Gets or sets the date and time when this Folium was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this Folium was published. Null if never published.
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this post Folium last updated. Null if never updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether this Folium is a draft (not yet published).
    /// </summary>
    public bool IsDraft { get; set; } = true;

    /// <summary>
    /// The ActivityPub URI for this Folium.
    /// </summary>
    public Uri? Uri { get; set; }

    /// <summary>
    /// The ActivityPub URL for this Folium.
    /// </summary>
    public Uri? Url { get; set; }

    /// <summary>
    /// Gets or sets the list of image URLs associated with this Folium.
    /// </summary>
    public List<string> ImageUrls { get; set; } = [];

    /// <summary>
    /// Navigation property for Themata associated with this Folium.
    /// </summary>
    public ICollection<Thema> Themata { get; set; } = [];

    /// <summary>
    /// Navigation property for Loca associated with this Folium.
    /// </summary>
    public ICollection<Locus> Loca { get; set; } = [];
}
