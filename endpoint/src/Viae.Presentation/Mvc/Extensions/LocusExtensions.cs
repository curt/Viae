// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using NetTopologySuite.Features;
using Viae.Domain.Criteria;
using Viae.Domain.Models;

namespace Viae.Presentation.Mvc.Extensions;

public static class LocusExtensions
{
    public static AttributesTable CreateAttributesTable(
        this Locus locus,
        Func<Locus, string> path,
        Locus? primary = null
    )
    {
        AttributesTable table = new()
        {
            { "id", locus.Id },
            { "name", locus.Name },
            { "slug", locus.Slug },
            { "path", path(locus) },
        };

        if (locus.Id == primary?.Id)
        {
            table.Add("center", true);
        }

        return table;
    }

    public static LocusCriteria GetNearbyLocusCriteria(this Locus locus) =>
        new LocusCriteriaBuilder()
            .Near(locus.Coordinates, 20000) // TODO: Fix magic number
            .SortByDistance()
            .Page(1, 10) // TODO: Fix magic number
            .Build();
}
