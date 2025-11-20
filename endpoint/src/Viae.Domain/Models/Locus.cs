// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using NetTopologySuite.Geometries;

namespace Viae.Domain.Models;

/// <summary>
/// Represents a geographic location.
/// </summary>
public class Locus : CreatableBase
{
    /// <summary>
    /// Gets or sets the OpenStreetMap identifier for this Locus.
    /// </summary>
    public OsmId? OsmId { get; set; }

    /// <summary>
    /// Gets or sets the name of this Locus.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the slug of this Locus.
    /// </summary>
    public string? Slug { get; set; }

    /// <summary>
    /// Geographic coordinates for this Locus using PostGIS Point type.
    /// Stored in SRID 4326 (WGS 84).
    /// </summary>
    public Point Coordinates { get; set; } = null!;

    /// <summary>
    /// Convenience property for latitude (Y coordinate) for this Locus.
    /// Not mapped to database - uses Coordinates.Y internally.
    /// </summary>
    public double Latitude
    {
        get => Coordinates?.Y ?? 0;
        set =>
            Coordinates =
                Coordinates == null
                    ? new Point(Longitude, value) { SRID = 4326 }
                    : new Point(Coordinates.X, value) { SRID = 4326 };
    }

    /// <summary>
    /// Convenience property for longitude (X coordinate) for this Locus.
    /// Not mapped to database - uses Coordinates.X internally.
    /// </summary>
    public double Longitude
    {
        get => Coordinates?.X ?? 0;
        set =>
            Coordinates =
                Coordinates == null
                    ? new Point(value, Latitude) { SRID = 4326 }
                    : new Point(value, Coordinates.Y) { SRID = 4326 };
    }

    /// <summary>
    /// Gets or sets the content of this Locus.
    /// </summary>
    public string? Content { get; set; }

    /// <summary>
    /// Gets or sets the street address of this Locus.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the city where this Locus is situated.
    /// </summary>
    public string? City { get; set; }

    /// <summary>
    /// Gets or sets the country where this Locus is situated.
    /// </summary>
    public string? Country { get; set; }

    /// <summary>
    /// Navigation property for Folios associated with this Locus.
    /// </summary>
    public ICollection<Folium> Folios { get; set; } = [];

    /// <summary>
    /// Navigation property for Themata associated with this Locus.
    /// </summary>
    public ICollection<Thema> Themata { get; set; } = [];

    /// <summary>
    /// Navigation property for Vestigia associated with this Locus.
    /// </summary>
    public ICollection<Vestigium> Vestigia { get; set; } = [];
}
