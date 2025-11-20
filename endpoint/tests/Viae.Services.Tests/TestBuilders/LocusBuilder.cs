// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using NetTopologySuite.Geometries;
using Viae.Domain.Models;

namespace Viae.Services.Tests.TestBuilders;

/// <summary>
/// Fluent builder for creating Locus test data.
/// </summary>
public class LocusBuilder
{
    private string _id = Guid.NewGuid().ToString("N")[..12]; // Short test ID
    private string _name = "Default Locus";
    private Point _coordinates = new(-122.4194, 37.7749) { SRID = 4326 }; // San Francisco
    private string? _slug;
    private string? _content;
    private string? _address;
    private string? _city;
    private string? _country;
    private DateTime _createdAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private readonly List<Thema> _themata = [];
    private readonly List<Folium> _folios = [];

    /// <summary>
    /// Creates a new LocusBuilder with default values.
    /// </summary>
    public static LocusBuilder Default() => new();

    /// <summary>
    /// Sets the ID of the locus.
    /// </summary>
    public LocusBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    /// <summary>
    /// Sets the name of the locus.
    /// </summary>
    public LocusBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the coordinates using latitude and longitude.
    /// Note: Point constructor takes (longitude, latitude) but this method takes (latitude, longitude) for readability.
    /// </summary>
    public LocusBuilder WithCoordinates(double latitude, double longitude)
    {
        _coordinates = new Point(longitude, latitude) { SRID = 4326 };
        return this;
    }

    /// <summary>
    /// Sets the coordinates using a Point object.
    /// </summary>
    public LocusBuilder WithCoordinates(Point coordinates)
    {
        _coordinates = coordinates;
        return this;
    }

    /// <summary>
    /// Sets the slug of the locus.
    /// </summary>
    public LocusBuilder WithSlug(string slug)
    {
        _slug = slug;
        return this;
    }

    /// <summary>
    /// Sets the content of the locus.
    /// </summary>
    public LocusBuilder WithContent(string content)
    {
        _content = content;
        return this;
    }

    /// <summary>
    /// Sets the address of the locus.
    /// </summary>
    public LocusBuilder WithAddress(string address)
    {
        _address = address;
        return this;
    }

    /// <summary>
    /// Sets the city of the locus.
    /// </summary>
    public LocusBuilder WithCity(string city)
    {
        _city = city;
        return this;
    }

    /// <summary>
    /// Sets the country of the locus.
    /// </summary>
    public LocusBuilder WithCountry(string country)
    {
        _country = country;
        return this;
    }

    /// <summary>
    /// Sets the created date of the locus.
    /// </summary>
    public LocusBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    /// <summary>
    /// Adds a single thema to the locus.
    /// </summary>
    public LocusBuilder WithThema(Thema thema)
    {
        _themata.Add(thema);
        return this;
    }

    /// <summary>
    /// Adds multiple themata to the locus.
    /// </summary>
    public LocusBuilder WithThemata(params Thema[] themata)
    {
        _themata.AddRange(themata);
        return this;
    }

    /// <summary>
    /// Adds multiple themata to the locus.
    /// </summary>
    public LocusBuilder WithThemata(IEnumerable<Thema> themata)
    {
        _themata.AddRange(themata);
        return this;
    }

    /// <summary>
    /// Builds the Locus with the configured values.
    /// </summary>
    public Locus Build()
    {
        var locus = new Locus
        {
            Id = _id,
            Name = _name,
            Coordinates = _coordinates,
            Slug = _slug,
            Content = _content,
            Address = _address,
            City = _city,
            Country = _country,
            CreatedAt = _createdAt,
        };

        foreach (var thema in _themata)
        {
            locus.Themata.Add(thema);
        }

        foreach (var folium in _folios)
        {
            locus.Folios.Add(folium);
        }

        return locus;
    }

    #region Factory Methods

    /// <summary>
    /// Creates a locus representing Golden Gate Park in San Francisco.
    /// </summary>
    public static Locus GoldenGatePark() =>
        Default()
            .WithName("Golden Gate Park")
            .WithCoordinates(37.7694, -122.4862)
            .WithCity("San Francisco")
            .WithCountry("USA")
            .Build();

    /// <summary>
    /// Creates a locus representing the Golden Gate Bridge.
    /// </summary>
    public static Locus GoldenGateBridge() =>
        Default()
            .WithName("Golden Gate Bridge")
            .WithCoordinates(37.8199, -122.4783)
            .WithCity("San Francisco")
            .WithCountry("USA")
            .Build();

    /// <summary>
    /// Creates a locus representing Alcatraz Island.
    /// </summary>
    public static Locus AlcatrazIsland() =>
        Default()
            .WithName("Alcatraz Island")
            .WithCoordinates(37.8267, -122.4230)
            .WithCity("San Francisco")
            .WithCountry("USA")
            .Build();

    /// <summary>
    /// Creates a locus representing the Palace of Fine Arts.
    /// </summary>
    public static Locus PalaceOfFineArts() =>
        Default()
            .WithName("Palace of Fine Arts")
            .WithCoordinates(37.8026, -122.4486)
            .WithCity("San Francisco")
            .WithCountry("USA")
            .Build();

    /// <summary>
    /// Creates a builder for a locus with a simple setup for basic tests.
    /// Returns a builder to allow further customization.
    /// </summary>
    public static LocusBuilder Simple(string name = "Simple Location") => Default().WithName(name);

    #endregion
}
