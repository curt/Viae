// Copyright (c) Curt Gilman. Licensed under the AGPL-3.0 License. See LICENSE file in the project root for full license information.

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Viae.Domain.Models;
using Viae.Presentation.Mvc.Tests.Builders;

namespace Viae.Presentation.Mvc.Tests.Controllers;

[TestClass]
public class LocaControllerTests : LocaControllerTestBase
{
    #region Index Action Tests

    [TestMethod]
    public async Task Index_WithHtmlAcceptHeader_ReturnsViewResult()
    {
        // Arrange
        var loca = new List<Locus> { LocusBuilder.Default().Build() };

        SetupGetLocaAsync(loca);
        SetAcceptHeader("text/html");

        // Act
        var result = await Controller.Index(CancellationToken.None);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result.As<ViewResult>();
        AssertViewResult(viewResult, "Index");
    }

    [TestMethod]
    public async Task Index_WithGeoJsonAcceptHeader_ReturnsGeoJsonContentResult()
    {
        // Arrange
        var loca = new List<Locus>
        {
            LocusBuilder
                .Default()
                .WithId("1")
                .WithName("Location 1")
                .WithCoordinates(37.7749, -122.4194)
                .Build(),
            LocusBuilder
                .Default()
                .WithId("2")
                .WithName("Location 2")
                .WithCoordinates(40.7128, -74.0060)
                .Build(),
        };

        SetupGetLocaAsync(loca);
        SetAcceptHeader("application/geo+json");

        // Act
        var result = await Controller.Index(CancellationToken.None);

        // Assert
        result.Should().BeOfType<ContentResult>();
        var contentResult = result.As<ContentResult>();
        AssertGeoJsonContentResult(contentResult, "Location 1", "Location 2");
    }

    [TestMethod]
    public async Task Index_WithActivityPubAcceptHeader_ReturnsActivityPubResult()
    {
        // Arrange
        var loca = new List<Locus> { LocusBuilder.Default().Build() };

        SetupGetLocaAsync(loca);
        SetAcceptHeader("application/activity+json");

        // Act
        var result = await Controller.Index(CancellationToken.None);

        // Assert
        // Index does not have ActivityPub handler, falls back to GeoJSON (first registered handler)
        result.Should().BeOfType<ContentResult>();
        var contentResult = result.As<ContentResult>();
        contentResult.ContentType.Should().Be("application/activity+json");
    }

    #endregion

    #region Show Action - Success Cases

