// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.ActivityPub;
using Viae.Domain.Models;

namespace Viae.Presentation.Mvc.ViewModels;

public class LocusActivity
{
    public static ActivityObject FromLocus(
        Locus locus,
        Func<Locus, Uri> uriIdBuilder,
        Func<Locus, Uri> uriBuilder
    ) =>
        new Place()
        {
            Name = locus.Name,
            Latitude = locus.Latitude,
            Longitude = locus.Longitude,
            Id = uriIdBuilder(locus),
            Url = new LinkOr<Uri>(uriBuilder(locus)),
        };
}
