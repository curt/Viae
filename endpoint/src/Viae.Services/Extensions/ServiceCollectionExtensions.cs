// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.Extensions.DependencyInjection;
using Viae.Domain.Services;

namespace Viae.Services.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViaeServices(this IServiceCollection services)
    {
        services.AddScoped<IMissioService, MissioService>();
        services.AddScoped<ILocusService, LocusService>();

        return services;
    }
}
