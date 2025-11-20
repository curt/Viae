// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using NetTopologySuite.Geometries;
using Viae.Domain.Models;

namespace Viae.Domain.Tests.Helpers;

/// <summary>
/// Provides fluent builder methods for creating test data with sensible defaults.
/// Promotes DRY principle by centralizing test data creation.
/// </summary>
public static class TestDataBuilders
{
    /// <summary>
    /// Creates a Persona builder with default values for testing.
    /// </summary>
    public static PersonaBuilder APersona() => new();

    /// <summary>
    /// Creates a Folium builder with default values for testing.
    /// </summary>
    public static FoliumBuilder AFolium() => new();

    /// <summary>
    /// Creates a Locus builder with default values for testing.
    /// </summary>
    public static LocusBuilder ALocus() => new();

    /// <summary>
    /// Creates a Vestigium builder with default values for testing.
    /// </summary>
    public static VestigiumBuilder AVestigium() => new();

    /// <summary>
    /// Creates a Thema builder with default values for testing.
    /// </summary>
    public static ThemaBuilder AThema() => new();

    /// <summary>
    /// Creates a Point for testing with default coordinates (San Francisco).
    /// </summary>
    public static Point APoint(double latitude = 37.7749, double longitude = -122.4194)
    {
        var geometryFactory = new GeometryFactory(new PrecisionModel(), 4326);
        return geometryFactory.CreatePoint(new Coordinate(longitude, latitude));
    }
}

public class PersonaBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private Origo _origo = Origo.Domesticus;
    private string _username = "testuser";
    private string _displayName = "Test User";
    private string? _email = "test@example.com";
    private readonly Guid? _cognitoUserId = Guid.NewGuid();
    private string? _bio;
    private readonly DateTime _createdAt = DateTime.UtcNow;
    private Uri? _uri;
    private string? _publicKey;
    private string? _privateKey;

    public PersonaBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public PersonaBuilder WithOrigo(Origo origo)
    {
        _origo = origo;
        return this;
    }

    public PersonaBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    public PersonaBuilder WithDisplayName(string displayName)
    {
        _displayName = displayName;
        return this;
    }

    public PersonaBuilder WithEmail(string email)
    {
        _email = email;
        return this;
    }

    public PersonaBuilder WithBio(string bio)
    {
        _bio = bio;
        return this;
    }

    public PersonaBuilder WithUri(Uri uri)
    {
        _uri = uri;
        return this;
    }

    public PersonaBuilder WithKeys(string publicKey, string privateKey)
    {
        _publicKey = publicKey;
        _privateKey = privateKey;
        return this;
    }

    public Persona Build() =>
        new()
        {
            Id = _id,
            Origo = _origo,
            Username = _username,
            DisplayName = _displayName,
            Email = _email,
            CognitoUserId = _cognitoUserId,
            Bio = _bio,
            AvatarUrl = null,
            CreatedAt = _createdAt,
            UpdatedAt = null,
            Uri = _uri,
            Url = null,
            PublicKey = _publicKey,
            PrivateKey = _privateKey,
        };
}

public class FoliumBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private readonly Origo _origo = Origo.Domesticus;
    private string _title = "Test Post";
    private string _content = "Test content";
    private string _personaId = Guid.NewGuid().ToString();
    private readonly DateTime _createdAt = DateTime.UtcNow;
    private DateTime? _publishedAt;
    private bool _isDraft = true;

    public FoliumBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public FoliumBuilder WithTitle(string title)
    {
        _title = title;
        return this;
    }

    public FoliumBuilder WithContent(string content)
    {
        _content = content;
        return this;
    }

    public FoliumBuilder WithPersonaId(string personaId)
    {
        _personaId = personaId;
        return this;
    }

    public FoliumBuilder Published()
    {
        _publishedAt = DateTime.UtcNow;
        _isDraft = false;
        return this;
    }

    public FoliumBuilder Draft()
    {
        _publishedAt = null;
        _isDraft = true;
        return this;
    }

    public Folium Build() =>
        new()
        {
            Id = _id,
            Origo = _origo,
            Title = _title,
            Content = _content,
            PersonaId = _personaId,
            CreatedAt = _createdAt,
            PublishedAt = _publishedAt,
            UpdatedAt = null,
            IsDraft = _isDraft,
            Uri = null,
            Url = null,
        };
}

