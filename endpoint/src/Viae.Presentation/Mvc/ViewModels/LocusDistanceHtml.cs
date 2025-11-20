// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Presentation.Mvc.ViewModels;

public class LocusDistanceHtml
{
    public required string Name { get; init; }
    public required string Path { get; init; }

    /// <summary>
    /// Distance in kilometers for display.
    /// </summary>
    public required double Distance { get; init; }

    public static LocusDistanceHtml FromLocusDistance(
        LocusDistance locusDistance,
        Func<Locus, string> pathFunc
    ) =>
        new()
        {
            Name = locusDistance.Locus.Name,
            Path = pathFunc(locusDistance.Locus),
            Distance = Math.Round(locusDistance.Distance / 1000, 1),
        };
}
