// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.ActivityPub;
using Viae.Domain.Models;

namespace Viae.Presentation.Mvc.ViewModels;

public class VestigiaActivity
{
    public static ActivityObject FromVestigia(
        IEnumerable<Vestigium> vestigia,
        Func<Vestigium, Uri> vestigiumIdBuilder,
        Func<Vestigium, Uri> vestigiumUrlBuilder,
        Func<Locus, Uri> locusIdBuilder,
        Func<Locus, Uri> locusUrlBuilder
    ) =>
        new Collection
        {
            Items =
            [
                .. vestigia.Select(v => new LinkOr<ActivityObject>(
                    VestigiumActivity.FromVestigium(
                        v,
                        vestigiumIdBuilder,
                        vestigiumUrlBuilder,
                        locusIdBuilder,
                        locusUrlBuilder
                    )
                )),
            ],
        };
}