public class LocusBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private OsmId? _osmId;
    private string _name = "Test Location";
    private string _slug = "test-location";
    private string? _address = "123 Test St";
    private string? _city = "San Francisco";
    private string? _country = "USA";
    private Point _coordinates = TestDataBuilders.APoint();
    private readonly DateTime _createdAt = DateTime.UtcNow;

    public LocusBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public LocusBuilder WithOsmId(OsmId osmId)
    {
        _osmId = osmId;
        return this;
    }

    public LocusBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public LocusBuilder WithSlug(string slug)
    {
        _slug = slug;
        return this;
    }

    public LocusBuilder WithCoordinates(double latitude, double longitude)
    {
        _coordinates = TestDataBuilders.APoint(latitude, longitude);
        return this;
    }

    public LocusBuilder WithCoordinates(Point coordinates)
    {
        _coordinates = coordinates;
        return this;
    }

    public LocusBuilder WithAddress(string address, string city, string country)
    {
        _address = address;
        _city = city;
        _country = country;
        return this;
    }

    public Locus Build() =>
        new()
        {
            Id = _id,
            OsmId = _osmId,
            Name = _name,
            Slug = _slug,
            Content = null,
            Address = _address,
            City = _city,
            Country = _country,
            Coordinates = _coordinates,
            CreatedAt = _createdAt,
        };
}

public class VestigiumBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private string _personaId = Guid.NewGuid().ToString();
    private string _locusId = Guid.NewGuid().ToString();
    private DateTime _happenedAt = DateTime.UtcNow;
    private readonly Origo _origo = Origo.Domesticus;
    private string? _content;
    private readonly DateTime _createdAt = DateTime.UtcNow;
    private DateTime? _publishedAt;
    private DateTime? _tombstonedAt;

    public VestigiumBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public VestigiumBuilder WithPersonaId(string personaId)
    {
        _personaId = personaId;
        return this;
    }

    public VestigiumBuilder WithLocusId(string locusId)
    {
        _locusId = locusId;
        return this;
    }

    public VestigiumBuilder WithHappenedAt(DateTime happenedAt)
    {
        _happenedAt = happenedAt;
        return this;
    }

    public VestigiumBuilder WithContent(string content)
    {
        _content = content;
        return this;
    }

    public VestigiumBuilder Published()
    {
        _publishedAt = DateTime.UtcNow;
        return this;
    }

    public VestigiumBuilder Tombstoned()
    {
        _tombstonedAt = DateTime.UtcNow;
        return this;
    }

#pragma warning disable CS8601 // Possible null reference assignment.
#pragma warning disable CS8625 // Cannot convert null literal to non-nullable reference type.
    public Vestigium Build() =>
        new()
        {
            Id = _id,
            PersonaId = _personaId,
            LocusId = _locusId,
            HappenedAt = _happenedAt,
            Origo = _origo,
            Content = _content,
            CreatedAt = _createdAt,
            PublishedAt = _publishedAt,
            UpdatedAt = null,
            TombstonedAt = _tombstonedAt,
            Uri = null,
        };
#pragma warning restore CS8625 // Cannot convert null literal to non-nullable reference type.
#pragma warning restore CS8601 // Possible null reference assignment.
}

public class ThemaBuilder
{
    private string _id = Guid.NewGuid().ToString();
    private string _name = "Test Theme";
    private string _slug = "test-theme";
    private readonly DateTime _createdAt = DateTime.UtcNow;

    public ThemaBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    public ThemaBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    public ThemaBuilder WithSlug(string slug)
    {
        _slug = slug;
        return this;
    }

    public Thema Build() =>
        new()
        {
            Id = _id,
            Name = _name,
            Slug = _slug,
            CreatedAt = _createdAt,
        };
}
