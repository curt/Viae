// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Domain.Tests.Models;

[TestClass]
public class OsmIdTests
{
    #region Construction Tests

    [TestMethod]
    public void Constructor_ShouldCreateOsmId_WithValidTypeAndId()
    {
        // Arrange & Act
        var osmId = new OsmId(OsmType.Node, 123456789);

        // Assert
        osmId.Type.Should().Be(OsmType.Node);
        osmId.Id.Should().Be(123456789);
    }

    [TestMethod]
    public void Constructor_ShouldCreateOsmId_WithWayType()
    {
        // Arrange & Act
        var osmId = new OsmId(OsmType.Way, 987654321);

        // Assert
        osmId.Type.Should().Be(OsmType.Way);
        osmId.Id.Should().Be(987654321);
    }

    [TestMethod]
    public void Constructor_ShouldCreateOsmId_WithRelationType()
    {
        // Arrange & Act
        var osmId = new OsmId(OsmType.Relation, 555555);

        // Assert
        osmId.Type.Should().Be(OsmType.Relation);
        osmId.Id.Should().Be(555555);
    }

    #endregion

    #region ToString Tests

    [TestMethod]
    public void ToString_ShouldReturnCorrectFormat_ForNode()
    {
        // Arrange
        var osmId = new OsmId(OsmType.Node, 123456789);

        // Act
        var result = osmId.ToString();

        // Assert
        result.Should().Be("node/123456789");
    }

    [TestMethod]
    public void ToString_ShouldReturnCorrectFormat_ForWay()
    {
        // Arrange
        var osmId = new OsmId(OsmType.Way, 987654321);

        // Act
        var result = osmId.ToString();

        // Assert
        result.Should().Be("way/987654321");
    }

    [TestMethod]
    public void ToString_ShouldReturnCorrectFormat_ForRelation()
    {
        // Arrange
        var osmId = new OsmId(OsmType.Relation, 555555);

        // Act
        var result = osmId.ToString();

        // Assert
        result.Should().Be("relation/555555");
    }

    #endregion

    #region TryParse Tests

    [TestMethod]
    public void TryParse_ShouldReturnTrue_ForValidNodeFormat()
    {
        // Arrange
        var input = "node/123456789";

        // Act
        var success = OsmId.TryParse(input, out var osmId);

        // Assert
        success.Should().BeTrue();
        osmId.Type.Should().Be(OsmType.Node);
        osmId.Id.Should().Be(123456789);
    }

    [TestMethod]
    public void TryParse_ShouldReturnTrue_ForValidWayFormat()
    {
        // Arrange
        var input = "way/987654321";

        // Act
        var success = OsmId.TryParse(input, out var osmId);

        // Assert
        success.Should().BeTrue();
        osmId.Type.Should().Be(OsmType.Way);
        osmId.Id.Should().Be(987654321);
    }

    [TestMethod]
    public void TryParse_ShouldReturnTrue_ForValidRelationFormat()
    {
        // Arrange
        var input = "relation/555555";

        // Act
        var success = OsmId.TryParse(input, out var osmId);

        // Assert
        success.Should().BeTrue();
        osmId.Type.Should().Be(OsmType.Relation);
        osmId.Id.Should().Be(555555);
    }

    [TestMethod]
    public void TryParse_ShouldReturnFalse_ForInvalidFormat_MissingSlash()
    {
        // Arrange
        var input = "node123456789";

        // Act
        var success = OsmId.TryParse(input, out var osmId);

        // Assert
        success.Should().BeFalse();
        osmId.Should().Be(default);
    }

    [TestMethod]
    public void TryParse_ShouldReturnFalse_ForInvalidFormat_TooManyParts()
    {
        // Arrange
        var input = "node/123/456";

        // Act
        var success = OsmId.TryParse(input, out var osmId);

        // Assert
        success.Should().BeFalse();
        osmId.Should().Be(default);
    }

