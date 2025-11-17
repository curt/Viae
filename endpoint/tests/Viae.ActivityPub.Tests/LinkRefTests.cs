// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using FluentAssertions;

namespace Viae.ActivityPub.Tests;

[TestClass]
public class LinkRefTests
{
    [TestMethod]
    public void Constructor_ShouldCreateLinkRef_FromUri()
    {
        // Arrange
        var uri = new Uri("https://example.com/users/alice");

        // Act
        var linkRef = new LinkRef(uri);

        // Assert
        linkRef.Href.Should().Be(uri);
    }

    [TestMethod]
    public void ImplicitConversion_FromUri_ShouldWork()
    {
        // Arrange
        var uri = new Uri("https://example.com/users/bob");

        // Act
        LinkRef linkRef = uri;

        // Assert
        linkRef.Href.Should().Be(uri);
    }

    [TestMethod]
    public void ImplicitConversion_FromString_ShouldWork()
    {
        // Arrange
        var uriString = "https://example.com/users/charlie";

        // Act
        LinkRef linkRef = uriString;

        // Assert
        linkRef.Href.Should().Be(new Uri(uriString));
    }

    [TestMethod]
    public void ImplicitConversion_ToUri_ShouldWork()
    {
        // Arrange
        var uri = new Uri("https://example.com/users/dave");
        var linkRef = new LinkRef(uri);

        // Act
        Uri result = linkRef;

        // Assert
        result.Should().Be(uri);
    }

    [TestMethod]
    public void Equality_ShouldWork_ForSameUri()
    {
        // Arrange
        var uri = new Uri("https://example.com/test");
        var linkRef1 = new LinkRef(uri);
        var linkRef2 = new LinkRef(uri);

        // Act & Assert
        linkRef1.Should().Be(linkRef2);
        (linkRef1 == linkRef2).Should().BeTrue();
    }

    [TestMethod]
    public void Equality_ShouldFail_ForDifferentUris()
    {
        // Arrange
        var linkRef1 = new LinkRef(new Uri("https://example.com/a"));
        var linkRef2 = new LinkRef(new Uri("https://example.com/b"));

        // Act & Assert
        linkRef1.Should().NotBe(linkRef2);
        (linkRef1 == linkRef2).Should().BeFalse();
    }

    [TestMethod]
    public void GetHashCode_ShouldBeConsistent()
    {
        // Arrange
        var uri = new Uri("https://example.com/test");
        var linkRef1 = new LinkRef(uri);
        var linkRef2 = new LinkRef(uri);

        // Act
        var hash1 = linkRef1.GetHashCode();
        var hash2 = linkRef2.GetHashCode();

        // Assert
        hash1.Should().Be(hash2);
    }

    [TestMethod]
    public void ToString_ShouldReturnUriString()
    {
        // Arrange
        var uriString = "https://example.com/users/eve";
        var linkRef = new LinkRef(new Uri(uriString));

        // Act
        var result = linkRef.ToString();

        // Assert
        result.Should().Be(uriString);
    }
}
