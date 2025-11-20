// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Viae.Domain.Criteria;
using Viae.Domain.Services;
using Viae.Presentation.Mvc.ContentNegotiation;
using Viae.Presentation.Mvc.Extensions;
using Viae.Presentation.Mvc.ViewModels;

namespace Viae.Presentation.Mvc.Controllers;

/// <summary>
/// Controller for displaying Vestigium resources.
/// </summary>
[Route("vestigia")]
public class VestigiaController(ILocusService locSvc, IOptions<JsonOptions> jsonOpts)
    : ContentNegotiatingController(jsonOpts)
{
    /// <summary>
    /// List all vestigia with content negotiation support.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of vestigia in the requested format.</returns>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct) =>
        await NegotiateContent(
            await locSvc.GetVestigiaAsync(new VestigiumCriteriaBuilder().Build(), ct),
            n =>
                n.ForGeoJson(async vv =>
                        vv.ToGeoJsonFeatureCollection(
                            v => v.Locus.Coordinates,
                            v => v.Locus.CreateAttributesTable(l => Url.LocusPath(l))
                        )
                    )
                    .ForActivityPub(async vv =>
                        VestigiaActivity.FromVestigia(
                            vv,
                            v => Url.VestigiumIdUrl(v, Request),
                            v => Url.VestigiumUrl(v, Request),
                            l => Url.LocusIdUrl(l, Request),
                            l => Url.LocusUrl(l, Request)
                        )
                    )
                    .ForHtml(
                        "Index",
                        async vv => VestigiaHtml.FromVestigia(vv, Url.VestigiumPath, Url.LocusPath)
                    )
        );

    /// <summary>
    /// Display a single vestigium with canonical URL handling.
    /// </summary>
    /// <param name="id">The vestigium ID.</param>
    /// <param name="slug">The optional slug for SEO-friendly URLs.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The locus in the requested format.</returns>
    [HttpGet("{id}/{**catchAll}", Name = PathExtensions.VestigiumRoute)]
    [SuppressMessage("catchAll", "IDE0060")]
    public async Task<IActionResult> Show(string id, string? catchAll, CancellationToken ct) =>
        await NegotiateContent(
            await locSvc.GetVestigiumByIdAsync(id, ct),
            Url.VestigiumPath,
            n =>
                n.ForGeoJson(async vtg =>
                        (
                            await locSvc.GetLocaDistancesAsync(
                                vtg.Locus.GetNearbyLocusCriteria(),
                                vtg.Locus,
                                ct
                            )
                        ).ToGeoJsonFeatureCollection(
                            v => v.Locus.Coordinates,
                            v => v.Locus.CreateAttributesTable(Url.LocusPath, vtg.Locus)
                        )
                    )
                    .ForActivityPub(async vtg =>
                        VestigiumActivity.FromVestigium(
                            vtg,
                            v => Url.VestigiumIdUrl(v, Request),
                            v => Url.VestigiumUrl(v, Request),
                            l => Url.LocusIdUrl(l, Request),
                            l => Url.LocusUrl(l, Request)
                        )
                    )
                    .ForHtml(
                        "Show",
                        async vtg =>
                            VestigiumHtml.FromVestigium(vtg, Url.VestigiumPath, Url.LocusPath)
                    )
        );
}
