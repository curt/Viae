// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Tests.Helpers;

/// <summary>
/// Provides custom assertion helpers for DateTime comparisons to handle
/// precision issues and make tests more readable and maintainable.
/// </summary>
public static class DateTimeAssertions
{
    /// <summary>
    /// Asserts that a DateTime is close to the expected value within a tolerance.
    /// Default tolerance is 1 second to account for test execution time.
    /// </summary>
    public static void ShouldBeCloseToUtcNow(this DateTime? actual, TimeSpan? tolerance = null)
    {
        actual.Should().NotBeNull();
        actual.Should().BeCloseTo(DateTime.UtcNow, tolerance ?? TimeSpan.FromSeconds(1));
    }

    /// <summary>
    /// Asserts that a DateTime is close to another DateTime within a tolerance.
    /// Default tolerance is 1 second.
    /// </summary>
    public static void ShouldBeCloseTo(
        this DateTime? actual,
        DateTime expected,
        TimeSpan? tolerance = null
    )
    {
        actual.Should().NotBeNull();
        actual.Should().BeCloseTo(expected, tolerance ?? TimeSpan.FromSeconds(1));
    }
}
