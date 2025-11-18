// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Presentation.Mvc.ViewModels;

public class LocaHtml
{
    public required IEnumerable<LocusHtml> Loca { get; set; }
    public bool HasLoca => Loca.Any();

    public static LocaHtml FromLoca(IEnumerable<Locus> loca, Func<Locus, string> pathFunc) =>
        new() { Loca = loca.Select(l => LocusHtml.FromLocus(l, pathFunc)) };
}
