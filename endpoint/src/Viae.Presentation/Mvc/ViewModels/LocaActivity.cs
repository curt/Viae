// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.ActivityPub;
using Viae.Domain.Models;
using Collection = Viae.ActivityPub.Collection;

namespace Viae.Presentation.Mvc.ViewModels;

public class LocaActivity
{
    public static ActivityObject FromLoca(
        IEnumerable<Locus> loca,
        Func<Locus, Uri> uriIdBuilder,
        Func<Locus, Uri> uriBuilder
    ) =>
        new Collection
        {
            Items =
            [
                .. loca.Select(l => new LinkOr<ActivityObject>(
                    LocusActivity.FromLocus(l, uriIdBuilder, uriBuilder)
                )),
            ],
        };
}
