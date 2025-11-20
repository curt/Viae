// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.ActivityPub;
using Viae.Domain.Models;
using Viae.Presentation.Renderers;

namespace Viae.Presentation.Mvc.ViewModels;

public class VestigiumActivity
{
    public static ActivityObject FromVestigium(
        Vestigium vestigium,
        Func<Vestigium, Uri> vestigiumIdBuilder,
        Func<Vestigium, Uri> vestigiumUrlBuilder,
        Func<Locus, Uri> locusIdBuilder,
        Func<Locus, Uri> locusUrlBuilder
    ) =>
        new Note()
        {
            Content = MarkdownRenderer.Instance.RenderToHtml(vestigium.Content).Trim(),
            Id = vestigiumIdBuilder(vestigium),
            Url = new LinkOr<Uri>(vestigiumUrlBuilder(vestigium)),
            Location = new OneOrMany<LinkOr<ActivityObject>>(
                new LinkOr<ActivityObject>(
                    LocusActivity.FromLocus(vestigium.Locus, locusIdBuilder, locusUrlBuilder)
                )
            ),
            StartedAt = vestigium.HappenedAt,
            EndedAt = vestigium.HappenedAt,
        };
}
