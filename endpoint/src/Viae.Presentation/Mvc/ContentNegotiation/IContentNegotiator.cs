// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.AspNetCore.Mvc;
using Viae.ActivityPub;

namespace Viae.Presentation.Mvc.ContentNegotiation;

/// <summary>
/// Provides a fluent interface for configuring content negotiation handlers.
/// </summary>
/// <typeparam name="TModel">The type of model to be negotiated.</typeparam>
public interface IContentNegotiator<TModel>
    where TModel : notnull
{
    /// <summary>
    /// Registers a handler for GeoJSON format (application/geo+json).
    /// </summary>
    /// <param name="transform">Function to transform the model into GeoJSON-compatible format.</param>
    /// <returns>The negotiator for method chaining.</returns>
    IContentNegotiator<TModel> ForGeoJson<TResultModel>(Func<TModel, Task<TResultModel>> transform);

    /// <summary>
    /// Registers a handler for plain JSON format (application/json) without transformation.
    /// </summary>
    /// <returns>The negotiator for method chaining.</returns>
    IContentNegotiator<TModel> ForJson();

    /// <summary>
    /// Registers a handler for plain JSON format (application/json) with synchronous transformation.
    /// </summary>
    /// <param name="transform">Function to transform the model.</param>
    /// <returns>The negotiator for method chaining.</returns>
    IContentNegotiator<TModel> ForJson<TResultModel>(Func<TModel, TResultModel> transform);

    /// <summary>
    /// Registers a handler for plain JSON format (application/json) with asynchronous transformation.
    /// </summary>
    /// <param name="transform">Function to transform the model.</param>
    /// <returns>The negotiator for method chaining.</returns>
    IContentNegotiator<TModel> ForJson<TResultModel>(Func<TModel, Task<TResultModel>> transform);

    /// <summary>
    /// Registers a handler for ActivityPub/JSON-LD format (application/activity+json, application/ld+json) with synchronous transformation.
    /// </summary>
    /// <param name="transform">Function to transform the model into ActivityPub format.</param>
    /// <returns>The negotiator for method chaining.</returns>
    IContentNegotiator<TModel> ForActivityPub<TResultModel>(Func<TModel, TResultModel> transform)
        where TResultModel : ActivityObject;

    /// <summary>
    /// Registers a handler for ActivityPub/JSON-LD format (application/activity+json, application/ld+json) with asynchronous transformation.
    /// </summary>
    /// <param name="transform">Function to transform the model into ActivityPub format.</param>
    /// <returns>The negotiator for method chaining.</returns>
    IContentNegotiator<TModel> ForActivityPub<TResultModel>(
        Func<TModel, Task<TResultModel>> transform
    )
        where TResultModel : ActivityObject;

    /// <summary>
    /// Registers a handler for HTML format (text/html).
    /// </summary>
    /// <param name="viewName">The name of the view to render.</param>
    /// <returns>The negotiator for method chaining.</returns>
    IContentNegotiator<TModel> ForHtml(string viewName);

    /// <summary>
    /// Registers a handler for HTML format (text/html).
    /// </summary>
    /// <param name="viewName">The name of the view to render.</param>
    /// <param name="transform">Function to transform the model into a view model.</param>
    /// <returns>The negotiator for method chaining.</returns>
    IContentNegotiator<TModel> ForHtml<TViewModel>(
        string viewName,
        Func<TModel, Task<TViewModel>> transform
    );

    /// <summary>
    /// Registers a handler for RSS format (application/rss+xml).
    /// </summary>
    /// <param name="transform">Function to transform the model into RSS XML string.</param>
    /// <returns>The negotiator for method chaining.</returns>
    IContentNegotiator<TModel> ForRss(Func<TModel, string> transform);

    /// <summary>
    /// Registers a handler for Atom format (application/atom+xml).
    /// </summary>
    /// <param name="transform">Function to transform the model into Atom XML string.</param>
    /// <returns>The negotiator for method chaining.</returns>
    IContentNegotiator<TModel> ForAtom(Func<TModel, string> transform);

    /// <summary>
    /// Registers a custom handler for a specific media type.
    /// </summary>
    /// <param name="mediaType">The media type to handle (e.g., "application/xml").</param>
    /// <param name="handler">Function that returns an IActionResult for this media type.</param>
    /// <returns>The negotiator for method chaining.</returns>
    IContentNegotiator<TModel> ForMediaType(
        string mediaType,
        Func<TModel, Task<IActionResult>> handler
    );

    /// <summary>
    /// Sets a default handler when no Accept header matches registered handlers.
    /// If not set, the first registered handler will be used as default.
    /// </summary>
    /// <param name="handler">Function that returns an IActionResult as the default.</param>
    /// <returns>The negotiator for method chaining.</returns>
    IContentNegotiator<TModel> WithDefault(Func<TModel, Task<IActionResult>> handler);

    /// <summary>
    /// Executes content negotiation based on the request's Accept header.
    /// </summary>
    /// <returns>An IActionResult appropriate for the requested content type.</returns>
    Task<IActionResult> ExecuteAsync(Func<TModel, string>? canonicalPath = null);
}
