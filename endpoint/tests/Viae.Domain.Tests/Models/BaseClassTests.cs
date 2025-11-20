// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;
using Viae.Domain.Tests.Helpers;

namespace Viae.Domain.Tests.Models;

[TestClass]
public class BaseClassTests
{
    #region PublishableBase Tests

    [TestMethod]
    public void PublishableBase_Status_ShouldBeLatens_WhenPublishedAtIsNull()
    {
        // Arrange
        var vestigium = TestDataBuilders.AVestigium().Build();

        // Ensure PublishedAt is null
        vestigium.PublishedAt = null;

        // Act
        var status = vestigium.Status;

        // Assert
        status.Should().Be(Status.Latens);
    }

    [TestMethod]
    public void PublishableBase_Status_ShouldBeManifestus_WhenPublishedAtIsSet()
    {
        // Arrange
        var vestigium = TestDataBuilders.AVestigium().Published().Build();

        // Act
        var status = vestigium.Status;

        // Assert
        status.Should().Be(Status.Manifestus);
        vestigium.PublishedAt.Should().NotBeNull();
    }

    #endregion

    #region TombstonableBase Tests

    [TestMethod]
    public void TombstonableBase_Status_ShouldBeLatens_WhenNotPublished()
    {
        // Arrange
        var vestigium = TestDataBuilders.AVestigium().Build();

        vestigium.PublishedAt = null;
        vestigium.TombstonedAt = null;

        // Act
        var status = vestigium.Status;

        // Assert
        status.Should().Be(Status.Latens);
    }

    [TestMethod]
    public void TombstonableBase_Status_ShouldBeManifestus_WhenPublishedButNotTombstoned()
    {
        // Arrange
        var vestigium = TestDataBuilders.AVestigium().Published().Build();

        vestigium.TombstonedAt = null;

        // Act
        var status = vestigium.Status;

        // Assert
        status.Should().Be(Status.Manifestus);
    }

    [TestMethod]
    public void TombstonableBase_Status_ShouldBeDeletus_WhenTombstoned()
    {
        // Arrange
        var vestigium = TestDataBuilders.AVestigium().Published().Tombstoned().Build();

        // Act
        var status = vestigium.Status;

        // Assert
        status.Should().Be(Status.Deletus);
        vestigium.TombstonedAt.Should().NotBeNull();
    }

    [TestMethod]
    public void TombstonableBase_Status_ShouldBeDeletus_EvenWhenNotPublished()
    {
        // Arrange
        var vestigium = TestDataBuilders.AVestigium().Tombstoned().Build();

        vestigium.PublishedAt = null;

        // Act
        var status = vestigium.Status;

        // Assert
        status.Should().Be(Status.Deletus);
    }

    #endregion

    #region CreatableBase Tests

    [TestMethod]
    public void CreatableBase_ShouldHaveCreatedAt()
    {
        // Arrange & Act
        var locus = TestDataBuilders.ALocus().Build();

        // Assert
        locus.CreatedAt.Should().NotBe(default);
        locus.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    #endregion

    #region UpdatableBase Tests

    [TestMethod]
    public void UpdatableBase_UpdatedAt_ShouldBeNullByDefault()
    {
        // Arrange & Act
        var persona = TestDataBuilders.APersona().Build();

        // Assert
        persona.UpdatedAt.Should().BeNull();
    }

    #endregion

    #region IdentifiableBase Tests

    [TestMethod]
    public void IdentifiableBase_ShouldHaveId()
    {
        // Arrange & Act
        var thema = TestDataBuilders.AThema().Build();

        // Assert
        thema.Id.Should().NotBeNullOrEmpty();
    }

    #endregion

    #region Status Enum Tests

    [TestMethod]
    public void Status_Enum_ShouldHaveCorrectValues()
    {
        // Assert
        ((int)Status.Latens)
            .Should()
            .Be(0);
        ((int)Status.Manifestus).Should().Be(1);
        ((int)Status.Deletus).Should().Be(2);
    }

    #endregion

    #region Origo Enum Tests

    [TestMethod]
    public void Origo_Enum_ShouldHaveCorrectValues()
    {
        // Assert
        ((int)Origo.Domesticus)
            .Should()
            .Be(0);
        ((int)Origo.Externus).Should().Be(1);
    }

    #endregion

    #region Status Transition Tests

    [TestMethod]
    public void Vestigium_StatusTransition_FromLatensToManifestus()
    {
        // Arrange
        var vestigium = TestDataBuilders.AVestigium().Build();
        vestigium.PublishedAt = null;

        // Act - Simulate publishing
        vestigium.Status.Should().Be(Status.Latens);
        vestigium.PublishedAt = DateTime.UtcNow;

        // Assert
        vestigium.Status.Should().Be(Status.Manifestus);
    }

    [TestMethod]
    public void Vestigium_StatusTransition_FromManifestusToDeletus()
    {
        // Arrange
        var vestigium = TestDataBuilders.AVestigium().Published().Build();

        // Act - Simulate tombstoning
        vestigium.Status.Should().Be(Status.Manifestus);
        vestigium.TombstonedAt = DateTime.UtcNow;

        // Assert
        vestigium.Status.Should().Be(Status.Deletus);
    }

    [TestMethod]
    public void Vestigium_StatusTransition_DirectlyToDeletus()
    {
        // Arrange
        var vestigium = TestDataBuilders.AVestigium().Build();
        vestigium.PublishedAt = null;
        vestigium.Status.Should().Be(Status.Latens);

        // Act - Tombstone without publishing
        vestigium.TombstonedAt = DateTime.UtcNow;

        // Assert
        vestigium.Status.Should().Be(Status.Deletus);
    }

    #endregion
}
