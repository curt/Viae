// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.Options;
using Moq;
using Viae.ActivityPub;
using Viae.Domain.Criteria;
using Viae.Domain.Models;
using Viae.Domain.Services;
using Viae.Presentation.Mvc.Controllers;

namespace Viae.Presentation.Mvc.Tests.Controllers;

/// <summary>
/// Base class for LocaController tests providing common setup and helper methods.
/// </summary>
public abstract class LocaControllerTestBase
{
    protected Mock<ILocusService> MockLocusService { get; }
    protected LocaController Controller { get; }
    protected JsonSerializerOptions JsonSerializerOptions { get; }

    protected LocaControllerTestBase()
    {
        MockLocusService = new Mock<ILocusService>();
        var jsonOptions = Options.Create(new JsonOptions());

        // Register ActivityObjectJsonConverter to match production configuration
        jsonOptions.Value.JsonSerializerOptions.Converters.Add(new ActivityObjectJsonConverter());

        JsonSerializerOptions = jsonOptions.Value.JsonSerializerOptions;
        Controller = new LocaController(MockLocusService.Object, jsonOptions);
        SetupControllerContext();
    }

    /// <summary>
    /// Sets up the controller context with a default HTTP context and URL helper.
    /// </summary>
    private void SetupControllerContext()
    {
        var httpContext = new DefaultHttpContext();
        Controller.ControllerContext = new ControllerContext { HttpContext = httpContext };

        // Setup URL helper mock
        var mockUrlHelper = new Mock<IUrlHelper>();

        // Mock RouteUrl(UrlRouteContext) - the actual interface method
        // The extension method RouteUrl(string, object) internally calls this
        mockUrlHelper
            .Setup(x => x.RouteUrl(It.IsAny<UrlRouteContext>()))
            .Returns(
                (UrlRouteContext context) =>
                {
                    if (context.Values == null)
                    {
                        return "/loca/unknown";
                    }

                    var routeValues =
                        context.Values as Microsoft.AspNetCore.Routing.RouteValueDictionary
                        ?? new Microsoft.AspNetCore.Routing.RouteValueDictionary(context.Values);

                    var id = routeValues.TryGetValue("id", out var idValue)
                        ? idValue?.ToString()
                        : "unknown";
                    var catchAll = routeValues.TryGetValue("catchAll", out var catchAllValue)
                        ? catchAllValue?.ToString()
                        : null;

                    return string.IsNullOrEmpty(catchAll)
                        ? $"/loca/{id}"
                        : $"/loca/{id}/{catchAll}";
                }
            );

        Controller.Url = mockUrlHelper.Object;
    }

    /// <summary>
    /// Sets the Accept header on the controller's request.
    /// </summary>
    protected void SetAcceptHeader(string acceptHeader)
    {
        Controller.HttpContext.Request.Headers.Accept = acceptHeader;
    }

    /// <summary>
    /// Sets the request path on the controller's request.
    /// </summary>
    protected void SetRequestPath(string path)
    {
        Controller.HttpContext.Request.Path = new PathString(path);
    }

    protected void SetSchemeHost(string scheme, string host)
    {
        Controller.HttpContext.Request.Scheme = scheme;
        Controller.HttpContext.Request.Host = new HostString(host);
    }

    /// <summary>
    /// Creates a cancellation token source with default timeout for testing.
    /// </summary>
    protected static CancellationTokenSource CreateCancellationTokenSource() =>
        new(TimeSpan.FromSeconds(5));

    /// <summary>
    /// Sets up mock for GetLocaAsync with the specified loca list.
    /// </summary>
    /// <param name="loca">The loca list to return.</param>
    protected void SetupGetLocaAsync(IReadOnlyList<Locus> loca)
    {
        MockLocusService
            .Setup(s => s.GetLocaAsync(It.IsAny<LocusCriteria>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(loca);
    }

    /// <summary>
    /// Sets up mock for GetLocusByIdAsync with the specified locus.
    /// </summary>
    /// <param name="id">The locus ID to match.</param>
    /// <param name="locus">The locus to return (null for not found scenarios).</param>
    protected void SetupGetLocusByIdAsync(string id, Locus? locus)
    {
        MockLocusService
            .Setup(s => s.GetLocusByIdAsync(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(locus);
    }

    /// <summary>
    /// Sets up mocks for nearby loca queries with optional test data.
    /// </summary>
    /// <param name="nearbyLoca">The nearby loca distances to return. If null, returns an empty list.</param>
    protected void SetupNearbyLocaMocks(IReadOnlyList<LocusDistance>? nearbyLoca = null)
    {
        MockLocusService
            .Setup(s =>
                s.GetLocaDistancesAsync(
                    It.IsAny<LocusCriteria>(),
                    It.IsAny<Locus>(),
                    It.IsAny<CancellationToken>()
                )
            )
            .ReturnsAsync(nearbyLoca ?? []);
    }

    /// <summary>
    /// Asserts that a ContentResult is a valid GeoJSON response with expected content.
    /// </summary>
    /// <param name="contentResult">The ContentResult to assert.</param>
    /// <param name="expectedContent">Optional array of strings that should be present in the content.</param>
    protected static void AssertGeoJsonContentResult(
        ContentResult contentResult,
        params string[] expectedContent
    )
    {
        contentResult.ContentType.Should().Be("application/geo+json");
        contentResult.StatusCode.Should().Be(200);
        contentResult.Content.Should().NotBeNullOrWhiteSpace();
        contentResult.Content.Should().Contain("\"type\":\"FeatureCollection\"");

        foreach (var expected in expectedContent)
        {
            contentResult.Content.Should().Contain(expected);
        }
    }

    /// <summary>
    /// Asserts that a ViewResult has the expected view name and optionally the expected model.
    /// </summary>
    /// <param name="viewResult">The ViewResult to assert.</param>
    /// <param name="expectedViewName">The expected view name.</param>
    /// <param name="expectedModel">Optional expected model to compare against.</param>
    protected static void AssertViewResult(
        ViewResult viewResult,
        string expectedViewName,
        object? expectedModel = null
    )
    {
        viewResult.ViewName.Should().Be(expectedViewName);
        if (expectedModel != null)
        {
            viewResult.ViewData.Model.Should().BeEquivalentTo(expectedModel);
        }
    }
}
