// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Viae.ActivityPub;

namespace Viae.Presentation.Mvc.ContentNegotiation;

/// <summary>
/// Implements content negotiation by matching request Accept headers to registered handlers.
/// </summary>
/// <typeparam name="TModel">The type of model to be negotiated.</typeparam>
public class ContentNegotiator<TModel>(
    TModel model,
    HttpRequest request,
    JsonSerializerOptions jsonSerializerOptions
) : IContentNegotiator<TModel>
    where TModel : notnull
{
    private readonly Dictionary<string, Func<TModel, Task<IActionResult>>> _handlers = [];
    private Func<TModel, Task<IActionResult>>? _defaultHandler;

    public IContentNegotiator<TModel> ForMediaType(
        string mediaType,
        Func<TModel, Task<IActionResult>> handler
    )
    {
        _handlers[mediaType.ToLowerInvariant()] = handler;
        return this;
    }

    public IContentNegotiator<TModel> ForGeoJson<TResultModel>(
        Func<TModel, Task<TResultModel>> transform
    )
    {
        return ForMediaType(
            "application/geo+json",
            async model =>
            {
                var result = await transform(model);
                var serializer = NetTopologySuite.IO.GeoJsonSerializer.Create();
                using var stringWriter = new StringWriter();
                serializer.Serialize(stringWriter, result);
                var geoJson = stringWriter.ToString();
                return new ContentResult
                {
                    Content = geoJson,
                    ContentType = "application/geo+json",
                    StatusCode = 200,
                };
            }
        );
    }

    public IContentNegotiator<TModel> ForJson()
    {
        return ForMediaType(
            "application/json",
            model => Task.FromResult<IActionResult>(new JsonResult(model, jsonSerializerOptions))
        );
    }

    public IContentNegotiator<TModel> ForJson<TResultModel>(Func<TModel, TResultModel> transform)
    {
        return ForMediaType(
            "application/json",
            model =>
                Task.FromResult<IActionResult>(
                    new JsonResult(transform(model), jsonSerializerOptions)
                )
        );
    }

    public IContentNegotiator<TModel> ForJson<TResultModel>(
        Func<TModel, Task<TResultModel>> transform
    )
    {
        return ForMediaType(
            "application/json",
            async model => new JsonResult(await transform(model), jsonSerializerOptions)
        );
    }

    public IContentNegotiator<TModel> ForActivityPub<TResultModel>(
        Func<TModel, TResultModel> transform
    )
        where TResultModel : ActivityObject
    {
        ForMediaType(
            "application/activity+json",
            model => Task.FromResult<IActionResult>(ActivityJsonResult(transform(model)))
        );
        return ForMediaType(
            "application/ld+json",
            model =>
                Task.FromResult<IActionResult>(
                    ActivityJsonResult(
                        transform(model),
                        contentType: "application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\""
                    )
                )
        );
    }

    public IContentNegotiator<TModel> ForActivityPub<TResultModel>(
        Func<TModel, Task<TResultModel>> transform
    )
        where TResultModel : ActivityObject
    {
        ForMediaType(
            "application/activity+json",
            async model => ActivityJsonResult(await transform(model))
        );
        return ForMediaType(
            "application/ld+json",
            async model =>
                ActivityJsonResult(
                    await transform(model),
                    contentType: "application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\""
                )
        );
    }

    public IContentNegotiator<TModel> ForHtml(string viewName)
    {
        return ForHtml(viewName, l => Task.FromResult(l));
    }

    public IContentNegotiator<TModel> ForHtml<TViewModel>(
        string viewName,
        Func<TModel, Task<TViewModel>> transform
    )
    {
        return ForMediaType(
            "text/html",
            async model => new ViewResult
            {
                ViewName = viewName,
                ViewData = new ViewDataDictionary(
                    new EmptyModelMetadataProvider(),
                    new ModelStateDictionary()
                )
                {
                    Model = await transform(model),
                },
            }
        );
    }

    public IContentNegotiator<TModel> ForRss(Func<TModel, string> transform)
    {
        return ForMediaType(
            "application/rss+xml",
            model =>
                Task.FromResult<IActionResult>(
                    new ContentResult
                    {
                        Content = transform(model),
                        ContentType = "application/rss+xml",
                        StatusCode = 200,
                    }
                )
        );
    }

    public IContentNegotiator<TModel> ForAtom(Func<TModel, string> transform)
    {
        return ForMediaType(
            "application/atom+xml",
            model =>
                Task.FromResult<IActionResult>(
                    new ContentResult
                    {
                        Content = transform(model),
                        ContentType = "application/atom+xml",
                        StatusCode = 200,
                    }
                )
        );
    }

    public IContentNegotiator<TModel> WithDefault(Func<TModel, Task<IActionResult>> handler)
    {
        _defaultHandler = handler;
        return this;
    }

    public async Task<IActionResult> ExecuteAsync(Func<TModel, string>? canonicalPath = null)
    {
        var acceptHeader = request.Headers.Accept.ToString().ToLowerInvariant();

        foreach (var (mediaType, handler) in _handlers)
        {
            if (acceptHeader.Contains(mediaType))
            {
                if (mediaType == "application/json" && acceptHeader.Contains("activity"))
                {
                    continue;
                }

                if (canonicalPath != null && mediaType == "text/html")
                {
                    if (request.Path != canonicalPath(model))
                    {
                        return new RedirectResult(canonicalPath(model), true);
                    }
                }

                return await handler(model);
            }
        }

        var fallbackHandler = _defaultHandler ?? _handlers.Values.FirstOrDefault();

        return fallbackHandler != null ? await fallbackHandler(model) : new StatusCodeResult(406);
    }

    private ContentResult ActivityJsonResult<T>(
        T value,
        JsonSerializerOptions? options = null,
        string? contentType = "application/activity+json"
    )
        where T : ActivityObject
    {
        options ??= jsonSerializerOptions;
        var json = JsonSerializer.Serialize<ActivityObject>(value, options);

        return new ContentResult
        {
            Content = json,
            ContentType = contentType,
            StatusCode = 200,
        };
    }
}
