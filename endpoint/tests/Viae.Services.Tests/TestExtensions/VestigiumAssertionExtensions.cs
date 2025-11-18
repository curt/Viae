// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using FluentAssertions;
using Viae.Domain.Models;

namespace Viae.Services.Tests.TestExtensions;

/// <summary>
/// Extension methods for asserting on Vestigium collections in tests.
/// </summary>
public static class VestigiumAssertionExtensions
{
    /// <summary>
    /// Asserts that the collection contains exactly one vestigium with the specified content.
    /// </summary>
    public static void ShouldContainSingleVestigium(
        this IReadOnlyList<Vestigium> vestigia,
        string expectedContent
    )
    {
        vestigia.Should().NotBeNull();
        vestigia.Should().HaveCount(1);
        vestigia[0].Content.Should().Be(expectedContent);
    }

    /// <summary>
    /// Asserts that the collection contains vestigia with the specified contents.
    /// </summary>
    public static void ShouldContainVestigia(
        this IReadOnlyList<Vestigium> vestigia,
        params string[] expectedContents
    )
    {
        vestigia.Should().NotBeNull();
        vestigia.Should().HaveCount(expectedContents.Length);

        foreach (var content in expectedContents)
        {
            vestigia
                .Should()
                .Contain(
                    v => v.Content == content,
                    $"because the collection should contain a vestigium with content '{content}'"
                );
        }
    }

    /// <summary>
    /// Asserts that the collection is empty.
    /// </summary>
    public static void ShouldBeEmpty(this IReadOnlyList<Vestigium> vestigia)
    {
        vestigia.Should().NotBeNull();
        vestigia.Should().BeEmpty();
    }

    /// <summary>
    /// Asserts that all vestigia in the collection belong to the specified persona.
    /// </summary>
    public static void ShouldAllBelongToPersona(
        this IReadOnlyList<Vestigium> vestigia,
        string personaId
    )
    {
        vestigia.Should().NotBeNull();
        vestigia.Should().NotBeEmpty();
        vestigia
            .Should()
            .OnlyContain(
                v => v.PersonaId == personaId,
                $"because all vestigia should belong to persona '{personaId}'"
            );
    }

    /// <summary>
    /// Asserts that all vestigia in the collection are at the specified locus.
    /// </summary>
    public static void ShouldAllBeAtLocus(this IReadOnlyList<Vestigium> vestigia, string locusId)
    {
        vestigia.Should().NotBeNull();
        vestigia.Should().NotBeEmpty();
        vestigia
            .Should()
            .OnlyContain(
                v => v.LocusId == locusId,
                $"because all vestigia should be at locus '{locusId}'"
            );
    }

    /// <summary>
    /// Asserts that all vestigia in the collection have the specified status.
    /// </summary>
    public static void ShouldAllHaveStatus(this IReadOnlyList<Vestigium> vestigia, Status status)
    {
        vestigia.Should().NotBeNull();
        vestigia.Should().NotBeEmpty();
        vestigia
            .Should()
            .OnlyContain(
                v => v.Status == status,
                $"because all vestigia should have status '{status}'"
            );
    }

    /// <summary>
    /// Asserts that all vestigia in the collection have the specified origin.
    /// </summary>
    public static void ShouldAllHaveOrigo(this IReadOnlyList<Vestigium> vestigia, Origo origo)
    {
        vestigia.Should().NotBeNull();
        vestigia.Should().NotBeEmpty();
        vestigia
            .Should()
            .OnlyContain(
                v => v.Origo == origo,
                $"because all vestigia should have origo '{origo}'"
            );
    }

    /// <summary>
    /// Asserts that the vestigia are ordered by happened date in ascending order.
    /// </summary>
    public static void ShouldBeOrderedByHappenedAtAscending(this IReadOnlyList<Vestigium> vestigia)
    {
        vestigia.Should().NotBeNull();
        if (vestigia.Count <= 1)
        {
            return;
        }

        vestigia.Should().BeInAscendingOrder(v => v.HappenedAt);
    }

    /// <summary>
    /// Asserts that the vestigia are ordered by happened date in descending order.
    /// </summary>
    public static void ShouldBeOrderedByHappenedAtDescending(this IReadOnlyList<Vestigium> vestigia)
    {
        vestigia.Should().NotBeNull();
        if (vestigia.Count <= 1)
        {
            return;
        }

        vestigia.Should().BeInDescendingOrder(v => v.HappenedAt);
    }

    /// <summary>
    /// Asserts that the vestigia are ordered by persona ID in ascending order.
    /// </summary>
    public static void ShouldBeOrderedByPersonaIdAscending(this IReadOnlyList<Vestigium> vestigia)
    {
        vestigia.Should().NotBeNull();
        if (vestigia.Count <= 1)
        {
            return;
        }

        vestigia.Should().BeInAscendingOrder(v => v.PersonaId);
    }

    /// <summary>
    /// Asserts that the vestigia are ordered by persona ID in descending order.
    /// </summary>
    public static void ShouldBeOrderedByPersonaIdDescending(this IReadOnlyList<Vestigium> vestigia)
    {
        vestigia.Should().NotBeNull();
        if (vestigia.Count <= 1)
        {
            return;
        }

        vestigia.Should().BeInDescendingOrder(v => v.PersonaId);
    }

    /// <summary>
    /// Asserts that the vestigia are ordered by locus ID in ascending order.
    /// </summary>
    public static void ShouldBeOrderedByLocusIdAscending(this IReadOnlyList<Vestigium> vestigia)
    {
        vestigia.Should().NotBeNull();
        if (vestigia.Count <= 1)
        {
            return;
        }

        vestigia.Should().BeInAscendingOrder(v => v.LocusId);
    }

    /// <summary>
    /// Asserts that the vestigia are ordered by locus ID in descending order.
    /// </summary>
    public static void ShouldBeOrderedByLocusIdDescending(this IReadOnlyList<Vestigium> vestigia)
    {
        vestigia.Should().NotBeNull();
        if (vestigia.Count <= 1)
        {
            return;
        }

        vestigia.Should().BeInDescendingOrder(v => v.LocusId);
    }

    /// <summary>
    /// Asserts that the vestigia collection has the exact count and contains vestigia with the specified contents in order.
    /// </summary>
    public static void ShouldContainInOrder(
        this IReadOnlyList<Vestigium> vestigia,
        params string[] expectedContentsInOrder
    )
    {
        vestigia.Should().NotBeNull();
        vestigia.Should().HaveCount(expectedContentsInOrder.Length);

        for (int i = 0; i < expectedContentsInOrder.Length; i++)
        {
            vestigia[i]
                .Content.Should()
                .Be(
                    expectedContentsInOrder[i],
                    $"because the vestigium at index {i} should have content '{expectedContentsInOrder[i]}'"
                );
        }
    }

    /// <summary>
    /// Asserts that all vestigia happened within the specified date range.
    /// </summary>
    public static void ShouldAllHaveHappenedBetween(
        this IReadOnlyList<Vestigium> vestigia,
        DateTime start,
        DateTime end
    )
    {
        vestigia.Should().NotBeNull();
        vestigia.Should().NotBeEmpty();
        vestigia
            .Should()
            .OnlyContain(
                v => v.HappenedAt >= start && v.HappenedAt <= end,
                $"because all vestigia should have happened between {start} and {end}"
            );
    }
}
