// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Viae.Domain.Criteria;
using Viae.Domain.Models;
using Viae.Persistence;
using Viae.Services.Tests.TestBuilders;
using Viae.Services.Tests.TestData;
using Viae.Services.Tests.TestExtensions;
using Viae.Testing.Common;

namespace Viae.Services.Tests;

/// <summary>
/// Tests for the LocusService that handles Locus entity queries.
/// </summary>
[TestClass]
public class LocusServiceTests : IDisposable
{
    private ViaeDbContext _dbContext = null!;
    private LocusService _service = null!;
    private bool _disposed;

    [TestInitialize]
    public void TestInitialize()
    {
        var options = new DbContextOptionsBuilder<ViaeDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        var factory = new TestDbContextFactory(options);

        _dbContext = factory.CreateDbContext();
        _service = new LocusService(factory);
    }

    [TestCleanup]
    public void TestCleanup()
    {
        Dispose();
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _dbContext?.Dispose();
        _disposed = true;
        GC.SuppressFinalize(this);
    }

    #region GetLocusByIdAsync Tests

    [TestMethod]
    public async Task GetLocusByIdAsync_ShouldReturnLocus_WhenExists()
    {
        // Arrange
        var locus = LocusBuilder.Default().WithName("Golden Gate Bridge").WithId("test123").Build();
        _dbContext.Loca.Add(locus);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        // Act
        var result = await _service.GetLocusByIdAsync("test123", TestContext.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("test123");
        result.Name.Should().Be("Golden Gate Bridge");
    }

    [TestMethod]
    public async Task GetLocusByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Act
        var result = await _service.GetLocusByIdAsync("nonexistent", TestContext.CancellationToken);

        // Assert
        result.Should().BeNull();
    }

    #endregion

    #region GetLocaAsync - Basic Tests

