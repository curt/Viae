// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Presentation.Mvc.ViewModels;

public class LocusHtml
{
    public required string Name { get; init; }
    public required string Path { get; init; }
    public IEnumerable<LocusDistanceHtml>? NearbyLoca { get; set; }
    public IEnumerable<VestigiumHtml>? Vestigia { get; set; }
    public string? Content { get; set; }

    public static LocusHtml FromLocus(
        Locus locus,
        IEnumerable<LocusDistance> nearby,
        Func<Locus, string> locusPath,
        Func<Vestigium, string> vestigiumPath
    )
    {
        return new()
        {
            Name = locus.Name,
            Path = locusPath(locus),
            NearbyLoca = nearby
                .Where(n => !n.IsReferenceLocus)
                .Select(n => LocusDistanceHtml.FromLocusDistance(n, locusPath)),
            Vestigia = locus.Vestigia.Select(v =>
                VestigiumHtml.FromVestigium(v, vestigiumPath, locusPath)
            ),
        };
    }

    public static LocusHtml FromLocus(Locus locus, Func<Locus, string> locusPath)
    {
        return new()
        {
            Name = locus.Name,
            Path = locusPath(locus),
            Content = locus.Content,
        };
    }
}
