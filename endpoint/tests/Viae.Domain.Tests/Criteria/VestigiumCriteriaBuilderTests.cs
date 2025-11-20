// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Criteria;
using Viae.Domain.Models;

namespace Viae.Domain.Tests.Criteria;

[TestClass]
public class VestigiumCriteriaBuilderTests
{
    [TestMethod]
    public void ForPersona_ShouldSetPersonaIdFilter()
    {
        // Arrange
        var personaId = Guid.NewGuid().ToString();

        // Act
        var criteria = new VestigiumCriteriaBuilder().ForPersona(personaId).Build();

        // Assert
        criteria.PersonaId.Should().Be(personaId);
    }

    [TestMethod]
    public void AtLocus_ShouldSetLocusIdFilter()
    {
        // Arrange
        var locusId = Guid.NewGuid().ToString();

        // Act
        var criteria = new VestigiumCriteriaBuilder().AtLocus(locusId).Build();

        // Assert
        criteria.LocusId.Should().Be(locusId);
    }

    [TestMethod]
    public void FromOrigo_ShouldSetOrigoFilter()
    {
        // Arrange & Act
        var criteria = new VestigiumCriteriaBuilder().FromOrigo(Origo.Externus).Build();

        // Assert
        criteria.Origo.Should().Be(Origo.Externus);
    }

    [TestMethod]
    public void After_ShouldSetHappenedAfterFilter()
    {
        // Arrange
        var date = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        // Act
        var criteria = new VestigiumCriteriaBuilder().After(date).Build();

        // Assert
        criteria.HappenedAfter.Should().Be(date);
    }

    [TestMethod]
    public void Before_ShouldSetHappenedBeforeFilter()
    {
        // Arrange
        var date = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        // Act
        var criteria = new VestigiumCriteriaBuilder().Before(date).Build();

        // Assert
        criteria.HappenedBefore.Should().Be(date);
    }

    [TestMethod]
    public void Between_ShouldSetBothTimeFilters()
    {
        // Arrange
        var startDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2025, 12, 31, 23, 59, 59, DateTimeKind.Utc);

        // Act
        var criteria = new VestigiumCriteriaBuilder().Between(startDate, endDate).Build();

        // Assert
        criteria.HappenedAfter.Should().Be(startDate);
        criteria.HappenedBefore.Should().Be(endDate);
    }

    [TestMethod]
    public void IncludeLatens_ShouldSetLatensFlag()
    {
        // Arrange & Act
        var criteria = new VestigiumCriteriaBuilder().IncludeLatens().Build();

        // Assert
        criteria.IncludeLatens.Should().BeTrue();
    }

    [TestMethod]
    public void ExcludeManifestus_ShouldUnsetManifestusFlag()
    {
        // Arrange & Act
        var criteria = new VestigiumCriteriaBuilder().ExcludeManifestus().Build();

        // Assert
        criteria.IncludeManifestus.Should().BeFalse();
    }

    [TestMethod]
    public void IncludeDeletus_ShouldSetDeletusFlag()
    {
        // Arrange & Act
        var criteria = new VestigiumCriteriaBuilder().IncludeDeletus().Build();

        // Assert
        criteria.IncludeDeletus.Should().BeTrue();
    }

    [TestMethod]
    public void SortByHappenedAt_ShouldSetHappenedAtSortField()
    {
        // Arrange & Act
        var criteria = new VestigiumCriteriaBuilder().SortByHappenedAt().Build();

        // Assert
        criteria.SortBy.Should().Be(VestigiumSortField.HappenedAt);
    }

    [TestMethod]
    public void SortByPersona_ShouldSetPersonaSortField()
    {
        // Arrange & Act
        var criteria = new VestigiumCriteriaBuilder().SortByPersona().Build();

        // Assert
        criteria.SortBy.Should().Be(VestigiumSortField.PersonaId);
    }

    [TestMethod]
    public void Ascending_ShouldSetAscendingSortDirection()
    {
        // Arrange & Act
        var criteria = new VestigiumCriteriaBuilder().SortByHappenedAt().Ascending().Build();

        // Assert
        criteria.SortDirection.Should().Be(SortDirection.Ascending);
    }

    [TestMethod]
    public void Descending_ShouldSetDescendingSortDirection()
    {
        // Arrange & Act
        var criteria = new VestigiumCriteriaBuilder().SortByHappenedAt().Descending().Build();

        // Assert
        criteria.SortDirection.Should().Be(SortDirection.Descending);
    }

    [TestMethod]
    public void Page_ShouldSetPageNumberAndSize()
    {
        // Arrange & Act
        var criteria = new VestigiumCriteriaBuilder().Page(3, 50).Build();

        // Assert
        criteria.PageNumber.Should().Be(3);
        criteria.PageSize.Should().Be(50);
    }

    [TestMethod]
    public void FluentChain_ShouldCombineAllFilters()
    {
        // Arrange
        var personaId = Guid.NewGuid().ToString();
        var locusId = Guid.NewGuid().ToString();
        var startDate = new DateTime(2025, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var endDate = new DateTime(2025, 6, 30, 23, 59, 59, DateTimeKind.Utc);

        // Act
        var criteria = new VestigiumCriteriaBuilder()
            .ForPersona(personaId)
            .AtLocus(locusId)
            .FromOrigo(Origo.Domesticus)
            .Between(startDate, endDate)
            .IncludeLatens()
            .IncludeDeletus()
            .SortByHappenedAt()
            .Descending()
            .Page(2, 25)
            .Build();

        // Assert
        criteria.PersonaId.Should().Be(personaId);
        criteria.LocusId.Should().Be(locusId);
        criteria.Origo.Should().Be(Origo.Domesticus);
        criteria.HappenedAfter.Should().Be(startDate);
        criteria.HappenedBefore.Should().Be(endDate);
        criteria.IncludeLatens.Should().BeTrue();
        criteria.IncludeDeletus.Should().BeTrue();
        criteria.SortBy.Should().Be(VestigiumSortField.HappenedAt);
        criteria.SortDirection.Should().Be(SortDirection.Descending);
        criteria.PageNumber.Should().Be(2);
        criteria.PageSize.Should().Be(25);
    }
}
