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
/// Controller for displaying Locus resources.
/// </summary>
[Route("loca")]
public class LocaController(ILocusService locSvc, IOptions<JsonOptions> jsonOpts)
    : ContentNegotiatingController(jsonOpts)
{
    /// <summary>
    /// List all loca with content negotiation support.
    /// </summary>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of loca in the requested format.</returns>
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var criteria = new LocusCriteriaBuilder().Build();
        var loca = await locSvc.GetLocaAsync(criteria, ct);

        return await NegotiateContent(
            loca,
            n =>
                n.ForGeoJson(async ll =>
                        ll.ToGeoJsonFeatureCollection(
                            l => l.Coordinates,
                            l => l.CreateAttributesTable(Url.LocusPath)
                        )
                    )
                    .ForActivityPub(async ll =>
                        LocaActivity.FromLoca(
                            ll,
                            l => Url.LocusIdUrl(l, Request),
                            l => Url.LocusUrl(l, Request)
                        )
                    )
                    .ForHtml("Index", async ll => LocaHtml.FromLoca(ll, Url.LocusPath))
        );
    }

    /// <summary>
    /// Display a single locus with canonical URL handling.
    /// </summary>
    /// <param name="id">The locus ID.</param>
    /// <param name="slug">The optional slug for SEO-friendly URLs.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>The locus in the requested format.</returns>
    [HttpGet("{id}/{**catchAll}", Name = PathExtensions.LocusRoute)]
    [SuppressMessage("catchAll", "IDE0060")]
    public async Task<IActionResult> Show(string id, string? catchAll, CancellationToken ct)
    {
        var locus = await locSvc.GetLocusByIdAsync(id, ct);

        return await NegotiateContent(
            locus,
            Url.LocusPath,
            n =>
                n.ForGeoJson(async loc =>
                        (
                            await locSvc.GetLocaDistancesAsync(
                                loc.GetNearbyLocusCriteria(),
                                loc,
                                ct
                            )
                        ).ToGeoJsonFeatureCollection(
                            ld => ld.Locus.Coordinates,
                            ld => ld.Locus.CreateAttributesTable(Url.LocusPath, loc)
                        )
                    )
                    .ForActivityPub(async loc =>
                        LocusActivity.FromLocus(
                            loc,
                            l => Url.LocusIdUrl(l, Request),
                            l => Url.LocusUrl(l, Request)
                        )
                    )
                    .ForHtml(
                        "Show",
                        async loc =>
                            LocusHtml.FromLocus(
                                loc,
                                await locSvc.GetLocaDistancesAsync(
                                    loc.GetNearbyLocusCriteria(),
                                    loc,
                                    ct
                                ),
                                Url.LocusPath,
                                Url.VestigiumPath
                            )
                    )
        );
    }
}