    [TestMethod]
    public async Task Show_WithNonExistentId_ReturnsNotFound()
    {
        // Arrange
        SetupGetLocusByIdAsync("nonexistent", null);
        SetAcceptHeader("text/html");

        // Act
        var result = await Controller.Show("nonexistent", null, CancellationToken.None);

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [TestMethod]
    public async Task Show_WithHtmlAcceptHeader_ReturnsViewResult()
    {
        // Arrange
        var locus = LocusBuilder.Default().WithId("test123").WithoutSlug().Build();

        SetupGetLocusByIdAsync("test123", locus);
        SetupNearbyLocaMocks();
        SetAcceptHeader("text/html");
        SetRequestPath("/loca/test123");

        // Act
        var result = await Controller.Show("test123", null, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ViewResult>();
        var viewResult = result.As<ViewResult>();
        AssertViewResult(viewResult, "Show");
    }

    [TestMethod]
    public async Task Show_WithGeoJsonAcceptHeader_ReturnsSingleFeature()
    {
        // Arrange
        var locus = LocusBuilder
            .Default()
            .WithId("test123")
            .WithName("Test Location")
            .WithCoordinates(37.7749, -122.4194)
            .WithoutSlug()
            .Build();

        // GeoJSON handler returns nearby loca which should include the main locus
        var nearbyLoca = new List<LocusDistance>
        {
            new()
            {
                Locus = locus,
                ReferenceLocus = locus,
                Distance = 0.0,
            },
        };

        SetupGetLocusByIdAsync("test123", locus);
        SetupNearbyLocaMocks(nearbyLoca);
        SetAcceptHeader("application/geo+json");
        SetRequestPath("/loca/test123");

        // Act
        var result = await Controller.Show("test123", null, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ContentResult>();
        var contentResult = result.As<ContentResult>();
        AssertGeoJsonContentResult(contentResult, "test123", "Test Location");
    }

    #endregion

    #region Show Action - ActivityPub Tests

    [TestMethod]
    public async Task Show_WithActivityPubAcceptHeader_ReturnsActivityPubResult()
    {
        // Arrange
        var locus = LocusBuilder
            .Default()
            .WithId("test123")
            .WithName("Test Location")
            .WithSlug("test-location")
            .WithContent("Test content for location")
            .WithCoordinates(37.7749, -122.4194)
            .Build();

        SetupGetLocusByIdAsync("test123", locus);
        SetupNearbyLocaMocks();
        SetAcceptHeader("application/activity+json");
        SetRequestPath("/loca/test123/test-location");
        SetSchemeHost("http", "example.com");

        // Act
        var result = await Controller.Show("test123", "test-location", CancellationToken.None);

        // Assert
        result.Should().BeOfType<ContentResult>();
        var contentResult = result.As<ContentResult>();
        contentResult.ContentType.Should().Be("application/activity+json");
        contentResult.Content.Should().NotBeNullOrWhiteSpace();
    }

    [TestMethod]
    public async Task Show_WithActivityPubAcceptHeader_ReturnsPlaceType()
    {
        // Arrange
        var locus = LocusBuilder
            .Default()
            .WithId("test123")
            .WithName("Test Location")
            .WithSlug("test-location")
            .WithContent("Test content for location")
            .WithCoordinates(37.7749, -122.4194)
            .Build();

        SetupGetLocusByIdAsync("test123", locus);
        SetupNearbyLocaMocks();
        SetAcceptHeader("application/activity+json");
        SetRequestPath("/loca/test123/test-location");
        SetSchemeHost("http", "example.com");

        // Act
        var result = await Controller.Show("test123", "test-location", CancellationToken.None);

        // Assert
        result.Should().BeOfType<ContentResult>();
        var contentResult = result.As<ContentResult>();

        // Verify the JSON contains the type discriminator
        contentResult.Content.Should().Contain("\"type\"");
        contentResult.Content.Should().Contain("\"Place\"");
    }

    [TestMethod]
    public async Task Show_WithJsonLdAcceptHeader_ReturnsActivityPubResult()
    {
        // Arrange
        var locus = LocusBuilder
            .Default()
            .WithId("test123")
            .WithName("Test Location")
            .WithCoordinates(37.7749, -122.4194)
            .WithoutSlug()
            .Build();

        SetupGetLocusByIdAsync("test123", locus);
        SetupNearbyLocaMocks();
        SetAcceptHeader("application/ld+json");
        SetRequestPath("/loca/test123");
        SetSchemeHost("http", "example.com");

        // Act
        var result = await Controller.Show("test123", null, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ContentResult>();
        var contentResult = result.As<ContentResult>();
        contentResult
            .ContentType.Should()
            .Be("application/ld+json; profile=\"https://www.w3.org/ns/activitystreams\"");
    }

    [TestMethod]
    public async Task Show_WithActivityPubAcceptHeader_ContainsLocationName()
    {
        // Arrange
        var locus = LocusBuilder
            .Default()
            .WithId("test123")
            .WithName("Golden Gate Park")
            .WithSlug("golden-gate-park")
            .WithContent("Beautiful park in San Francisco")
            .WithCoordinates(37.7694, -122.4862)
            .Build();

        SetupGetLocusByIdAsync("test123", locus);
        SetupNearbyLocaMocks();
        SetAcceptHeader("application/activity+json");
        SetRequestPath("/loca/test123/golden-gate-park");
        SetSchemeHost("http", "example.com");

        // Act
        var result = await Controller.Show("test123", "golden-gate-park", CancellationToken.None);

        // Assert
        result.Should().BeOfType<ContentResult>();
        var contentResult = result.As<ContentResult>();

        // Verify the JSON contains the location name
        contentResult.Content.Should().Contain("\"name\"");
        contentResult.Content.Should().Contain("Golden Gate Park");
    }

    #endregion

    #region Show Action - Canonical URL Tests

    [TestMethod]
    public async Task Show_WithWrongSlug_RedirectsToCanonical()
    {
        // Arrange
        var locus = LocusBuilder.Default().WithId("test123").WithSlug("correct-slug").Build();

        SetupGetLocusByIdAsync("test123", locus);
        SetRequestPath("/loca/test123/wrong-slug");
        SetAcceptHeader("text/html");

        // Act
        var result = await Controller.Show("test123", "wrong-slug", CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result.As<RedirectResult>();
        redirectResult.Url.Should().Be("/loca/test123/correct-slug");
        redirectResult.Permanent.Should().BeTrue();
    }

    [TestMethod]
    public async Task Show_WithMissingSlugWhenSlugExists_RedirectsToCanonical()
    {
        // Arrange
        var locus = LocusBuilder.Default().WithId("test123").WithSlug("expected-slug").Build();

        SetupGetLocusByIdAsync("test123", locus);
        SetRequestPath("/loca/test123");
        SetAcceptHeader("text/html");

        // Act
        var result = await Controller.Show("test123", null, CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result.As<RedirectResult>();
        redirectResult.Url.Should().Be("/loca/test123/expected-slug");
        redirectResult.Permanent.Should().BeTrue();
    }

    [TestMethod]
    public async Task Show_WithSlugWhenNoneExists_RedirectsToIdOnly()
    {
        // Arrange
        var locus = LocusBuilder.Default().WithId("test123").WithoutSlug().Build();

        SetupGetLocusByIdAsync("test123", locus);
        SetRequestPath("/loca/test123/some-slug");
        SetAcceptHeader("text/html");

        // Act
        var result = await Controller.Show("test123", "some-slug", CancellationToken.None);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result.As<RedirectResult>();
        redirectResult.Url.Should().Be("/loca/test123");
        redirectResult.Permanent.Should().BeTrue();
    }

    #endregion

    #region Show Action - GeoJSON Feature Tests

    [TestMethod]
    public async Task Show_GeoJsonFeature_ContainsAllAttributes()
    {
        // Arrange
        var locus = LocusBuilder
            .Default()
            .WithId("test123")
            .WithName("Test Location")
            .WithSlug("test-location")
            .WithContent("Test content")
            .WithAddress("123 Test St")
            .WithCity("Test City")
            .WithCountry("Test Country")
            .WithCoordinates(37.7749, -122.4194)
            .Build();

        // GeoJSON handler returns nearby loca which should include the main locus
        var nearbyLoca = new List<LocusDistance>
        {
            new()
            {
                Locus = locus,
                ReferenceLocus = locus,
                Distance = 0.0,
            },
        };

        SetupGetLocusByIdAsync("test123", locus);
        SetupNearbyLocaMocks(nearbyLoca);
        SetAcceptHeader("application/geo+json");
        SetRequestPath("/loca/test123/test-location");

        // Act
        var result = await Controller.Show("test123", "test-location", CancellationToken.None);

        // Assert
        result.Should().BeOfType<ContentResult>();
        var contentResult = result.As<ContentResult>();
        // GeoJSON properties only include: id, name, slug, path, and center flag
        AssertGeoJsonContentResult(
            contentResult,
            "test123",
            "Test Location",
            "test-location",
            "/loca/test123/test-location",
            "\"center\":true"
        );
    }

    [TestMethod]
    public async Task Show_GeoJsonFeature_HasCorrectGeometry()
    {
        // Arrange
        var locus = LocusBuilder
            .Default()
            .WithId("test123")
            .WithCoordinates(37.7749, -122.4194)
            .WithoutSlug()
            .Build();

        // GeoJSON handler returns nearby loca which should include the main locus
        var nearbyLoca = new List<LocusDistance>
        {
            new()
            {
                Locus = locus,
                ReferenceLocus = locus,
                Distance = 0.0,
            },
        };

        SetupGetLocusByIdAsync("test123", locus);
        SetupNearbyLocaMocks(nearbyLoca);
        SetAcceptHeader("application/geo+json");
        SetRequestPath("/loca/test123");

        // Act
        var result = await Controller.Show("test123", null, CancellationToken.None);

        // Assert
        result.Should().BeOfType<ContentResult>();
        var contentResult = result.As<ContentResult>();
        AssertGeoJsonContentResult(contentResult, "-122.4194", "37.7749", "\"type\":\"Point\"");
    }

    #endregion
}