    [TestMethod]
    public void TryParse_ShouldReturnFalse_ForInvalidFormat_InvalidType()
    {
        // Arrange
        var input = "invalid/123456789";

        // Act
        var success = OsmId.TryParse(input, out var osmId);

        // Assert
        success.Should().BeFalse();
        osmId.Should().Be(default);
    }

    [TestMethod]
    public void TryParse_ShouldReturnFalse_ForInvalidFormat_NonNumericId()
    {
        // Arrange
        var input = "node/notanumber";

        // Act
        var success = OsmId.TryParse(input, out var osmId);

        // Assert
        success.Should().BeFalse();
        osmId.Should().Be(default);
    }

    [TestMethod]
    public void TryParse_ShouldReturnFalse_ForNullInput()
    {
        // Arrange
        string? input = null;

        // Act
        var success = OsmId.TryParse(input, out var osmId);

        // Assert
        success.Should().BeFalse();
        osmId.Should().Be(default);
    }

    [TestMethod]
    public void TryParse_ShouldReturnFalse_ForEmptyString()
    {
        // Arrange
        var input = string.Empty;

        // Act
        var success = OsmId.TryParse(input, out var osmId);

        // Assert
        success.Should().BeFalse();
        osmId.Should().Be(default);
    }

    [TestMethod]
    public void TryParse_ShouldBeCaseInsensitive()
    {
        // Arrange
        var input = "NODE/123456789";

        // Act
        var success = OsmId.TryParse(input, out var osmId);

        // Assert
        success.Should().BeTrue();
        osmId.Type.Should().Be(OsmType.Node);
        osmId.Id.Should().Be(123456789);
    }

    #endregion

    #region Equality Tests

    [TestMethod]
    public void Equals_ShouldReturnTrue_ForSameTypeAndId()
    {
        // Arrange
        var osmId1 = new OsmId(OsmType.Node, 123456789);
        var osmId2 = new OsmId(OsmType.Node, 123456789);

        // Act & Assert
        osmId1.Equals(osmId2).Should().BeTrue();
        (osmId1 == osmId2).Should().BeTrue();
        (osmId1 != osmId2).Should().BeFalse();
    }

    [TestMethod]
    public void Equals_ShouldReturnFalse_ForDifferentTypes()
    {
        // Arrange
        var osmId1 = new OsmId(OsmType.Node, 123456789);
        var osmId2 = new OsmId(OsmType.Way, 123456789);

        // Act & Assert
        osmId1.Equals(osmId2).Should().BeFalse();
        (osmId1 == osmId2).Should().BeFalse();
        (osmId1 != osmId2).Should().BeTrue();
    }

    [TestMethod]
    public void Equals_ShouldReturnFalse_ForDifferentIds()
    {
        // Arrange
        var osmId1 = new OsmId(OsmType.Node, 123456789);
        var osmId2 = new OsmId(OsmType.Node, 987654321);

        // Act & Assert
        osmId1.Equals(osmId2).Should().BeFalse();
        (osmId1 == osmId2).Should().BeFalse();
        (osmId1 != osmId2).Should().BeTrue();
    }

    [TestMethod]
    public void GetHashCode_ShouldBeSame_ForEqualOsmIds()
    {
        // Arrange
        var osmId1 = new OsmId(OsmType.Node, 123456789);
        var osmId2 = new OsmId(OsmType.Node, 123456789);

        // Act & Assert
        osmId1.GetHashCode().Should().Be(osmId2.GetHashCode());
    }

    [TestMethod]
    public void GetHashCode_ShouldBeDifferent_ForDifferentOsmIds()
    {
        // Arrange
        var osmId1 = new OsmId(OsmType.Node, 123456789);
        var osmId2 = new OsmId(OsmType.Way, 123456789);

        // Act & Assert
        osmId1.GetHashCode().Should().NotBe(osmId2.GetHashCode());
    }

    #endregion

    #region Comparison Tests

