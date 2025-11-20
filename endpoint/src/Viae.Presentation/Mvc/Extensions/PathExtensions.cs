// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Globalization;
using Microsoft.AspNetCore.Mvc;
using Viae.Domain.Models;

namespace Viae.Presentation.Mvc.Extensions;

public static class PathExtensions
{
    public const string LocusRoute = "LocusRoute";
    public const string VestigiumRoute = "VestigiumRoute";

    public static string LocusPath(this IUrlHelper url, Locus locus) =>
        url.RouteUrl(LocusRoute, new { id = locus.Id, catchAll = locus.Slug })!;

    public static string LocusIdPath(this IUrlHelper url, Locus locus) =>
        url.RouteUrl(LocusRoute, new { id = locus.Id })!;

    public static Uri LocusUrl(this IUrlHelper url, Locus locus, HttpRequest request) =>
        new(GetBaseUrl(request), url.LocusPath(locus));

    public static Uri LocusIdUrl(this IUrlHelper url, Locus locus, HttpRequest request) =>
        new(GetBaseUrl(request), url.LocusIdPath(locus));

    public static string VestigiumPath(this IUrlHelper url, Vestigium vestigium) =>
        url.RouteUrl(
            VestigiumRoute,
            new
            {
                id = vestigium.Id,
                catchAll = $"{vestigium.HappenedAt.ToString("yyyy/MM/dd", CultureInfo.InvariantCulture)}{(vestigium.Locus.Slug != null ? $"/{vestigium.Locus.Slug}" : "")}",
            }
        )!;

    public static string VestigiumIdPath(this IUrlHelper url, Vestigium vestigium) =>
        url.RouteUrl(VestigiumRoute, new { id = vestigium.Id })!;

    public static Uri VestigiumUrl(this IUrlHelper url, Vestigium vestigium, HttpRequest request) =>
        new(GetBaseUrl(request), url.VestigiumPath(vestigium));

    public static Uri VestigiumIdUrl(
        this IUrlHelper url,
        Vestigium vestigium,
        HttpRequest request
    ) => new(GetBaseUrl(request), url.VestigiumIdPath(vestigium));

    private static Uri GetBaseUrl(HttpRequest request) =>
        new UriBuilder(request.Scheme, request.Host.Host, request.Host.Port ?? -1).Uri;
}
