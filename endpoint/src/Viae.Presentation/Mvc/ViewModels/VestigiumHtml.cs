// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Presentation.Mvc.ViewModels;

public class VestigiumHtml
{
    public required string LocusName { get; init; }
    public required DateTime HappenedAt { get; init; }
    public required string Path { get; init; }
    public required string LocusPath { get; init; }
    public string? Content { get; set; }

    public static VestigiumHtml FromVestigium(
        Vestigium vestigium,
        Func<Vestigium, string> vestigiumPath,
        Func<Locus, string> locusPath
    ) =>
        new()
        {
            LocusName = vestigium.Locus.Name,
            HappenedAt = vestigium.HappenedAt,
            Path = vestigiumPath(vestigium),
            LocusPath = locusPath(vestigium.Locus),
            Content = vestigium.Content,
        };
}