    [TestMethod]
    public async Task GetLocaAsync_ShouldReturnAllLoca_WhenNoCriteria()
    {
        // Arrange
        var loca = new[]
        {
            LocusBuilder.Simple("Location 1").Build(),
            LocusBuilder.Simple("Location 2").Build(),
            LocusBuilder.Simple("Location 3").Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        // Act
        var result = await _service.GetLocaAsync(
            new LocusCriteria(),
            TestContext.CancellationToken
        );

        // Assert
        result.Should().HaveCount(3);
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldReturnEmptyList_WhenNoLoca()
    {
        // Act
        var result = await _service.GetLocaAsync(
            new LocusCriteria(),
            TestContext.CancellationToken
        );

        // Assert
        result.ShouldBeEmpty();
    }

    #endregion

    #region GetLocaAsync - Geospatial Filtering Tests

    [TestMethod]
    [Ignore(
        "InMemory database does not support PostGIS spatial distance calculations. This test requires a real PostgreSQL database with PostGIS."
    )]
    public async Task GetLocaAsync_ShouldFilterByDistance_WhenNearPointSpecified()
    {
        // Arrange
        var loca = new[]
        {
            LocusBuilder
                .Default()
                .WithName("Near Location")
                .WithCoordinates(TestCoordinates.VeryNear)
                .Build(),
            LocusBuilder
                .Default()
                .WithName("Far Location")
                .WithCoordinates(TestCoordinates.VeryFar)
                .Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria
        {
            NearPoint = TestCoordinates.SanFranciscoCenter,
            MaxDistanceMeters = 1000,
        };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleLocus("Near Location");
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldNotFilterByDistance_WhenOnlyNearPointSpecified()
    {
        // Arrange
        var locus = LocusBuilder.Simple().Build();
        _dbContext.Loca.Add(locus);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria { NearPoint = TestCoordinates.VeryFar };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(1);
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldNotFilterByDistance_WhenOnlyMaxDistanceSpecified()
    {
        // Arrange
        var locus = LocusBuilder.Simple().Build();
        _dbContext.Loca.Add(locus);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria { MaxDistanceMeters = 1000 };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(1);
    }

    #endregion

    #region GetLocaAsync - Themata Filtering Tests

    [TestMethod]
    public async Task GetLocaAsync_ShouldFilterByMinThemataCount()
    {
        // Arrange
        var thema1 = ThemaBuilder.Nature();
        var thema2 = ThemaBuilder.Culture();
        var thema3 = ThemaBuilder.History();

        var loca = new[]
        {
            LocusBuilder.Simple("Location with no themata").Build(),
            LocusBuilder.Simple("Location with one thema").WithThema(thema1).Build(),
            LocusBuilder
                .Simple("Location with three themata")
                .WithThemata(thema1, thema2, thema3)
                .Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria { MinThemataCount = 2 };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleLocus("Location with three themata");
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldFilterByHasThemaId()
    {
        // Arrange
        var thema1 = ThemaBuilder
            .Default()
            .WithName("Theme 1")
            .WithSlug("theme-1")
            .WithId("theme1")
            .Build();
        var thema2 = ThemaBuilder
            .Default()
            .WithName("Theme 2")
            .WithSlug("theme-2")
            .WithId("theme2")
            .Build();

        var loca = new[]
        {
            LocusBuilder.Simple("Location with theme1").WithThema(thema1).Build(),
            LocusBuilder.Simple("Location with theme2").WithThema(thema2).Build(),
            LocusBuilder.Simple("Location with both themes").WithThemata(thema1, thema2).Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria { HasThemaId = "theme1" };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.ShouldContainLoca("Location with theme1", "Location with both themes");
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldNotFilterByHasThemaId_WhenEmpty()
    {
        // Arrange
        var locus = LocusBuilder.Simple().Build();
        _dbContext.Loca.Add(locus);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria { HasThemaId = string.Empty };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(1);
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldCombineThemataFilters()
    {
        // Arrange
        var thema1 = ThemaBuilder.Default().WithNameAndSlug("Theme 1").WithId("theme1").Build();
        var thema2 = ThemaBuilder.Default().WithNameAndSlug("Theme 2").WithId("theme2").Build();
        var thema3 = ThemaBuilder.Default().WithNameAndSlug("Theme 3").WithId("theme3").Build();

        var loca = new[]
        {
            LocusBuilder.Simple("Location with theme1 only").WithThema(thema1).Build(),
            LocusBuilder
                .Simple("Location with theme1 and theme2")
                .WithThemata(thema1, thema2)
                .Build(),
            LocusBuilder
                .Simple("Location with all three themes")
                .WithThemata(thema1, thema2, thema3)
                .Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria { HasThemaId = "theme1", MinThemataCount = 2 };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.ShouldContainLoca(
            "Location with theme1 and theme2",
            "Location with all three themes"
        );
    }

    #endregion

    #region GetLocaAsync - Sorting Tests

    [TestMethod]
    public async Task GetLocaAsync_ShouldSortByName_Ascending()
    {
        // Arrange
        var loca = new[]
        {
            LocusBuilder.Simple("Zebra Location").Build(),
            LocusBuilder.Simple("Alpha Location").Build(),
            LocusBuilder.Simple("Bravo Location").Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria
        {
            SortBy = LocusSortField.Name,
            SortDirection = SortDirection.Ascending,
        };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.ShouldContainInOrder("Alpha Location", "Bravo Location", "Zebra Location");
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldSortByName_Descending()
    {
        // Arrange
        var loca = new[]
        {
            LocusBuilder.Simple("Alpha Location").Build(),
            LocusBuilder.Simple("Bravo Location").Build(),
            LocusBuilder.Simple("Zebra Location").Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria
        {
            SortBy = LocusSortField.Name,
            SortDirection = SortDirection.Descending,
        };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.ShouldContainInOrder("Zebra Location", "Bravo Location", "Alpha Location");
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldSortByThemataCount_Ascending()
    {
        // Arrange
        var thema1 = ThemaBuilder.Nature();
        var thema2 = ThemaBuilder.Culture();
        var thema3 = ThemaBuilder.History();

        var loca = new[]
        {
            LocusBuilder.Simple("Three Themata").WithThemata(thema1, thema2, thema3).Build(),
            LocusBuilder.Simple("No Themata").Build(),
            LocusBuilder.Simple("One Thema").WithThema(thema1).Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria
        {
            SortBy = LocusSortField.ThemataCount,
            SortDirection = SortDirection.Ascending,
        };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.ShouldContainInOrder("No Themata", "One Thema", "Three Themata");
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldSortByThemataCount_Descending()
    {
        // Arrange
        var thema1 = ThemaBuilder.Nature();
        var thema2 = ThemaBuilder.Culture();

        var loca = new[]
        {
            LocusBuilder.Simple("No Themata").Build(),
            LocusBuilder.Simple("Two Themata").WithThemata(thema1, thema2).Build(),
            LocusBuilder.Simple("One Thema").WithThema(thema1).Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria
        {
            SortBy = LocusSortField.ThemataCount,
            SortDirection = SortDirection.Descending,
        };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.ShouldContainInOrder("Two Themata", "One Thema", "No Themata");
    }

    [TestMethod]
    [Ignore(
        "InMemory database does not support PostGIS spatial distance calculations for sorting. This test requires a real PostgreSQL database with PostGIS."
    )]
    public async Task GetLocaAsync_ShouldSortByDistance_Ascending_WhenNearPointSpecified()
    {
        // Arrange
        var loca = new[]
        {
            LocusBuilder
                .Default()
                .WithName("Farthest")
                .WithCoordinates(TestCoordinates.ModeratelyClose)
                .Build(),
            LocusBuilder
                .Default()
                .WithName("Nearest")
                .WithCoordinates(TestCoordinates.VeryNear)
                .Build(),
            LocusBuilder
                .Default()
                .WithName("Middle")
                .WithCoordinates(TestCoordinates.LocationA)
                .Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria
        {
            NearPoint = TestCoordinates.SanFranciscoCenter,
            SortBy = LocusSortField.Distance,
            SortDirection = SortDirection.Ascending,
        };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Nearest");
        result[2].Name.Should().Be("Farthest");
    }

    [TestMethod]
    [Ignore(
        "InMemory database does not support PostGIS spatial distance calculations for sorting. This test requires a real PostgreSQL database with PostGIS."
    )]
    public async Task GetLocaAsync_ShouldSortByDistance_Descending_WhenNearPointSpecified()
    {
        // Arrange
        var loca = new[]
        {
            LocusBuilder
                .Default()
                .WithName("Nearest")
                .WithCoordinates(TestCoordinates.VeryNear)
                .Build(),
            LocusBuilder
                .Default()
                .WithName("Farthest")
                .WithCoordinates(TestCoordinates.ModeratelyClose)
                .Build(),
            LocusBuilder
                .Default()
                .WithName("Middle")
                .WithCoordinates(TestCoordinates.LocationA)
                .Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria
        {
            NearPoint = TestCoordinates.SanFranciscoCenter,
            SortBy = LocusSortField.Distance,
            SortDirection = SortDirection.Descending,
        };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result[0].Name.Should().Be("Farthest");
        result[2].Name.Should().Be("Nearest");
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldFallbackToNameSort_WhenDistanceSortWithoutNearPoint()
    {
        // Arrange
        var loca = new[]
        {
            LocusBuilder.Simple("Zebra").Build(),
            LocusBuilder.Simple("Alpha").Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria { SortBy = LocusSortField.Distance };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.ShouldContainInOrder("Alpha", "Zebra");
    }

    #endregion

    #region GetLocaAsync - Pagination Tests

    [TestMethod]
    public async Task GetLocaAsync_ShouldPaginate_WhenPageNumberAndSizeSpecified()
    {
        // Arrange
        var loca = Enumerable
            .Range(1, 10)
            .Select(i => LocusBuilder.Simple($"Location {i:D2}").Build())
            .ToArray();
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria
        {
            PageNumber = 2,
            PageSize = 3,
            SortBy = LocusSortField.Name,
        };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.ShouldContainInOrder("Location 04", "Location 05", "Location 06");
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldReturnFirstPage_WhenPageNumberIsOne()
    {
        // Arrange
        var loca = Enumerable
            .Range(1, 10)
            .Select(i => LocusBuilder.Simple($"Location {i:D2}").Build())
            .ToArray();
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria
        {
            PageNumber = 1,
            PageSize = 5,
            SortBy = LocusSortField.Name,
        };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(5);
        result[0].Name.Should().Be("Location 01");
        result[4].Name.Should().Be("Location 05");
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldReturnPartialPage_WhenLastPage()
    {
        // Arrange
        var loca = Enumerable
            .Range(1, 7)
            .Select(i => LocusBuilder.Simple($"Location {i:D2}").Build())
            .ToArray();
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria
        {
            PageNumber = 3,
            PageSize = 3,
            SortBy = LocusSortField.Name,
        };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleLocus("Location 07");
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldNotPaginate_WhenOnlyPageNumberSpecified()
    {
        // Arrange
        var loca = Enumerable
            .Range(1, 5)
            .Select(i => LocusBuilder.Simple($"Location {i}").Build())
            .ToArray();
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria { PageNumber = 2 };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(5);
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldNotPaginate_WhenOnlyPageSizeSpecified()
    {
        // Arrange
        var loca = Enumerable
            .Range(1, 5)
            .Select(i => LocusBuilder.Simple($"Location {i}").Build())
            .ToArray();
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria { PageSize = 2 };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(5);
    }

    #endregion

    #region GetLocaAsync - Complex Integration Tests

    [TestMethod]
    [Ignore(
        "InMemory database does not support PostGIS spatial distance calculations. This test requires a real PostgreSQL database with PostGIS."
    )]
    public async Task GetLocaAsync_ShouldCombineAllFiltersAndSorting()
    {
        // Arrange
        var thema1 = ThemaBuilder.Default().WithNameAndSlug("Theme 1").WithId("theme1").Build();
        var thema2 = ThemaBuilder.Default().WithNameAndSlug("Theme 2").WithId("theme2").Build();

        var loca = new[]
        {
            LocusBuilder
                .Default()
                .WithName("Near with theme1, two themata")
                .WithCoordinates(TestCoordinates.VeryNear)
                .WithCreatedAt(DateTime.UtcNow.AddDays(-2))
                .WithThemata(thema1, thema2)
                .Build(),
            LocusBuilder
                .Default()
                .WithName("Near with theme1, one thema")
                .WithCoordinates(TestCoordinates.ModeratelyClose)
                .WithCreatedAt(DateTime.UtcNow.AddDays(-1))
                .WithThema(thema1)
                .Build(),
            LocusBuilder
                .Default()
                .WithName("Far with theme1")
                .WithCoordinates(TestCoordinates.VeryFar)
                .WithCreatedAt(DateTime.UtcNow)
                .WithThemata(thema1, thema2)
                .Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria
        {
            NearPoint = TestCoordinates.SanFranciscoCenter,
            MaxDistanceMeters = 5000,
            HasThemaId = "theme1",
            MinThemataCount = 2,
            SortBy = LocusSortField.CreatedAt,
            SortDirection = SortDirection.Descending,
        };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleLocus("Near with theme1, two themata");
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldCombineFiltersSortingAndPagination()
    {
        // Arrange
        var thema1 = ThemaBuilder.Default().WithNameAndSlug("Theme 1").WithId("theme1").Build();

        var loca = Enumerable
            .Range(1, 10)
            .Select(i =>
                LocusBuilder
                    .Simple($"Location {i:D2}")
                    .WithThemata(i % 2 == 0 ? [thema1] : [])
                    .Build()
            )
            .ToArray();
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria
        {
            HasThemaId = "theme1",
            SortBy = LocusSortField.Name,
            SortDirection = SortDirection.Ascending,
            PageNumber = 1,
            PageSize = 2,
        };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.ShouldContainInOrder("Location 02", "Location 04");
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldHandleEmptyResult_WithComplexCriteria()
    {
        // Arrange
        var locus = LocusBuilder
            .Default()
            .WithName("Far Location")
            .WithCoordinates(TestCoordinates.VeryFar)
            .Build();
        _dbContext.Loca.Add(locus);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteria
        {
            NearPoint = TestCoordinates.SanFranciscoCenter,
            MaxDistanceMeters = 1000,
            MinThemataCount = 1,
        };

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldBeEmpty();
    }

    #endregion

    #region GetLocaAsync - Cancellation Token Tests

    [TestMethod]
    public async Task GetLocaAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () => await _service.GetLocaAsync(new LocusCriteria(), cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    [TestMethod]
    public async Task GetLocusByIdAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () => await _service.GetLocusByIdAsync("test123", cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region LocusCriteriaBuilder Integration Tests

    [TestMethod]
    [Ignore(
        "InMemory database does not support PostGIS spatial distance calculations. This test requires a real PostgreSQL database with PostGIS."
    )]
    public async Task GetLocaAsync_ShouldWorkWithCriteriaBuilder_NearQuery()
    {
        // Arrange
        var loca = new[]
        {
            LocusBuilder
                .Default()
                .WithName("Near")
                .WithCoordinates(TestCoordinates.VeryNear)
                .Build(),
            LocusBuilder.Default().WithName("Far").WithCoordinates(TestCoordinates.VeryFar).Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteriaBuilder()
            .Near(TestCoordinates.SanFranciscoCenter, 5000)
            .SortByDistance()
            .Build();

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleLocus("Near");
    }

    [TestMethod]
    public async Task GetLocaAsync_ShouldWorkWithCriteriaBuilder_ComplexQuery()
    {
        // Arrange
        var thema1 = ThemaBuilder.Default().WithNameAndSlug("Theme 1").WithId("theme1").Build();
        var thema2 = ThemaBuilder.Default().WithNameAndSlug("Theme 2").WithId("theme2").Build();

        var loca = new[]
        {
            LocusBuilder.Simple("Match").WithThemata(thema1, thema2).Build(),
            LocusBuilder.Simple("No Match").WithThema(thema1).Build(),
        };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new LocusCriteriaBuilder()
            .HasThema("theme1")
            .WithMinThemata(2)
            .SortByName()
            .Descending()
            .Build();

        // Act
        var result = await _service.GetLocaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleLocus("Match");
    }

    #endregion

    #region GetLocaDistancesAsync Tests

    [TestMethod]
    [Ignore(
        "InMemory database does not support PostGIS spatial distance calculations. This test requires a real PostgreSQL database with PostGIS."
    )]
    public async Task GetLocaDistancesAsync_ShouldReturnDistancesFromReferencePoint()
    {
        // Arrange
        var referenceLocus = LocusBuilder
            .Default()
            .WithName("Reference")
            .WithCoordinates(TestCoordinates.SanFranciscoCenter)
            .WithId("reference")
            .Build();

        var nearLocus = LocusBuilder
            .Default()
            .WithName("Near")
            .WithCoordinates(TestCoordinates.VeryNear)
            .WithId("near")
            .Build();

        var farLocus = LocusBuilder
            .Default()
            .WithName("Far")
            .WithCoordinates(TestCoordinates.VeryFar)
            .WithId("far")
            .Build();

        var loca = new[] { referenceLocus, nearLocus, farLocus };
        _dbContext.Loca.AddRange(loca);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        // Act
        var result = await _service.GetLocaDistancesAsync(
            [nearLocus, farLocus],
            referenceLocus,
            TestContext.CancellationToken
        );

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        result.Should().ContainKey("near");
        result.Should().ContainKey("far");
        result["near"].Should().BeLessThan(result["far"]);
    }

    [TestMethod]
    public async Task GetLocaDistancesAsync_ShouldReturnEmptyDictionary_WhenNoLoca()
    {
        // Arrange
        var referenceLocus = LocusBuilder.Default().Build();
        _dbContext.Loca.Add(referenceLocus);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        // Act
        var result = await _service.GetLocaDistancesAsync(
            [],
            referenceLocus,
            TestContext.CancellationToken
        );

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();
    }

    [TestMethod]
    public async Task GetLocaDistancesAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var referenceLocus = LocusBuilder.Default().Build();
        var locus = LocusBuilder.Default().Build();
        _dbContext.Loca.AddRange(referenceLocus, locus);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () =>
            await _service.GetLocaDistancesAsync([locus], referenceLocus, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region GetVestigiumByIdAsync Tests

    [TestMethod]
    public async Task GetVestigiumByIdAsync_ShouldReturnVestigium_WhenExists()
    {
        // Arrange
        var persona = PersonaBuilder.Simple("user1").Build();
        var locus = LocusBuilder.Simple("Test Location").Build();
        var vestigium = VestigiumBuilder
            .Default()
            .WithId("vest123")
            .WithPersona(persona)
            .WithLocus(locus)
            .WithContent("Visit to test location")
            .Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        // Act
        var result = await _service.GetVestigiumByIdAsync("vest123", TestContext.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("vest123");
        result.Content.Should().Be("Visit to test location");
    }

    [TestMethod]
    public async Task GetVestigiumByIdAsync_ShouldReturnNull_WhenNotFound()
    {
        // Act
        var result = await _service.GetVestigiumByIdAsync(
            "nonexistent",
            TestContext.CancellationToken
        );

        // Assert
        result.Should().BeNull();
    }

    [TestMethod]
    public async Task GetVestigiumByIdAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () => await _service.GetVestigiumByIdAsync("test123", cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region GetVestigiaAsync - Basic Tests

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldReturnManifestusVestigia_WhenDefaultCriteria()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();

        var vestigia = new[]
        {
            VestigiumBuilder
                .Simple("Published 1")
                .WithPersona(persona)
                .WithLocus(locus)
                .AsPublished()
                .Build(),
            VestigiumBuilder
                .Simple("Draft")
                .WithPersona(persona)
                .WithLocus(locus)
                .AsDraft()
                .Build(),
            VestigiumBuilder
                .Simple("Deleted")
                .WithPersona(persona)
                .WithLocus(locus)
                .AsTombstoned()
                .Build(),
        };

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria();

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleVestigium("Published 1");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldReturnEmptyList_WhenNoVestigia()
    {
        // Act
        var result = await _service.GetVestigiaAsync(
            new VestigiumCriteria(),
            TestContext.CancellationToken
        );

        // Assert
        result.ShouldBeEmpty();
    }

    #endregion

    #region GetVestigiaAsync - Persona Filtering Tests

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldFilterByPersonaId()
    {
        // Arrange
        var persona1 = PersonaBuilder.Simple("user1").WithId("persona1").Build();
        var persona2 = PersonaBuilder.Simple("user2").WithId("persona2").Build();
        var locus = LocusBuilder.Simple().Build();

        var vestigia = new[]
        {
            VestigiumBuilder
                .Simple("Persona 1 event")
                .WithPersona(persona1)
                .WithLocus(locus)
                .Build(),
            VestigiumBuilder
                .Simple("Persona 2 event")
                .WithPersona(persona2)
                .WithLocus(locus)
                .Build(),
        };

        _dbContext.Personae.AddRange(persona1, persona2);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria { PersonaId = "persona1" };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleVestigium("Persona 1 event");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldNotFilterByPersonaId_WhenEmpty()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var vestigium = VestigiumBuilder.Simple().WithPersona(persona).WithLocus(locus).Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria { PersonaId = string.Empty };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(1);
    }

    #endregion

    #region GetVestigiaAsync - Locus Filtering Tests

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldFilterByLocusId()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus1 = LocusBuilder.Simple("Location 1").WithId("locus1").Build();
        var locus2 = LocusBuilder.Simple("Location 2").WithId("locus2").Build();

        var vestigia = new[]
        {
            VestigiumBuilder.Simple("At locus 1").WithPersona(persona).WithLocus(locus1).Build(),
            VestigiumBuilder.Simple("At locus 2").WithPersona(persona).WithLocus(locus2).Build(),
        };

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.AddRange(locus1, locus2);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria { LocusId = "locus1" };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleVestigium("At locus 1");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldNotFilterByLocusId_WhenEmpty()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var vestigium = VestigiumBuilder.Simple().WithPersona(persona).WithLocus(locus).Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria { LocusId = string.Empty };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(1);
    }

    #endregion

    #region GetVestigiaAsync - Origo Filtering Tests

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldFilterByOrigo_Domesticus()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();

        var vestigia = new[]
        {
            VestigiumBuilder
                .Simple("Local event")
                .WithPersona(persona)
                .WithLocus(locus)
                .AsLocal()
                .Build(),
            VestigiumBuilder
                .Simple("Remote event")
                .WithPersona(persona)
                .WithLocus(locus)
                .AsRemote()
                .Build(),
        };

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria { Origo = Origo.Domesticus };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleVestigium("Local event");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldFilterByOrigo_Externus()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();

        var vestigia = new[]
        {
            VestigiumBuilder
                .Simple("Local event")
                .WithPersona(persona)
                .WithLocus(locus)
                .AsLocal()
                .Build(),
            VestigiumBuilder
                .Simple("Remote event")
                .WithPersona(persona)
                .WithLocus(locus)
                .AsRemote()
                .Build(),
        };

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria { Origo = Origo.Externus };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleVestigium("Remote event");
    }

    #endregion

    #region GetVestigiaAsync - Date Range Filtering Tests

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldFilterByHappenedAfter()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var baseDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var vestigia = new[]
        {
            VestigiumBuilder
                .Simple("Old event")
                .WithPersona(persona)
                .WithLocus(locus)
                .WithHappenedAt(baseDate)
                .Build(),
            VestigiumBuilder
                .Simple("Recent event")
                .WithPersona(persona)
                .WithLocus(locus)
                .WithHappenedAt(baseDate.AddDays(5))
                .Build(),
        };

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria { HappenedAfter = baseDate.AddDays(3) };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleVestigium("Recent event");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldFilterByHappenedBefore()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var baseDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var vestigia = new[]
        {
            VestigiumBuilder
                .Simple("Old event")
                .WithPersona(persona)
                .WithLocus(locus)
                .WithHappenedAt(baseDate)
                .Build(),
            VestigiumBuilder
                .Simple("Recent event")
                .WithPersona(persona)
                .WithLocus(locus)
                .WithHappenedAt(baseDate.AddDays(5))
                .Build(),
        };

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria { HappenedBefore = baseDate.AddDays(3) };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleVestigium("Old event");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldFilterByDateRange()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var baseDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var vestigia = new[]
        {
            VestigiumBuilder
                .Simple("Too old")
                .WithPersona(persona)
                .WithLocus(locus)
                .WithHappenedAt(baseDate)
                .Build(),
            VestigiumBuilder
                .Simple("In range")
                .WithPersona(persona)
                .WithLocus(locus)
                .WithHappenedAt(baseDate.AddDays(5))
                .Build(),
            VestigiumBuilder
                .Simple("Too recent")
                .WithPersona(persona)
                .WithLocus(locus)
                .WithHappenedAt(baseDate.AddDays(10))
                .Build(),
        };

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            HappenedAfter = baseDate.AddDays(3),
            HappenedBefore = baseDate.AddDays(7),
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleVestigium("In range");
    }

    #endregion

    #region GetVestigiaAsync - Status Filtering Tests

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldIncludeLatens_WhenFlagSet()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();

        var vestigia = new[]
        {
            VestigiumBuilder
                .Simple("Draft")
                .WithPersona(persona)
                .WithLocus(locus)
                .AsDraft()
                .Build(),
            VestigiumBuilder
                .Simple("Published")
                .WithPersona(persona)
                .WithLocus(locus)
                .AsPublished()
                .Build(),
        };

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria { IncludeLatens = true, IncludeManifestus = false };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleVestigium("Draft");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldIncludeDeletus_WhenFlagSet()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();

        var vestigia = new[]
        {
            VestigiumBuilder
                .Simple("Deleted")
                .WithPersona(persona)
                .WithLocus(locus)
                .AsTombstoned()
                .Build(),
            VestigiumBuilder
                .Simple("Published")
                .WithPersona(persona)
                .WithLocus(locus)
                .AsPublished()
                .Build(),
        };

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria { IncludeDeletus = true, IncludeManifestus = false };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleVestigium("Deleted");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldIncludeMultipleStatuses()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();

        var vestigia = new[]
        {
            VestigiumBuilder
                .Simple("Draft")
                .WithPersona(persona)
                .WithLocus(locus)
                .AsDraft()
                .Build(),
            VestigiumBuilder
                .Simple("Published")
                .WithPersona(persona)
                .WithLocus(locus)
                .AsPublished()
                .Build(),
            VestigiumBuilder
                .Simple("Deleted")
                .WithPersona(persona)
                .WithLocus(locus)
                .AsTombstoned()
                .Build(),
        };

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            IncludeLatens = true,
            IncludeManifestus = true,
            IncludeDeletus = false,
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.ShouldContainVestigia("Draft", "Published");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldReturnEmpty_WhenNoStatusesIncluded()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var vestigium = VestigiumBuilder
            .Simple("Published")
            .WithPersona(persona)
            .WithLocus(locus)
            .Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            IncludeLatens = false,
            IncludeManifestus = false,
            IncludeDeletus = false,
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldBeEmpty();
    }

    #endregion

    #region GetVestigiaAsync - Sorting Tests

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldSortByHappenedAt_Ascending()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var baseDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var vestigia = new[]
        {
            VestigiumBuilder
                .Simple("Newest")
                .WithPersona(persona)
                .WithLocus(locus)
                .WithHappenedAt(baseDate.AddDays(2))
                .Build(),
            VestigiumBuilder
                .Simple("Oldest")
                .WithPersona(persona)
                .WithLocus(locus)
                .WithHappenedAt(baseDate)
                .Build(),
            VestigiumBuilder
                .Simple("Middle")
                .WithPersona(persona)
                .WithLocus(locus)
                .WithHappenedAt(baseDate.AddDays(1))
                .Build(),
        };

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            SortBy = VestigiumSortField.HappenedAt,
            SortDirection = SortDirection.Ascending,
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.ShouldContainInOrder("Oldest", "Middle", "Newest");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldSortByHappenedAt_Descending()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var baseDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var vestigia = new[]
        {
            VestigiumBuilder
                .Simple("Oldest")
                .WithPersona(persona)
                .WithLocus(locus)
                .WithHappenedAt(baseDate)
                .Build(),
            VestigiumBuilder
                .Simple("Middle")
                .WithPersona(persona)
                .WithLocus(locus)
                .WithHappenedAt(baseDate.AddDays(1))
                .Build(),
            VestigiumBuilder
                .Simple("Newest")
                .WithPersona(persona)
                .WithLocus(locus)
                .WithHappenedAt(baseDate.AddDays(2))
                .Build(),
        };

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            SortBy = VestigiumSortField.HappenedAt,
            SortDirection = SortDirection.Descending,
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.ShouldContainInOrder("Newest", "Middle", "Oldest");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldSortByPersonaId_Ascending()
    {
        // Arrange
        var persona1 = PersonaBuilder.Simple("aaa").WithId("persona_aaa").Build();
        var persona2 = PersonaBuilder.Simple("bbb").WithId("persona_bbb").Build();
        var persona3 = PersonaBuilder.Simple("ccc").WithId("persona_ccc").Build();
        var locus = LocusBuilder.Simple().Build();

        var vestigia = new[]
        {
            VestigiumBuilder.Simple("C event").WithPersona(persona3).WithLocus(locus).Build(),
            VestigiumBuilder.Simple("A event").WithPersona(persona1).WithLocus(locus).Build(),
            VestigiumBuilder.Simple("B event").WithPersona(persona2).WithLocus(locus).Build(),
        };

        _dbContext.Personae.AddRange(persona1, persona2, persona3);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            SortBy = VestigiumSortField.PersonaId,
            SortDirection = SortDirection.Ascending,
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.ShouldContainInOrder("A event", "B event", "C event");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldSortByPersonaId_Descending()
    {
        // Arrange
        var persona1 = PersonaBuilder.Simple("aaa").WithId("persona_aaa").Build();
        var persona2 = PersonaBuilder.Simple("bbb").WithId("persona_bbb").Build();
        var persona3 = PersonaBuilder.Simple("ccc").WithId("persona_ccc").Build();
        var locus = LocusBuilder.Simple().Build();

        var vestigia = new[]
        {
            VestigiumBuilder.Simple("A event").WithPersona(persona1).WithLocus(locus).Build(),
            VestigiumBuilder.Simple("B event").WithPersona(persona2).WithLocus(locus).Build(),
            VestigiumBuilder.Simple("C event").WithPersona(persona3).WithLocus(locus).Build(),
        };

        _dbContext.Personae.AddRange(persona1, persona2, persona3);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            SortBy = VestigiumSortField.PersonaId,
            SortDirection = SortDirection.Descending,
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.ShouldContainInOrder("C event", "B event", "A event");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldSortByLocusId_Ascending()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus1 = LocusBuilder.Simple("AAA").WithId("locus_aaa").Build();
        var locus2 = LocusBuilder.Simple("BBB").WithId("locus_bbb").Build();
        var locus3 = LocusBuilder.Simple("CCC").WithId("locus_ccc").Build();

        var vestigia = new[]
        {
            VestigiumBuilder.Simple("At C").WithPersona(persona).WithLocus(locus3).Build(),
            VestigiumBuilder.Simple("At A").WithPersona(persona).WithLocus(locus1).Build(),
            VestigiumBuilder.Simple("At B").WithPersona(persona).WithLocus(locus2).Build(),
        };

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.AddRange(locus1, locus2, locus3);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            SortBy = VestigiumSortField.LocusId,
            SortDirection = SortDirection.Ascending,
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.ShouldContainInOrder("At A", "At B", "At C");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldSortByLocusId_Descending()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus1 = LocusBuilder.Simple("AAA").WithId("locus_aaa").Build();
        var locus2 = LocusBuilder.Simple("BBB").WithId("locus_bbb").Build();
        var locus3 = LocusBuilder.Simple("CCC").WithId("locus_ccc").Build();

        var vestigia = new[]
        {
            VestigiumBuilder.Simple("At A").WithPersona(persona).WithLocus(locus1).Build(),
            VestigiumBuilder.Simple("At B").WithPersona(persona).WithLocus(locus2).Build(),
            VestigiumBuilder.Simple("At C").WithPersona(persona).WithLocus(locus3).Build(),
        };

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.AddRange(locus1, locus2, locus3);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            SortBy = VestigiumSortField.LocusId,
            SortDirection = SortDirection.Descending,
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.ShouldContainInOrder("At C", "At B", "At A");
    }

    #endregion

    #region GetVestigiaAsync - Pagination Tests

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldPaginate_WhenPageNumberAndSizeSpecified()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var baseDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var vestigia = Enumerable
            .Range(1, 10)
            .Select(i =>
                VestigiumBuilder
                    .Simple($"Event {i:D2}")
                    .WithPersona(persona)
                    .WithLocus(locus)
                    .WithHappenedAt(baseDate.AddHours(i))
                    .Build()
            )
            .ToArray();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            PageNumber = 2,
            PageSize = 3,
            SortBy = VestigiumSortField.HappenedAt,
            SortDirection = SortDirection.Descending,
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.ShouldContainInOrder("Event 07", "Event 06", "Event 05");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldReturnFirstPage_WhenPageNumberIsOne()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var baseDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var vestigia = Enumerable
            .Range(1, 10)
            .Select(i =>
                VestigiumBuilder
                    .Simple($"Event {i:D2}")
                    .WithPersona(persona)
                    .WithLocus(locus)
                    .WithHappenedAt(baseDate.AddHours(i))
                    .Build()
            )
            .ToArray();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            PageNumber = 1,
            PageSize = 5,
            SortBy = VestigiumSortField.HappenedAt,
            SortDirection = SortDirection.Descending,
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(5);
        result[0].Content.Should().Be("Event 10");
        result[4].Content.Should().Be("Event 06");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldReturnPartialPage_WhenLastPage()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var baseDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var vestigia = Enumerable
            .Range(1, 7)
            .Select(i =>
                VestigiumBuilder
                    .Simple($"Event {i:D2}")
                    .WithPersona(persona)
                    .WithLocus(locus)
                    .WithHappenedAt(baseDate.AddHours(i))
                    .Build()
            )
            .ToArray();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            PageNumber = 3,
            PageSize = 3,
            SortBy = VestigiumSortField.HappenedAt,
            SortDirection = SortDirection.Descending,
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldContainSingleVestigium("Event 01");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldNotPaginate_WhenOnlyPageNumberSpecified()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();

        var vestigia = Enumerable
            .Range(1, 5)
            .Select(i =>
                VestigiumBuilder.Simple($"Event {i}").WithPersona(persona).WithLocus(locus).Build()
            )
            .ToArray();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria { PageNumber = 2 };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(5);
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldNotPaginate_WhenOnlyPageSizeSpecified()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();

        var vestigia = Enumerable
            .Range(1, 5)
            .Select(i =>
                VestigiumBuilder.Simple($"Event {i}").WithPersona(persona).WithLocus(locus).Build()
            )
            .ToArray();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria { PageSize = 2 };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(5);
    }

    #endregion

    #region GetVestigiaAsync - Complex Integration Tests

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldCombineAllFiltersAndSorting()
    {
        // Arrange
        var persona1 = PersonaBuilder.Simple("user1").WithId("persona1").Build();
        var persona2 = PersonaBuilder.Simple("user2").WithId("persona2").Build();
        var locus1 = LocusBuilder.Simple("Location 1").WithId("locus1").Build();
        var locus2 = LocusBuilder.Simple("Location 2").WithId("locus2").Build();
        var baseDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var vestigia = new[]
        {
            // Should match: persona1, locus1, local, in date range, published
            VestigiumBuilder
                .Simple("Match 1")
                .WithPersona(persona1)
                .WithLocus(locus1)
                .AsLocal()
                .WithHappenedAt(baseDate.AddDays(5))
                .AsPublished()
                .Build(),
            // Should match: persona1, locus1, local, in date range, published
            VestigiumBuilder
                .Simple("Match 2")
                .WithPersona(persona1)
                .WithLocus(locus1)
                .AsLocal()
                .WithHappenedAt(baseDate.AddDays(3))
                .AsPublished()
                .Build(),
            // No match: wrong persona
            VestigiumBuilder
                .Simple("Wrong persona")
                .WithPersona(persona2)
                .WithLocus(locus1)
                .AsLocal()
                .WithHappenedAt(baseDate.AddDays(4))
                .AsPublished()
                .Build(),
            // No match: wrong locus
            VestigiumBuilder
                .Simple("Wrong locus")
                .WithPersona(persona1)
                .WithLocus(locus2)
                .AsLocal()
                .WithHappenedAt(baseDate.AddDays(4))
                .AsPublished()
                .Build(),
            // No match: remote origin
            VestigiumBuilder
                .Simple("Remote")
                .WithPersona(persona1)
                .WithLocus(locus1)
                .AsRemote()
                .WithHappenedAt(baseDate.AddDays(4))
                .AsPublished()
                .Build(),
            // No match: too old
            VestigiumBuilder
                .Simple("Too old")
                .WithPersona(persona1)
                .WithLocus(locus1)
                .AsLocal()
                .WithHappenedAt(baseDate.AddDays(1))
                .AsPublished()
                .Build(),
            // No match: too recent
            VestigiumBuilder
                .Simple("Too recent")
                .WithPersona(persona1)
                .WithLocus(locus1)
                .AsLocal()
                .WithHappenedAt(baseDate.AddDays(10))
                .AsPublished()
                .Build(),
            // No match: draft status
            VestigiumBuilder
                .Simple("Draft")
                .WithPersona(persona1)
                .WithLocus(locus1)
                .AsLocal()
                .WithHappenedAt(baseDate.AddDays(4))
                .AsDraft()
                .Build(),
        };

        _dbContext.Personae.AddRange(persona1, persona2);
        _dbContext.Loca.AddRange(locus1, locus2);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            PersonaId = "persona1",
            LocusId = "locus1",
            Origo = Origo.Domesticus,
            HappenedAfter = baseDate.AddDays(2),
            HappenedBefore = baseDate.AddDays(7),
            SortBy = VestigiumSortField.HappenedAt,
            SortDirection = SortDirection.Descending,
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(2);
        result.ShouldContainInOrder("Match 1", "Match 2");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldCombineFiltersSortingAndPagination()
    {
        // Arrange
        var persona = PersonaBuilder.Simple("user1").WithId("persona1").Build();
        var locus = LocusBuilder.Simple().Build();
        var baseDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        var vestigia = Enumerable
            .Range(1, 10)
            .Select(i =>
                VestigiumBuilder
                    .Simple($"Event {i:D2}")
                    .WithPersona(persona)
                    .WithLocus(locus)
                    .WithHappenedAt(baseDate.AddHours(i))
                    .AsPublished()
                    .Build()
            )
            .ToArray();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            PersonaId = "persona1",
            SortBy = VestigiumSortField.HappenedAt,
            SortDirection = SortDirection.Ascending,
            PageNumber = 2,
            PageSize = 3,
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().HaveCount(3);
        result.ShouldContainInOrder("Event 04", "Event 05", "Event 06");
    }

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldHandleEmptyResult_WithComplexCriteria()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var vestigium = VestigiumBuilder.Simple().WithPersona(persona).WithLocus(locus).Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria
        {
            PersonaId = "nonexistent",
            Origo = Origo.Externus,
            HappenedAfter = DateTime.UtcNow.AddDays(10),
        };

        // Act
        var result = await _service.GetVestigiaAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.ShouldBeEmpty();
    }

    #endregion

    #region GetVestigiaAsync - Cancellation Token Tests

    [TestMethod]
    public async Task GetVestigiaAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () =>
            await _service.GetVestigiaAsync(new VestigiumCriteria(), cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region GetVestigiaCountAsync Tests

    [TestMethod]
    public async Task GetVestigiaCountAsync_ShouldReturnCorrectCount()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();

        var vestigia = Enumerable
            .Range(1, 5)
            .Select(i =>
                VestigiumBuilder.Simple($"Event {i}").WithPersona(persona).WithLocus(locus).Build()
            )
            .ToArray();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria();

        // Act
        var result = await _service.GetVestigiaCountAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().Be(5);
    }

    [TestMethod]
    public async Task GetVestigiaCountAsync_ShouldReturnZero_WhenNoMatches()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var vestigium = VestigiumBuilder.Simple().WithPersona(persona).WithLocus(locus).Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria { PersonaId = "nonexistent" };

        // Act
        var result = await _service.GetVestigiaCountAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().Be(0);
    }

    [TestMethod]
    public async Task GetVestigiaCountAsync_ShouldApplyFilters()
    {
        // Arrange
        var persona1 = PersonaBuilder.Simple("user1").WithId("persona1").Build();
        var persona2 = PersonaBuilder.Simple("user2").WithId("persona2").Build();
        var locus = LocusBuilder.Simple().Build();

        var vestigia = new[]
        {
            VestigiumBuilder.Simple("P1 Event 1").WithPersona(persona1).WithLocus(locus).Build(),
            VestigiumBuilder.Simple("P1 Event 2").WithPersona(persona1).WithLocus(locus).Build(),
            VestigiumBuilder.Simple("P2 Event").WithPersona(persona2).WithLocus(locus).Build(),
        };

        _dbContext.Personae.AddRange(persona1, persona2);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.AddRange(vestigia);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var criteria = new VestigiumCriteria { PersonaId = "persona1" };

        // Act
        var result = await _service.GetVestigiaCountAsync(criteria, TestContext.CancellationToken);

        // Assert
        result.Should().Be(2);
    }

    [TestMethod]
    public async Task GetVestigiaCountAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () =>
            await _service.GetVestigiaCountAsync(new VestigiumCriteria(), cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region AddLocusAsync Tests

    [TestMethod]
    public async Task AddLocusAsync_ShouldAddLocus_AndReturnIt()
    {
        // Arrange
        var locus = LocusBuilder.Simple("New Location").Build();

        // Act
        var result = await _service.AddLocusAsync(locus, TestContext.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(locus.Id);
        result.Name.Should().Be("New Location");

        // Verify it's in the database
        var fromDb = await _dbContext.Loca.FindAsync([locus.Id], TestContext.CancellationToken);
        fromDb.Should().NotBeNull();
        fromDb!.Name.Should().Be("New Location");
    }

    [TestMethod]
    public async Task AddLocusAsync_ShouldPreserveAllProperties()
    {
        // Arrange
        var locus = LocusBuilder
            .Default()
            .WithName("Complete Location")
            .WithCoordinates(37.7749, -122.4194)
            .WithSlug("complete-location")
            .WithContent("Detailed content")
            .WithAddress("123 Main St")
            .WithCity("San Francisco")
            .WithCountry("USA")
            .Build();

        // Act
        var result = await _service.AddLocusAsync(locus, TestContext.CancellationToken);

        // Assert
        result.Name.Should().Be("Complete Location");
        result.Slug.Should().Be("complete-location");
        result.Content.Should().Be("Detailed content");
        result.Address.Should().Be("123 Main St");
        result.City.Should().Be("San Francisco");
        result.Country.Should().Be("USA");
        result.Latitude.Should().BeApproximately(37.7749, 0.0001);
        result.Longitude.Should().BeApproximately(-122.4194, 0.0001);
    }

    [TestMethod]
    public async Task AddLocusAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var locus = LocusBuilder.Simple().Build();
        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () => await _service.AddLocusAsync(locus, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region UpdateLocusAsync Tests

    [TestMethod]
    public async Task UpdateLocusAsync_ShouldUpdateLocus_AndReturnIt()
    {
        // Arrange
        var locus = LocusBuilder.Simple("Original Name").Build();
        _dbContext.Loca.Add(locus);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        locus.Name = "Updated Name";
        locus.Content = "Updated content";

        // Act
        var result = await _service.UpdateLocusAsync(locus, TestContext.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.Content.Should().Be("Updated content");

        // Verify it's updated in the database
        var fromDb = await _dbContext.Loca.FindAsync([locus.Id], TestContext.CancellationToken);
        fromDb.Should().NotBeNull();
        fromDb!.Name.Should().Be("Updated Name");
        fromDb.Content.Should().Be("Updated content");
    }

    [TestMethod]
    public async Task UpdateLocusAsync_ShouldUpdateAllProperties()
    {
        // Arrange
        var locus = LocusBuilder.Simple("Original").Build();
        _dbContext.Loca.Add(locus);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        locus.Name = "Updated Name";
        locus.Slug = "updated-slug";
        locus.Content = "Updated content";
        locus.Address = "456 New St";
        locus.City = "Oakland";
        locus.Country = "Canada";
        locus.Coordinates = new NetTopologySuite.Geometries.Point(-122.2711, 37.8044)
        {
            SRID = 4326,
        };

        // Act
        var result = await _service.UpdateLocusAsync(locus, TestContext.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Updated Name");
        result.Slug.Should().Be("updated-slug");
        result.Content.Should().Be("Updated content");
        result.Latitude.Should().BeApproximately(37.8044, 0.0001);
        result.Longitude.Should().BeApproximately(-122.2711, 0.0001);
    }

    [TestMethod]
    public async Task UpdateLocusAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var locus = LocusBuilder.Simple().Build();
        _dbContext.Loca.Add(locus);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () => await _service.UpdateLocusAsync(locus, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region AddVestigiumAsync Tests

    [TestMethod]
    public async Task AddVestigiumAsync_ShouldAddVestigium_AndReturnIt()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var vestigium = VestigiumBuilder
            .Simple("New event")
            .WithPersona(persona)
            .WithLocus(locus)
            .Build();

        // Act
        var result = await _service.AddVestigiumAsync(vestigium, TestContext.CancellationToken);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(vestigium.Id);
        result.Content.Should().Be("New event");

        // Verify it's in the database
        var fromDb = await _dbContext.Vestigia.FindAsync(
            [vestigium.Id],
            TestContext.CancellationToken
        );
        fromDb.Should().NotBeNull();
        fromDb!.Content.Should().Be("New event");
    }

    [TestMethod]
    public async Task AddVestigiumAsync_ShouldPreserveAllProperties()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var happenedAt = new DateTime(2025, 6, 15, 14, 30, 0, DateTimeKind.Utc);
        var vestigium = VestigiumBuilder
            .Default()
            .WithPersona(persona)
            .WithLocus(locus)
            .WithContent("Complete event details")
            .WithHappenedAt(happenedAt)
            .WithOrigo(Origo.Domesticus)
            .WithUri(new Uri("https://example.com/event/123"))
            .AsPublished()
            .Build();

        // Act
        var result = await _service.AddVestigiumAsync(vestigium, TestContext.CancellationToken);

        // Assert
        result.Content.Should().Be("Complete event details");
        result.HappenedAt.Should().Be(happenedAt);
        result.Origo.Should().Be(Origo.Domesticus);
        result.Uri.Should().Be(new Uri("https://example.com/event/123"));
        result.PublishedAt.Should().NotBeNull();
    }

    [TestMethod]
    public async Task AddVestigiumAsync_ShouldAddDraftVestigium()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var vestigium = VestigiumBuilder
            .Simple("Draft event")
            .WithPersona(persona)
            .WithLocus(locus)
            .AsDraft()
            .Build();

        // Act
        var result = await _service.AddVestigiumAsync(vestigium, TestContext.CancellationToken);

        // Assert
        result.Status.Should().Be(Status.Latens);
        result.PublishedAt.Should().BeNull();
        result.TombstonedAt.Should().BeNull();
    }

    [TestMethod]
    public async Task AddVestigiumAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var vestigium = VestigiumBuilder.Simple().WithPersona(persona).WithLocus(locus).Build();

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () => await _service.AddVestigiumAsync(vestigium, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region UpdateVestigiumAsync Tests

    [TestMethod]
    public async Task UpdateVestigiumAsync_ShouldUpdateHappenedAtAndContent()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var originalHappenedAt = new DateTime(2025, 1, 1, 12, 0, 0, DateTimeKind.Utc);
        var vestigium = VestigiumBuilder
            .Simple("Original content")
            .WithPersona(persona)
            .WithLocus(locus)
            .WithHappenedAt(originalHappenedAt)
            .Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var newHappenedAt = new DateTime(2025, 6, 15, 14, 30, 0, DateTimeKind.Utc);

        // Act
        var result = await _service.UpdateVestigiumAsync(
            vestigium.Id,
            newHappenedAt,
            "Updated content",
            TestContext.CancellationToken
        );

        // Assert
        result.Should().NotBeNull();
        result!.HappenedAt.Should().Be(newHappenedAt);
        result.Content.Should().Be("Updated content");

        // Verify in database by querying fresh
        var fromDb = await _service.GetVestigiumByIdAsync(
            vestigium.Id,
            TestContext.CancellationToken
        );
        fromDb.Should().NotBeNull();
        fromDb!.HappenedAt.Should().Be(newHappenedAt);
        fromDb.Content.Should().Be("Updated content");
    }

    [TestMethod]
    public async Task UpdateVestigiumAsync_ShouldNotChangeOtherProperties()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var uri = new Uri("https://example.com/original");
        var vestigium = VestigiumBuilder
            .Simple("Original content")
            .WithPersona(persona)
            .WithLocus(locus)
            .WithUri(uri)
            .WithOrigo(Origo.Domesticus)
            .AsPublished()
            .Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var originalPersonaId = vestigium.PersonaId;
        var originalLocusId = vestigium.LocusId;
        var originalOrigo = vestigium.Origo;
        var originalPublishedAt = vestigium.PublishedAt;

        // Act
        await _service.UpdateVestigiumAsync(
            vestigium.Id,
            DateTime.UtcNow,
            "Updated content",
            TestContext.CancellationToken
        );

        // Assert by querying fresh
        var fromDb = await _service.GetVestigiumByIdAsync(
            vestigium.Id,
            TestContext.CancellationToken
        );
        fromDb!.PersonaId.Should().Be(originalPersonaId);
        fromDb.LocusId.Should().Be(originalLocusId);
        fromDb.Origo.Should().Be(originalOrigo);
        fromDb.Uri.Should().Be(uri);
        fromDb.PublishedAt.Should().Be(originalPublishedAt);
    }

    [TestMethod]
    public async Task UpdateVestigiumAsync_ShouldBeNull_WhenVestigiumNotFound()
    {
        // Act
        var act = await _service.UpdateVestigiumAsync(
            "nonexistent",
            DateTime.UtcNow,
            "content",
            TestContext.CancellationToken
        );

        // Assert
        act.Should().BeNull();
    }

    [TestMethod]
    public async Task UpdateVestigiumAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var vestigium = VestigiumBuilder.Simple().WithPersona(persona).WithLocus(locus).Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () =>
            await _service.UpdateVestigiumAsync(
                vestigium.Id,
                DateTime.UtcNow,
                "content",
                cts.Token
            );

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region PublishVestigiumAsync Tests

    [TestMethod]
    public async Task PublishVestigiumAsync_ShouldSetPublishedAt()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var vestigium = VestigiumBuilder
            .Simple("Draft event")
            .WithPersona(persona)
            .WithLocus(locus)
            .AsDraft()
            .Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        vestigium.PublishedAt.Should().BeNull();
        vestigium.Status.Should().Be(Status.Latens);

        // Act
        var result = await _service.PublishVestigiumAsync(
            vestigium.Id,
            TestContext.CancellationToken
        );

        // Assert
        result.PublishedAt.Should().NotBeNull();
        result.PublishedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        result.Status.Should().Be(Status.Manifestus);

        // Verify in database by querying fresh
        var fromDb = await _service.GetVestigiumByIdAsync(
            vestigium.Id,
            TestContext.CancellationToken
        );
        fromDb!.PublishedAt.Should().NotBeNull();
        fromDb.Status.Should().Be(Status.Manifestus);
    }

    [TestMethod]
    public async Task PublishVestigiumAsync_ShouldUpdatePublishedAt_WhenAlreadyPublished()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var originalPublishedAt = DateTime.UtcNow.AddDays(-7);
        var vestigium = VestigiumBuilder
            .Simple("Published event")
            .WithPersona(persona)
            .WithLocus(locus)
            .WithPublishedAt(originalPublishedAt)
            .Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        // Act
        var result = await _service.PublishVestigiumAsync(
            vestigium.Id,
            TestContext.CancellationToken
        );

        // Assert
        result.PublishedAt.Should().NotBe(originalPublishedAt);
        result.PublishedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [TestMethod]
    public async Task PublishVestigiumAsync_ShouldThrow_WhenVestigiumNotFound()
    {
        // Act
        Func<Task> act = async () =>
            await _service.PublishVestigiumAsync("nonexistent", TestContext.CancellationToken);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Vestigium with ID 'nonexistent' not found.");
    }

    [TestMethod]
    public async Task PublishVestigiumAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var vestigium = VestigiumBuilder.Simple().WithPersona(persona).WithLocus(locus).Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () => await _service.PublishVestigiumAsync(vestigium.Id, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    #endregion

    #region TombstoneVestigiumAsync Tests

    [TestMethod]
    public async Task TombstoneVestigiumAsync_ShouldSetTombstonedAt()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var vestigium = VestigiumBuilder
            .Simple("Published event")
            .WithPersona(persona)
            .WithLocus(locus)
            .AsPublished()
            .Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        vestigium.TombstonedAt.Should().BeNull();
        vestigium.Status.Should().Be(Status.Manifestus);

        // Act
        var result = await _service.TombstoneVestigiumAsync(
            vestigium.Id,
            TestContext.CancellationToken
        );

        // Assert
        result.TombstonedAt.Should().NotBeNull();
        result.TombstonedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
        result.Status.Should().Be(Status.Deletus);

        // Verify in database by querying fresh
        var fromDb = await _service.GetVestigiumByIdAsync(
            vestigium.Id,
            TestContext.CancellationToken
        );
        fromDb!.TombstonedAt.Should().NotBeNull();
        fromDb.Status.Should().Be(Status.Deletus);
    }

    [TestMethod]
    public async Task TombstoneVestigiumAsync_ShouldUpdateTombstonedAt_WhenAlreadyTombstoned()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var originalTombstonedAt = DateTime.UtcNow.AddDays(-7);
        var vestigium = VestigiumBuilder
            .Simple("Deleted event")
            .WithPersona(persona)
            .WithLocus(locus)
            .AsPublished()
            .WithTombstonedAt(originalTombstonedAt)
            .Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        // Act
        var result = await _service.TombstoneVestigiumAsync(
            vestigium.Id,
            TestContext.CancellationToken
        );

        // Assert
        result.TombstonedAt.Should().NotBe(originalTombstonedAt);
        result.TombstonedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(2));
    }

    [TestMethod]
    public async Task TombstoneVestigiumAsync_ShouldTombstoneDraft()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var vestigium = VestigiumBuilder
            .Simple("Draft event")
            .WithPersona(persona)
            .WithLocus(locus)
            .AsDraft()
            .Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        // Act
        var result = await _service.TombstoneVestigiumAsync(
            vestigium.Id,
            TestContext.CancellationToken
        );

        // Assert
        result.Status.Should().Be(Status.Deletus);
        result.PublishedAt.Should().BeNull();
        result.TombstonedAt.Should().NotBeNull();
    }

    [TestMethod]
    public async Task TombstoneVestigiumAsync_ShouldThrow_WhenVestigiumNotFound()
    {
        // Act
        Func<Task> act = async () =>
            await _service.TombstoneVestigiumAsync("nonexistent", TestContext.CancellationToken);

        // Assert
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Vestigium with ID 'nonexistent' not found.");
    }

    [TestMethod]
    public async Task TombstoneVestigiumAsync_ShouldRespectCancellationToken()
    {
        // Arrange
        var persona = PersonaBuilder.Simple().Build();
        var locus = LocusBuilder.Simple().Build();
        var vestigium = VestigiumBuilder.Simple().WithPersona(persona).WithLocus(locus).Build();

        _dbContext.Personae.Add(persona);
        _dbContext.Loca.Add(locus);
        _dbContext.Vestigia.Add(vestigium);
        await _dbContext.SaveChangesAsync(TestContext.CancellationToken);

        var cts = new CancellationTokenSource();
        cts.Cancel();

        // Act
        Func<Task> act = async () =>
            await _service.TombstoneVestigiumAsync(vestigium.Id, cts.Token);

        // Assert
        await act.Should().ThrowAsync<OperationCanceledException>();
    }

    public TestContext TestContext { get; set; }

    #endregion
}
