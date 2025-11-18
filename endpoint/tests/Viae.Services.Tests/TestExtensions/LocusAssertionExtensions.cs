// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using FluentAssertions;
using Viae.Domain.Models;

namespace Viae.Services.Tests.TestExtensions;

/// <summary>
/// Extension methods for asserting on Locus collections in tests.
/// </summary>
public static class LocusAssertionExtensions
{
    /// <summary>
    /// Asserts that the collection contains exactly one locus with the specified name.
    /// </summary>
    public static void ShouldContainSingleLocus(this IReadOnlyList<Locus> loca, string expectedName)
    {
        loca.Should().NotBeNull();
        loca.Should().HaveCount(1);
        loca[0].Name.Should().Be(expectedName);
    }

    /// <summary>
    /// Asserts that the collection contains loca with the specified names.
    /// </summary>
    public static void ShouldContainLoca(
        this IReadOnlyList<Locus> loca,
        params string[] expectedNames
    )
    {
        loca.Should().NotBeNull();
        loca.Should().HaveCount(expectedNames.Length);

        foreach (var name in expectedNames)
        {
            loca.Should()
                .Contain(
                    l => l.Name == name,
                    $"because the collection should contain a locus named '{name}'"
                );
        }
    }

    /// <summary>
    /// Asserts that the collection is empty.
    /// </summary>
    public static void ShouldBeEmpty(this IReadOnlyList<Locus> loca)
    {
        loca.Should().NotBeNull();
        loca.Should().BeEmpty();
    }

    /// <summary>
    /// Asserts that all loca in the collection have the specified thema.
    /// </summary>
    public static void ShouldAllHaveThema(this IReadOnlyList<Locus> loca, string themaName)
    {
        loca.Should().NotBeNull();
        loca.Should().NotBeEmpty();
        loca.Should()
            .OnlyContain(
                l => l.Themata.Any(t => t.Name == themaName),
                $"because all loca should have thema '{themaName}'"
            );
    }

    /// <summary>
    /// Asserts that the loca are ordered by name in ascending order.
    /// </summary>
    public static void ShouldBeOrderedByNameAscending(this IReadOnlyList<Locus> loca)
    {
        loca.Should().NotBeNull();
        if (loca.Count <= 1)
        {
            return;
        }

        loca.Should().BeInAscendingOrder(l => l.Name);
    }

    /// <summary>
    /// Asserts that the loca are ordered by name in descending order.
    /// </summary>
    public static void ShouldBeOrderedByNameDescending(this IReadOnlyList<Locus> loca)
    {
        loca.Should().NotBeNull();
        if (loca.Count <= 1)
        {
            return;
        }

        loca.Should().BeInDescendingOrder(l => l.Name);
    }

    /// <summary>
    /// Asserts that the loca are ordered by created date in ascending order.
    /// </summary>
    public static void ShouldBeOrderedByCreatedAtAscending(this IReadOnlyList<Locus> loca)
    {
        loca.Should().NotBeNull();
        if (loca.Count <= 1)
        {
            return;
        }

        loca.Should().BeInAscendingOrder(l => l.CreatedAt);
    }

    /// <summary>
    /// Asserts that the loca are ordered by created date in descending order.
    /// </summary>
    public static void ShouldBeOrderedByCreatedAtDescending(this IReadOnlyList<Locus> loca)
    {
        loca.Should().NotBeNull();
        if (loca.Count <= 1)
        {
            return;
        }

        loca.Should().BeInDescendingOrder(l => l.CreatedAt);
    }

    /// <summary>
    /// Asserts that the loca collection has the exact count and contains loca with the specified names in order.
    /// </summary>
    public static void ShouldContainInOrder(
        this IReadOnlyList<Locus> loca,
        params string[] expectedNamesInOrder
    )
    {
        loca.Should().NotBeNull();
        loca.Should().HaveCount(expectedNamesInOrder.Length);

        for (int i = 0; i < expectedNamesInOrder.Length; i++)
        {
            loca[i]
                .Name.Should()
                .Be(
                    expectedNamesInOrder[i],
                    $"because the locus at index {i} should be '{expectedNamesInOrder[i]}'"
                );
        }
    }

    /// <summary>
    /// Asserts that the loca collection contains at least the specified count.
    /// </summary>
    public static void ShouldHaveAtLeast(this IReadOnlyList<Locus> loca, int minimumCount)
    {
        loca.Should().NotBeNull();
        loca.Count.Should().BeGreaterOrEqualTo(minimumCount);
    }

    /// <summary>
    /// Asserts that each locus in the collection has at least the specified number of themata.
    /// </summary>
    public static void ShouldAllHaveMinimumThemata(
        this IReadOnlyList<Locus> loca,
        int minimumThemataCount
    )
    {
        loca.Should().NotBeNull();
        loca.Should().NotBeEmpty();
        loca.Should()
            .OnlyContain(
                l => l.Themata.Count >= minimumThemataCount,
                $"because all loca should have at least {minimumThemataCount} themata"
            );
    }
}
