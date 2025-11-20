// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Tests.Helpers;

namespace Viae.Domain.Tests.Models;

[TestClass]
public class ThemaTests
{
    #region Construction Tests

    [TestMethod]
    public void Thema_ShouldBeCreated_WithRequiredProperties()
    {
        // Arrange & Act
        var thema = TestDataBuilders.AThema().WithName("Adventure").WithSlug("adventure").Build();

        // Assert
        thema.Id.Should().NotBeNullOrEmpty();
        thema.Name.Should().Be("Adventure");
        thema.Slug.Should().Be("adventure");
        thema.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void Thema_ShouldInitializeCollections()
    {
        // Arrange & Act
        var thema = TestDataBuilders.AThema().Build();

        // Assert
        thema.Folios.Should().NotBeNull();
        thema.Loca.Should().NotBeNull();
    }

    #endregion

    #region Navigation Properties Tests

    [TestMethod]
    public void Thema_Folios_CanContainMultiplePosts()
    {
        // Arrange
        var thema = TestDataBuilders.AThema().Build();
        var folium1 = TestDataBuilders.AFolium().Build();
        var folium2 = TestDataBuilders.AFolium().Build();

        // Act
        thema.Folios.Add(folium1);
        thema.Folios.Add(folium2);

        // Assert
        thema.Folios.Should().HaveCount(2);
        thema.Folios.Should().Contain(folium1);
        thema.Folios.Should().Contain(folium2);
    }

    [TestMethod]
    public void Thema_Loca_CanContainMultipleLocations()
    {
        // Arrange
        var thema = TestDataBuilders.AThema().Build();
        var locus1 = TestDataBuilders.ALocus().Build();
        var locus2 = TestDataBuilders.ALocus().Build();

        // Act
        thema.Loca.Add(locus1);
        thema.Loca.Add(locus2);

        // Assert
        thema.Loca.Should().HaveCount(2);
        thema.Loca.Should().Contain(locus1);
        thema.Loca.Should().Contain(locus2);
    }

    #endregion

    #region Slug Tests

    [TestMethod]
    public void Thema_Slug_ShouldBeUrlFriendly()
    {
        // Arrange & Act
        var thema = TestDataBuilders
            .AThema()
            .WithName("Urban Exploration")
            .WithSlug("urban-exploration")
            .Build();

        // Assert
        thema.Slug.Should().Be("urban-exploration");
        thema.Slug.Should().NotContain(" ");
    }

    #endregion
}
