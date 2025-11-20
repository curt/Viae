// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Presentation.Mvc.ViewModels;

public sealed class LocusVestigiumHtml
{
    public required Vestigium Vestigium { get; init; }
    public required string Path { get; init; }

    public static LocusVestigiumHtml FromVestigium(
        Vestigium vestigium,
        Func<Vestigium, string> pathBuilder
    ) => new() { Path = pathBuilder(vestigium), Vestigium = vestigium };
}
