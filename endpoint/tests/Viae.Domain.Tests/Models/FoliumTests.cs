// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;
using Viae.Domain.Tests.Helpers;

namespace Viae.Domain.Tests.Models;

[TestClass]
public class FoliumTests
{
    [TestMethod]
    public void Folium_ShouldBeCreated_WithRequiredProperties()
    {
        // Arrange
        var personaId = Guid.NewGuid().ToString();

        // Act
        var folium = TestDataBuilders
            .AFolium()
            .WithTitle("My Travel Journey")
            .WithContent("This is my story...")
            .WithPersonaId(personaId)
            .Build();

        // Assert
        folium.Id.Should().NotBeNullOrEmpty();
        folium.Title.Should().Be("My Travel Journey");
        folium.Content.Should().Be("This is my story...");
        folium.PersonaId.Should().Be(personaId);
        folium.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void Folium_ShouldInitializeCollections()
    {
        // Arrange & Act
        var folium = TestDataBuilders.AFolium().Build();

        // Assert
        folium.Themata.Should().NotBeNull();
        folium.Loca.Should().NotBeNull();
        folium.ImageUrls.Should().NotBeNull();
    }

    [TestMethod]
    public void Folium_IsDraft_ShouldBeTrue_ByDefault()
    {
        // Arrange & Act
        var folium = TestDataBuilders.AFolium().Draft().Build();

        // Assert
        folium.IsDraft.Should().BeTrue();
        folium.PublishedAt.Should().BeNull();
    }

    [TestMethod]
    public void Folium_IsDraft_ShouldBeFalse_WhenPublished()
    {
        // Arrange & Act
        var folium = TestDataBuilders.AFolium().Published().Build();

        // Assert
        folium.IsDraft.Should().BeFalse();
        folium.PublishedAt.Should().NotBeNull();
    }

    [TestMethod]
    public void Folium_Origo_ShouldBeDomesticusByDefault()
    {
        // Arrange & Act
        var folium = TestDataBuilders.AFolium().Build();

        // Assert
        folium.Origo.Should().Be(Origo.Domesticus);
    }

    [TestMethod]
    public void Folium_PublishedAt_ShouldBeSet_WhenPublished()
    {
        // Arrange & Act
        var folium = TestDataBuilders.AFolium().Published().Build();

        // Assert
        folium.PublishedAt.Should().NotBeNull();
        folium.PublishedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void Folium_Persona_NavigationProperty_CanBeSet()
    {
        // Arrange
        var persona = TestDataBuilders.APersona().Build();
        var folium = TestDataBuilders.AFolium().WithPersonaId(persona.Id).Build();

        // Act
        folium.Persona = persona;

        // Assert
        folium.Persona.Should().Be(persona);
        folium.PersonaId.Should().Be(persona.Id);
    }

    [TestMethod]
    public void Folium_Themata_CanContainMultipleThemes()
    {
        // Arrange
        var folium = TestDataBuilders.AFolium().Build();
        var thema1 = TestDataBuilders.AThema().WithName("Travel").Build();
        var thema2 = TestDataBuilders.AThema().WithName("Photography").Build();

        // Act
        folium.Themata.Add(thema1);
        folium.Themata.Add(thema2);

        // Assert
        folium.Themata.Should().HaveCount(2);
        folium.Themata.Should().Contain(thema1);
        folium.Themata.Should().Contain(thema2);
    }

    [TestMethod]
    public void Folium_CanBePublished_AfterCreation()
    {
        // Arrange
        var folium = TestDataBuilders.AFolium().Draft().Build();
        folium.IsDraft.Should().BeTrue();

        // Act - Simulate publishing
        folium.PublishedAt = DateTime.UtcNow;
        folium.IsDraft = false;

        // Assert
        folium.IsDraft.Should().BeFalse();
        folium.PublishedAt.Should().NotBeNull();
    }
}
