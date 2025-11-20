// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Criteria;
using Viae.Domain.Tests.Helpers;

namespace Viae.Domain.Tests.Criteria;

[TestClass]
public class LocusCriteriaBuilderTests
{
    [TestMethod]
    public void Near_ShouldSetNearPointAndDistance()
    {
        // Arrange
        var point = TestDataBuilders.APoint(37.7749, -122.4194);
        var distance = 5000;

        // Act
        var criteria = new LocusCriteriaBuilder().Near(point, distance).Build();

        // Assert
        criteria.NearPoint.Should().Be(point);
        criteria.MaxDistanceMeters.Should().Be(distance);
    }

    [TestMethod]
    public void WithMinThemata_ShouldSetMinimumThemeCount()
    {
        // Arrange & Act
        var criteria = new LocusCriteriaBuilder().WithMinThemata(3).Build();

        // Assert
        criteria.MinThemataCount.Should().Be(3);
    }

    [TestMethod]
    public void HasThema_ShouldSetThemeIdFilter()
    {
        // Arrange
        var themaId = Guid.NewGuid().ToString();

        // Act
        var criteria = new LocusCriteriaBuilder().HasThema(themaId).Build();

        // Assert
        criteria.HasThemaId.Should().Be(themaId);
    }

    [TestMethod]
    public void SortByName_ShouldSetNameSortField()
    {
        // Arrange & Act
        var criteria = new LocusCriteriaBuilder().SortByName().Build();

        // Assert
        criteria.SortBy.Should().Be(LocusSortField.Name);
    }

    [TestMethod]
    public void SortByDistance_ShouldSetDistanceSortField()
    {
        // Arrange & Act
        var criteria = new LocusCriteriaBuilder().SortByDistance().Build();

        // Assert
        criteria.SortBy.Should().Be(LocusSortField.Distance);
    }

    [TestMethod]
    public void Descending_ShouldSetDescendingSortDirection()
    {
        // Arrange & Act
        var criteria = new LocusCriteriaBuilder().SortByName().Descending().Build();

        // Assert
        criteria.SortDirection.Should().Be(SortDirection.Descending);
    }

    [TestMethod]
    public void Page_ShouldSetPageNumberAndSize()
    {
        // Arrange & Act
        var criteria = new LocusCriteriaBuilder().Page(2, 25).Build();

        // Assert
        criteria.PageNumber.Should().Be(2);
        criteria.PageSize.Should().Be(25);
    }

    [TestMethod]
    public void IncludeVestigia_ShouldSetVestigiumCriteria()
    {
        // Arrange
        var vestigiumCriteria = new VestigiumCriteriaBuilder().ForPersona("persona-123").Build();

        // Act
        var criteria = new LocusCriteriaBuilder().IncludeVestigia(vestigiumCriteria).Build();

        // Assert
        criteria.IncludesVestigia.Should().NotBeNull();
        criteria.IncludesVestigia!.PersonaId.Should().Be("persona-123");
    }

    [TestMethod]
    public void FluentChain_ShouldCombineMultipleFilters()
    {
        // Arrange
        var point = TestDataBuilders.APoint(40.7128, -74.0060);
        var themaId = Guid.NewGuid().ToString();

        // Act
        var criteria = new LocusCriteriaBuilder()
            .Near(point, 3000)
            .WithMinThemata(2)
            .HasThema(themaId)
            .SortByDistance()
            .Page(1, 20)
            .Build();

        // Assert
        criteria.NearPoint.Should().Be(point);
        criteria.MaxDistanceMeters.Should().Be(3000);
        criteria.MinThemataCount.Should().Be(2);
        criteria.HasThemaId.Should().Be(themaId);
        criteria.SortBy.Should().Be(LocusSortField.Distance);
        criteria.PageNumber.Should().Be(1);
        criteria.PageSize.Should().Be(20);
    }
}