    [TestMethod]
    public void CompareTo_ShouldReturnNegative_WhenTypeIsLess()
    {
        // Arrange
        var osmId1 = new OsmId(OsmType.Node, 999999);
        var osmId2 = new OsmId(OsmType.Way, 100000);

        // Act
        var result = osmId1.CompareTo(osmId2);

        // Assert
        result.Should().BeNegative();
        (osmId1 < osmId2).Should().BeTrue();
        (osmId1 <= osmId2).Should().BeTrue();
    }

    [TestMethod]
    public void CompareTo_ShouldReturnPositive_WhenTypeIsGreater()
    {
        // Arrange
        var osmId1 = new OsmId(OsmType.Way, 100000);
        var osmId2 = new OsmId(OsmType.Node, 999999);

        // Act
        var result = osmId1.CompareTo(osmId2);

        // Assert
        result.Should().BePositive();
        (osmId1 > osmId2).Should().BeTrue();
        (osmId1 >= osmId2).Should().BeTrue();
    }

    [TestMethod]
    public void CompareTo_ShouldCompareById_WhenTypesAreSame()
    {
        // Arrange
        var osmId1 = new OsmId(OsmType.Node, 100000);
        var osmId2 = new OsmId(OsmType.Node, 200000);

        // Act
        var result = osmId1.CompareTo(osmId2);

        // Assert
        result.Should().BeNegative();
        (osmId1 < osmId2).Should().BeTrue();
    }

    [TestMethod]
    public void CompareTo_ShouldReturnZero_ForEqualOsmIds()
    {
        // Arrange
        var osmId1 = new OsmId(OsmType.Node, 123456789);
        var osmId2 = new OsmId(OsmType.Node, 123456789);

        // Act
        var result = osmId1.CompareTo(osmId2);

        // Assert
        result.Should().Be(0);
        (osmId1 <= osmId2).Should().BeTrue();
        (osmId1 >= osmId2).Should().BeTrue();
    }

    [TestMethod]
    public void ComparisonOperators_ShouldWorkCorrectly_InSortingScenario()
    {
        // Arrange
        var osmIds = new List<OsmId>
        {
            new(OsmType.Relation, 100),
            new(OsmType.Node, 300),
            new(OsmType.Way, 200),
            new(OsmType.Node, 100),
            new(OsmType.Way, 100),
        };

        // Act
        var sorted = osmIds.OrderBy(x => x).ToList();

        // Assert
        sorted[0].Should().Be(new OsmId(OsmType.Node, 100));
        sorted[1].Should().Be(new OsmId(OsmType.Node, 300));
        sorted[2].Should().Be(new OsmId(OsmType.Way, 100));
        sorted[3].Should().Be(new OsmId(OsmType.Way, 200));
        sorted[4].Should().Be(new OsmId(OsmType.Relation, 100));
    }

    #endregion

    #region Roundtrip Tests

    [TestMethod]
    public void Roundtrip_ShouldPreserveValue_ForNode()
    {
        // Arrange
        var original = new OsmId(OsmType.Node, 123456789);

        // Act
        var stringValue = original.ToString();
        var success = OsmId.TryParse(stringValue, out var parsed);

        // Assert
        success.Should().BeTrue();
        parsed.Should().Be(original);
    }

    [TestMethod]
    public void Roundtrip_ShouldPreserveValue_ForWay()
    {
        // Arrange
        var original = new OsmId(OsmType.Way, 987654321);

        // Act
        var stringValue = original.ToString();
        var success = OsmId.TryParse(stringValue, out var parsed);

        // Assert
        success.Should().BeTrue();
        parsed.Should().Be(original);
    }

    [TestMethod]
    public void Roundtrip_ShouldPreserveValue_ForRelation()
    {
        // Arrange
        var original = new OsmId(OsmType.Relation, 555555);

        // Act
        var stringValue = original.ToString();
        var success = OsmId.TryParse(stringValue, out var parsed);

        // Assert
        success.Should().BeTrue();
        parsed.Should().Be(original);
    }

    #endregion
}
