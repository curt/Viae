// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.Extensions.Options;
using Viae.Domain.Models;

namespace Viae.Persistence;

/// <summary>
/// Default implementation of IUriGeneratorService that generates URIs based on entity type and ID.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="UriGeneratorService"/> class.
/// </remarks>
/// <param name="options">Configuration options containing the base URI.</param>
public class UriGeneratorService(IOptions<UriGeneratorOptions> options) : IUriGeneratorService
{
    private readonly UriGeneratorOptions _options = options.Value;

    /// <inheritdoc />
    public Uri BaseUri => _options.BaseUri;

    /// <inheritdoc />
    public Uri GenerateUri(Admissio entity)
    {
        return string.IsNullOrEmpty(entity.Id)
            ? throw new InvalidOperationException(
                "Cannot generate URI for Admissio: Id has not been assigned yet."
            )
            : new Uri($"{BaseUri.AbsoluteUri.TrimEnd('/')}/ping/{entity.Id}");
    }

    /// <inheritdoc />
    public Uri GenerateUri(Adreflexio entity)
    {
        return string.IsNullOrEmpty(entity.Id)
            ? throw new InvalidOperationException(
                "Cannot generate URI for Adreflexio: Id has not been assigned yet."
            )
            : new Uri($"{BaseUri.AbsoluteUri.TrimEnd('/')}/pong/{entity.Id}");
    }
}

/// <summary>
/// Configuration options for the URI generator service.
/// </summary>
public class UriGeneratorOptions
{
    /// <summary>
    /// Gets or sets the base URI for generating entity URIs.
    /// Example: "https://example.com"
    /// </summary>
    public Uri BaseUri { get; set; } = new Uri("https://localhost");
}
