// Copyright (c) Curt Gilman. Licensed under the AGPL-3.0 License. See LICENSE file in the project root for full license information.

using NetTopologySuite.Geometries;
using Viae.Domain.Models;

namespace Viae.Presentation.Mvc.Tests.Builders;

/// <summary>
/// Builder for creating Locus test instances with fluent configuration.
/// </summary>
public class LocusBuilder
{
    private string _id = "TestId123";
    private string _name = "Test Location";
    private string? _slug = "test-location";
    private Point _coordinates = new(0, 0) { SRID = 4326 };
    private string? _content = "Test content";
    private string? _address = "123 Test St";
    private string? _city = "Test City";
    private string? _country = "Test Country";
    private DateTime _createdAt = DateTime.UtcNow;

    /// <summary>
    /// Sets the ID.
    /// </summary>
    public LocusBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    /// <summary>
    /// Sets the name.
    /// </summary>
    public LocusBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the slug.
    /// </summary>
    public LocusBuilder WithSlug(string? slug)
    {
        _slug = slug;
        return this;
    }

    /// <summary>
    /// Sets the slug to null.
    /// </summary>
    public LocusBuilder WithoutSlug()
    {
        _slug = null;
        return this;
    }

    /// <summary>
    /// Sets the coordinates using latitude and longitude.
    /// </summary>
    public LocusBuilder WithCoordinates(double latitude, double longitude)
    {
        _coordinates = new Point(longitude, latitude) { SRID = 4326 };
        return this;
    }

    /// <summary>
    /// Sets the coordinates using a Point.
    /// </summary>
    public LocusBuilder WithCoordinates(Point coordinates)
    {
        _coordinates = coordinates;
        return this;
    }

    /// <summary>
    /// Sets the content.
    /// </summary>
    public LocusBuilder WithContent(string? content)
    {
        _content = content;
        return this;
    }

    /// <summary>
    /// Sets the address.
    /// </summary>
    public LocusBuilder WithAddress(string? address)
    {
        _address = address;
        return this;
    }

    /// <summary>
    /// Sets the city.
    /// </summary>
    public LocusBuilder WithCity(string? city)
    {
        _city = city;
        return this;
    }

    /// <summary>
    /// Sets the country.
    /// </summary>
    public LocusBuilder WithCountry(string? country)
    {
        _country = country;
        return this;
    }

    /// <summary>
    /// Sets the created at timestamp.
    /// </summary>
    public LocusBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    /// <summary>
    /// Builds the Locus instance.
    /// </summary>
    public Locus Build()
    {
        return new Locus
        {
            Id = _id,
            Name = _name,
            Slug = _slug,
            Coordinates = _coordinates,
            Content = _content,
            Address = _address,
            City = _city,
            Country = _country,
            CreatedAt = _createdAt,
        };
    }

    /// <summary>
    /// Creates a new default builder instance.
    /// </summary>
    public static LocusBuilder Default() => new();

    /// <summary>
    /// Creates a builder with minimal required fields only.
    /// </summary>
    public static LocusBuilder Minimal() =>
        new LocusBuilder()
            .WithContent(null)
            .WithAddress(null)
            .WithCity(null)
            .WithCountry(null)
            .WithoutSlug();
}
