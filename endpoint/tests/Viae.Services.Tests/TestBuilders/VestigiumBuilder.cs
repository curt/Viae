// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Services.Tests.TestBuilders;

/// <summary>
/// Fluent builder for creating Vestigium test data.
/// </summary>
public class VestigiumBuilder
{
    private string _id = Guid.NewGuid().ToString("N")[..12]; // Short test ID
    private string _personaId = Guid.NewGuid().ToString("N")[..12];
    private Persona? _persona;
    private string _locusId = Guid.NewGuid().ToString("N")[..12];
    private Locus? _locus;
    private DateTime _happenedAt = new(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
    private Origo _origo = Origo.Domesticus;
    private string _content = "Default vestigium content";
    private Uri _uri = new("https://example.com/vestigia/default");
    private DateTime _createdAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private DateTime? _updatedAt;
    private DateTime? _publishedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private DateTime? _tombstonedAt;

    /// <summary>
    /// Creates a new VestigiumBuilder with default values.
    /// </summary>
    public static VestigiumBuilder Default() => new();

    /// <summary>
    /// Sets the ID of the vestigium.
    /// </summary>
    public VestigiumBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    /// <summary>
    /// Sets the persona ID of the vestigium.
    /// </summary>
    public VestigiumBuilder WithPersonaId(string personaId)
    {
        _personaId = personaId;
        return this;
    }

    /// <summary>
    /// Sets the persona of the vestigium.
    /// </summary>
    public VestigiumBuilder WithPersona(Persona persona)
    {
        _persona = persona;
        _personaId = persona.Id;
        return this;
    }

    /// <summary>
    /// Sets the locus ID of the vestigium.
    /// </summary>
    public VestigiumBuilder WithLocusId(string locusId)
    {
        _locusId = locusId;
        return this;
    }

    /// <summary>
    /// Sets the locus of the vestigium.
    /// </summary>
    public VestigiumBuilder WithLocus(Locus locus)
    {
        _locus = locus;
        _locusId = locus.Id;
        return this;
    }

    /// <summary>
    /// Sets when the event happened.
    /// </summary>
    public VestigiumBuilder WithHappenedAt(DateTime happenedAt)
    {
        _happenedAt = happenedAt;
        return this;
    }

    /// <summary>
    /// Sets the origin of the vestigium.
    /// </summary>
    public VestigiumBuilder WithOrigo(Origo origo)
    {
        _origo = origo;
        return this;
    }

    /// <summary>
    /// Sets the content of the vestigium.
    /// </summary>
    public VestigiumBuilder WithContent(string content)
    {
        _content = content;
        return this;
    }

    /// <summary>
    /// Sets the URI of the vestigium.
    /// </summary>
    public VestigiumBuilder WithUri(Uri uri)
    {
        _uri = uri;
        return this;
    }

    /// <summary>
    /// Sets the created date of the vestigium.
    /// </summary>
    public VestigiumBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    /// <summary>
    /// Sets the updated date of the vestigium.
    /// </summary>
    public VestigiumBuilder WithUpdatedAt(DateTime? updatedAt)
    {
        _updatedAt = updatedAt;
        return this;
    }

    /// <summary>
    /// Sets the published date of the vestigium.
    /// </summary>
    public VestigiumBuilder WithPublishedAt(DateTime? publishedAt)
    {
        _publishedAt = publishedAt;
        return this;
    }

    /// <summary>
    /// Sets the tombstoned date of the vestigium.
    /// </summary>
    public VestigiumBuilder WithTombstonedAt(DateTime? tombstonedAt)
    {
        _tombstonedAt = tombstonedAt;
        return this;
    }

    /// <summary>
    /// Configures the vestigium as a draft (Latens status).
    /// </summary>
    public VestigiumBuilder AsDraft()
    {
        _publishedAt = null;
        _tombstonedAt = null;
        return this;
    }

    /// <summary>
    /// Configures the vestigium as published (Manifestus status).
    /// </summary>
    public VestigiumBuilder AsPublished()
    {
        _publishedAt = _createdAt;
        _tombstonedAt = null;
        return this;
    }

    /// <summary>
    /// Configures the vestigium as tombstoned (Deletus status).
    /// </summary>
    public VestigiumBuilder AsTombstoned()
    {
        _publishedAt = _createdAt;
        _tombstonedAt = _createdAt.AddDays(1);
        return this;
    }

    /// <summary>
    /// Configures the vestigium as local (Domesticus).
    /// </summary>
    public VestigiumBuilder AsLocal()
    {
        _origo = Origo.Domesticus;
        return this;
    }

    /// <summary>
    /// Configures the vestigium as remote (Externus).
    /// </summary>
    public VestigiumBuilder AsRemote()
    {
        _origo = Origo.Externus;
        return this;
    }

    /// <summary>
    /// Builds the Vestigium with the configured values.
    /// </summary>
    public Vestigium Build()
    {
        var vestigium = new Vestigium
        {
            Id = _id,
            PersonaId = _personaId,
            LocusId = _locusId,
            HappenedAt = _happenedAt,
            Origo = _origo,
            Content = _content,
            Uri = _uri,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt,
            PublishedAt = _publishedAt,
            TombstonedAt = _tombstonedAt,
        };

        if (_persona != null)
        {
            vestigium.Persona = _persona;
        }

        if (_locus != null)
        {
            vestigium.Locus = _locus;
        }

        return vestigium;
    }

    #region Factory Methods

    /// <summary>
    /// Creates a simple vestigium for basic tests.
    /// </summary>
    public static VestigiumBuilder Simple(string content = "Test vestigium") =>
        Default().WithContent(content);

    /// <summary>
    /// Creates a published vestigium that happened recently.
    /// </summary>
    public static Vestigium RecentPublished(string content = "Recent event") =>
        Simple(content).WithHappenedAt(DateTime.UtcNow.AddHours(-2)).AsPublished().Build();

    /// <summary>
    /// Creates a draft vestigium.
    /// </summary>
    public static Vestigium Draft(string content = "Draft event") =>
        Simple(content).AsDraft().Build();

    /// <summary>
    /// Creates a tombstoned vestigium.
    /// </summary>
    public static Vestigium Tombstoned(string content = "Deleted event") =>
        Simple(content).AsTombstoned().Build();

    /// <summary>
    /// Creates a vestigium for a specific persona at a specific locus.
    /// </summary>
    public static Vestigium ForPersonaAtLocus(
        Persona persona,
        Locus locus,
        string content = "Event"
    ) => Simple(content).WithPersona(persona).WithLocus(locus).AsPublished().Build();

    #endregion
}
