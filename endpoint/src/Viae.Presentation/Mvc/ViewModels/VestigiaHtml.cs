// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Presentation.Mvc.ViewModels;

public class VestigiaHtml
{
    public required IEnumerable<VestigiumHtml> Vestigia { get; set; }
    public bool HasVestigia => Vestigia.Any();

    public static VestigiaHtml FromVestigia(
        IEnumerable<Vestigium> vestigia,
        Func<Vestigium, string> pathFunc,
        Func<Locus, string> locusPathFunc
    ) =>
        new()
        {
            Vestigia = vestigia.Select(v =>
                VestigiumHtml.FromVestigium(v, pathFunc, locusPathFunc)
            ),
        };
}
