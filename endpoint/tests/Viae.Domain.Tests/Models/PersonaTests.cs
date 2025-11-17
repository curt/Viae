// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;
using Viae.Domain.Tests.Helpers;

namespace Viae.Domain.Tests.Models;

[TestClass]
public class PersonaTests
{
    #region Construction Tests

    [TestMethod]
    public void Persona_ShouldBeCreated_WithRequiredProperties()
    {
        // Arrange & Act
        var persona = TestDataBuilders
            .APersona()
            .WithUsername("johndoe")
            .WithDisplayName("John Doe")
            .WithEmail("john@example.com")
            .Build();

        // Assert
        persona.Id.Should().NotBeNullOrEmpty();
        persona.Username.Should().Be("johndoe");
        persona.DisplayName.Should().Be("John Doe");
        persona.Email.Should().Be("john@example.com");
        persona.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [TestMethod]
    public void Persona_ShouldInitializeCollections()
    {
        // Arrange & Act
        var persona = TestDataBuilders.APersona().Build();

        // Assert
        persona.Folios.Should().NotBeNull();
    }

    #endregion

    #region Origo Tests

    [TestMethod]
    public void Persona_Origo_ShouldBeDomesticusByDefault()
    {
        // Arrange & Act
        var persona = TestDataBuilders.APersona().Build();

        // Assert
        persona.Origo.Should().Be(Origo.Domesticus);
    }

    [TestMethod]
    public void Persona_Origo_CanBeSetToExternus()
    {
        // Arrange & Act
        var persona = TestDataBuilders.APersona().WithOrigo(Origo.Externus).Build();

        // Assert
        persona.Origo.Should().Be(Origo.Externus);
    }

    #endregion

    #region Optional Fields Tests

    [TestMethod]
    public void Persona_Bio_CanBeNull()
    {
        // Arrange & Act
        var persona = TestDataBuilders.APersona().Build();

        // Assert
        persona.Bio.Should().BeNull();
    }

    [TestMethod]
    public void Persona_Bio_CanBeSet()
    {
        // Arrange
        var bio = "Travel enthusiast and blogger";

        // Act
        var persona = TestDataBuilders.APersona().WithBio(bio).Build();

        // Assert
        persona.Bio.Should().Be(bio);
    }

    [TestMethod]
    public void Persona_AvatarUrl_CanBeNull()
    {
        // Arrange & Act
        var persona = TestDataBuilders.APersona().Build();

        // Assert
        persona.AvatarUrl.Should().BeNull();
    }

    #endregion

    #region ActivityPub Properties Tests

    [TestMethod]
    public void Persona_Uri_CanBeSet()
    {
        // Arrange
        var uri = new Uri("https://example.com/users/johndoe");

        // Act
        var persona = TestDataBuilders.APersona().WithUri(uri).Build();

        // Assert
        persona.Uri.Should().Be(uri);
    }

    [TestMethod]
    public void Persona_Keys_CanBeSet()
    {
        // Arrange
        var publicKey = "-----BEGIN PUBLIC KEY-----\ntest\n-----END PUBLIC KEY-----";
        var privateKey = "-----BEGIN PRIVATE KEY-----\ntest\n-----END PRIVATE KEY-----";

        // Act
        var persona = TestDataBuilders.APersona().WithKeys(publicKey, privateKey).Build();

        // Assert
        persona.PublicKey.Should().Be(publicKey);
        persona.PrivateKey.Should().Be(privateKey);
    }

    [TestMethod]
    public void Persona_Keys_ShouldBeNull_ForExternalOrigin()
    {
        // Arrange & Act
        var persona = TestDataBuilders.APersona().WithOrigo(Origo.Externus).Build();

        // Assert
        persona.PrivateKey.Should().BeNull();
    }

    #endregion

    #region Timestamp Tests

    [TestMethod]
    public void Persona_UpdatedAt_CanBeSet()
    {
        // Arrange
        var updatedAt = DateTime.UtcNow.AddDays(-1);

        // Act
        var persona = TestDataBuilders.APersona().Build();
        persona.UpdatedAt = updatedAt;

        // Assert
        persona.UpdatedAt.Should().Be(updatedAt);
    }

    #endregion
}
