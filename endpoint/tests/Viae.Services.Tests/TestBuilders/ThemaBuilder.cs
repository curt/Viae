// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Services.Tests.TestBuilders;

/// <summary>
/// Fluent builder for creating Thema test data.
/// </summary>
public class ThemaBuilder
{
    private string _id = Guid.NewGuid().ToString("N")[..12]; // Short test ID
    private string _name = "Default Thema";
    private string _slug = "default-thema";
    private DateTime _createdAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    /// <summary>
    /// Creates a new ThemaBuilder with default values.
    /// </summary>
    public static ThemaBuilder Default() => new();

    /// <summary>
    /// Sets the ID of the thema.
    /// </summary>
    public ThemaBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    /// <summary>
    /// Sets the name of the thema.
    /// </summary>
    public ThemaBuilder WithName(string name)
    {
        _name = name;
        return this;
    }

    /// <summary>
    /// Sets the slug of the thema.
    /// </summary>
    public ThemaBuilder WithSlug(string slug)
    {
        _slug = slug;
        return this;
    }

    /// <summary>
    /// Sets the created date of the thema.
    /// </summary>
    public ThemaBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    /// <summary>
    /// Sets both name and generates a slug from the name.
    /// </summary>
    public ThemaBuilder WithNameAndSlug(string name)
    {
        _name = name;
        _slug = name.ToLowerInvariant().Replace(" ", "-");
        return this;
    }

    /// <summary>
    /// Builds the Thema with the configured values.
    /// </summary>
    public Thema Build()
    {
        return new Thema
        {
            Id = _id,
            Name = _name,
            Slug = _slug,
            CreatedAt = _createdAt,
        };
    }

    #region Factory Methods

    /// <summary>
    /// Creates a thema for Nature category.
    /// </summary>
    public static Thema Nature() => Default().WithNameAndSlug("Nature").Build();

    /// <summary>
    /// Creates a thema for Culture category.
    /// </summary>
    public static Thema Culture() => Default().WithNameAndSlug("Culture").Build();

    /// <summary>
    /// Creates a thema for History category.
    /// </summary>
    public static Thema History() => Default().WithNameAndSlug("History").Build();

    /// <summary>
    /// Creates a thema for Art category.
    /// </summary>
    public static Thema Art() => Default().WithNameAndSlug("Art").Build();

    /// <summary>
    /// Creates a thema for Architecture category.
    /// </summary>
    public static Thema Architecture() => Default().WithNameAndSlug("Architecture").Build();

    /// <summary>
    /// Creates a thema for Food category.
    /// </summary>
    public static Thema Food() => Default().WithNameAndSlug("Food").Build();

    /// <summary>
    /// Creates a thema for Entertainment category.
    /// </summary>
    public static Thema Entertainment() => Default().WithNameAndSlug("Entertainment").Build();

    /// <summary>
    /// Creates a simple thema with the given name.
    /// </summary>
    public static Thema Simple(string name = "Simple Thema") =>
        Default().WithNameAndSlug(name).Build();

    #endregion
}
