// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;
using Viae.Domain.Tests.Helpers;

namespace Viae.Domain.Tests.Models;

[TestClass]
public class VestigiumTests
{
    #region Construction Tests

    [TestMethod]
    public void Vestigium_ShouldBeCreated_WithRequiredProperties()
    {
        // Arrange
        var personaId = Guid.NewGuid().ToString();
        var locusId = Guid.NewGuid().ToString();
        var happenedAt = new DateTime(2025, 6, 15, 14, 30, 0, DateTimeKind.Utc);

        // Act
        var vestigium = TestDataBuilders
            .AVestigium()
            .WithPersonaId(personaId)
            .WithLocusId(locusId)
            .WithHappenedAt(happenedAt)
            .Build();

        // Assert
        vestigium.Id.Should().NotBeNullOrEmpty();
        vestigium.PersonaId.Should().Be(personaId);
        vestigium.LocusId.Should().Be(locusId);
        vestigium.HappenedAt.Should().Be(happenedAt);
        vestigium.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    #endregion

    #region Status Tests

    [TestMethod]
    public void Vestigium_Status_ShouldBeLatens_ByDefault()
    {
        // Arrange & Act
        var vestigium = TestDataBuilders.AVestigium().Build();
        vestigium.PublishedAt = null;
        vestigium.TombstonedAt = null;

        // Assert
        vestigium.Status.Should().Be(Status.Latens);
    }

    [TestMethod]
    public void Vestigium_Status_ShouldBeManifestus_WhenPublished()
    {
        // Arrange & Act
        var vestigium = TestDataBuilders.AVestigium().Published().Build();

        // Assert
        vestigium.Status.Should().Be(Status.Manifestus);
        vestigium.PublishedAt.Should().NotBeNull();
    }

    [TestMethod]
    public void Vestigium_Status_ShouldBeDeletus_WhenTombstoned()
    {
        // Arrange & Act
        var vestigium = TestDataBuilders.AVestigium().Tombstoned().Build();

        // Assert
        vestigium.Status.Should().Be(Status.Deletus);
        vestigium.TombstonedAt.Should().NotBeNull();
    }

    #endregion

    #region Origo Tests

    [TestMethod]
    public void Vestigium_Origo_ShouldBeDomesticusByDefault()
    {
        // Arrange & Act
        var vestigium = TestDataBuilders.AVestigium().Build();

        // Assert
        vestigium.Origo.Should().Be(Origo.Domesticus);
    }

    #endregion

    #region Content Tests

    [TestMethod]
    public void Vestigium_Content_CanBeNull()
    {
        // Arrange & Act
        var vestigium = TestDataBuilders.AVestigium().Build();

        // Assert
        vestigium.Content.Should().BeNull();
    }

    [TestMethod]
    public void Vestigium_Content_CanBeSet()
    {
        // Arrange
        var content = "Had a great time at this location!";

        // Act
        var vestigium = TestDataBuilders.AVestigium().WithContent(content).Build();

        // Assert
        vestigium.Content.Should().Be(content);
    }

    #endregion

    #region Timestamp Tests

    [TestMethod]
    public void Vestigium_HappenedAt_ShouldBeSettable()
    {
        // Arrange
        var happenedAt = new DateTime(2025, 1, 15, 10, 30, 0, DateTimeKind.Utc);

        // Act
        var vestigium = TestDataBuilders.AVestigium().WithHappenedAt(happenedAt).Build();

        // Assert
        vestigium.HappenedAt.Should().Be(happenedAt);
    }

    [TestMethod]
    public void Vestigium_UpdatedAt_CanBeSet()
    {
        // Arrange
        var updatedAt = DateTime.UtcNow.AddMinutes(-5);

        // Act
        var vestigium = TestDataBuilders.AVestigium().Build();
        vestigium.UpdatedAt = updatedAt;

        // Assert
        vestigium.UpdatedAt.Should().Be(updatedAt);
    }

    #endregion

    #region Navigation Properties Tests

    [TestMethod]
    public void Vestigium_Persona_NavigationProperty_CanBeSet()
    {
        // Arrange
        var persona = TestDataBuilders.APersona().Build();
        var vestigium = TestDataBuilders.AVestigium().WithPersonaId(persona.Id).Build();

        // Act
        vestigium.Persona = persona;

        // Assert
        vestigium.Persona.Should().Be(persona);
        vestigium.PersonaId.Should().Be(persona.Id);
    }

    [TestMethod]
    public void Vestigium_Locus_NavigationProperty_CanBeSet()
    {
        // Arrange
        var locus = TestDataBuilders.ALocus().Build();
        var vestigium = TestDataBuilders.AVestigium().WithLocusId(locus.Id).Build();

        // Act
        vestigium.Locus = locus;

        // Assert
        vestigium.Locus.Should().Be(locus);
        vestigium.LocusId.Should().Be(locus.Id);
    }

    #endregion

    #region Business Logic Tests

    [TestMethod]
    public void Vestigium_CanBePublished_AfterCreation()
    {
        // Arrange
        var vestigium = TestDataBuilders.AVestigium().Build();
        vestigium.PublishedAt = null;
        vestigium.Status.Should().Be(Status.Latens);

        // Act - Simulate publishing
        vestigium.PublishedAt = DateTime.UtcNow;

        // Assert
        vestigium.Status.Should().Be(Status.Manifestus);
    }

    [TestMethod]
    public void Vestigium_CanBeTombstoned_AfterPublishing()
    {
        // Arrange
        var vestigium = TestDataBuilders.AVestigium().Published().Build();
        vestigium.Status.Should().Be(Status.Manifestus);

        // Act - Simulate tombstoning
        vestigium.TombstonedAt = DateTime.UtcNow;

        // Assert
        vestigium.Status.Should().Be(Status.Deletus);
    }

    #endregion
}
