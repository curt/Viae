// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Viae.Presentation.Mvc.ContentNegotiation;

/// <summary>
/// Base controller providing content negotiation capabilities.
/// </summary>
public abstract class ContentNegotiatingController(IOptions<JsonOptions> jsonOptions) : Controller
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = jsonOptions
        .Value
        .JsonSerializerOptions;

    /// <summary>
    /// Performs content negotiation for the given model using a fluent configuration API.
    /// </summary>
    /// <typeparam name="TModel">The type of model to negotiate.</typeparam>
    /// <param name="model">The model to be returned in various formats.</param>
    /// <param name="canonicalPath">The canonical request path for the model.</param>
    /// <param name="configure">Configuration action for setting up content type handlers.</param>
    /// <returns>An IActionResult appropriate for the requested content type.</returns>
    /// <example>
    /// <code>
    /// return await NegotiateContent(locus, "/loca/abcde123456/place-slug", n => n
    ///     .ForGeoJson(l => CreateGeoJsonFeature(l))
    ///     .ForJson(l => l)
    ///     .ForActivityPub(l => new { })
    ///     .ForHtml("Show")
    /// );
    /// </code>
    /// </example>
    protected async Task<IActionResult> NegotiateContent<TModel>(
        TModel? model,
        Func<TModel, string> canonicalPath,
        Action<IContentNegotiator<TModel>> configure
    )
        where TModel : notnull
    {
        if (model == null)
        {
            return NotFound();
        }

        var negotiator = new ContentNegotiator<TModel>(model, Request, _jsonSerializerOptions);
        configure(negotiator);

        return await negotiator.ExecuteAsync(canonicalPath);
    }

    /// <summary>
    /// Performs content negotiation for a collection of models.
    /// Useful for index/list actions that need to handle collections differently than single items.
    /// </summary>
    /// <typeparam name="TModel">The type of model in the collection.</typeparam>
    /// <param name="models">The collection of models to be returned in various formats.</param>
    /// <param name="configure">Configuration action for setting up content type handlers.</param>
    /// <returns>An IActionResult appropriate for the requested content type.</returns>
    protected async Task<IActionResult> NegotiateContent<TModel>(
        IEnumerable<TModel> models,
        Action<IContentNegotiator<IEnumerable<TModel>>> configure
    )
    {
        var negotiator = new ContentNegotiator<IEnumerable<TModel>>(
            models,
            Request,
            _jsonSerializerOptions
        );
        configure(negotiator);
        return await negotiator.ExecuteAsync();
    }
}
