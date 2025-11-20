// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Viae.Presentation.Mvc.ContentNegotiation;

namespace Viae.Presentation.Mvc.Controllers;

[Route("")]
public class HomeController(IOptions<JsonOptions> jsonOpts) : ContentNegotiatingController(jsonOpts)
{
    // GET: HomeController
    public ActionResult Index()
    {
        return View();
    }
}
