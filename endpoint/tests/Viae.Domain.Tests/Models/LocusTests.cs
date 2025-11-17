// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;
using Viae.Domain.Tests.Helpers;

namespace Viae.Domain.Tests.Models;

[TestClass]
public class LocusTests
{
    [TestMethod]
    public void Locus_ShouldBeCreated_WithRequiredProperties()
    {
        // Arrange & Act
        var locus = TestDataBuilders
            .ALocus()
            .WithName("Golden Gate Bridge")
            .WithSlug("golden-gate-bridge")
            .WithCoordinates(37.8199, -122.4783)
            .Build();

        // Assert
        locus.Id.Should().NotBeNullOrEmpty();
        locus.Name.Should().Be("Golden Gate Bridge");
        locus.Slug.Should().Be("golden-gate-bridge");
        locus.Coordinates.Should().NotBeNull();
        locus.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void Locus_ShouldInitializeCollections()
    {
        // Arrange & Act
        var locus = TestDataBuilders.ALocus().Build();

        // Assert
        locus.Folios.Should().NotBeNull();
        locus.Themata.Should().NotBeNull();
        locus.Vestigia.Should().NotBeNull();
    }

    [TestMethod]
    public void Locus_Latitude_ShouldMatchCoordinatesY()
    {
        // Arrange
        var locus = TestDataBuilders.ALocus().WithCoordinates(40.7128, -74.0060).Build();

        // Act & Assert
        locus.Latitude.Should().Be(40.7128);
        locus.Coordinates.Y.Should().Be(40.7128);
    }

    [TestMethod]
    public void Locus_Longitude_ShouldMatchCoordinatesX()
    {
        // Arrange
        var locus = TestDataBuilders.ALocus().WithCoordinates(40.7128, -74.0060).Build();

        // Act & Assert
        locus.Longitude.Should().Be(-74.0060);
        locus.Coordinates.X.Should().Be(-74.0060);
    }

    [TestMethod]
    public void Locus_Coordinates_ShouldHaveCorrectSRID()
    {
        // Arrange & Act
        var locus = TestDataBuilders.ALocus().Build();

        // Assert
        locus.Coordinates.SRID.Should().Be(4326);
    }

    [TestMethod]
    public void Locus_OsmId_CanBeNull()
    {
        // Arrange & Act
        var locus = TestDataBuilders.ALocus().Build();

        // Assert
        locus.OsmId.Should().BeNull();
    }

    [TestMethod]
    public void Locus_OsmId_CanBeSet()
    {
        // Arrange
        var osmId = new OsmId(OsmType.Node, 123456789);

        // Act
        var locus = TestDataBuilders.ALocus().WithOsmId(osmId).Build();

        // Assert
        locus.OsmId.Should().NotBeNull();
        locus.OsmId.Should().Be(osmId);
    }

    [TestMethod]
    public void LocusDistance_ShouldStoreLocusAndDistance()
    {
        // Arrange
        var locus = TestDataBuilders.ALocus().Build();
        var referenceLocus = TestDataBuilders.ALocus().Build();
        var distance = 5000.0;

        // Act
        var locusDistance = new LocusDistance
        {
            Locus = locus,
            ReferenceLocus = referenceLocus,
            Distance = distance,
        };

        // Assert
        locusDistance.Locus.Should().Be(locus);
        locusDistance.ReferenceLocus.Should().Be(referenceLocus);
        locusDistance.Distance.Should().Be(distance);
    }

    [TestMethod]
    public void LocusDistance_IsReferenceLocus_ShouldBeTrue_WhenLocusIsSameAsReference()
    {
        // Arrange
        var locus = TestDataBuilders.ALocus().WithId("same-id").Build();

        // Act
        var locusDistance = new LocusDistance
        {
            Locus = locus,
            ReferenceLocus = locus,
            Distance = 0,
        };

        // Assert
        locusDistance.IsReferenceLocus.Should().BeTrue();
    }
}
