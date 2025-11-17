// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Viae.Persistence.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddViaePersistence(
        this IServiceCollection services,
        IConfiguration configuration
    )
    {
        // Configure and register URI generator service
        services.Configure<UriGeneratorOptions>(options =>
        {
            var baseUri = configuration["BaseUri"] ?? "https://localhost";
            options.BaseUri = new Uri(baseUri);
        });

        services.AddSingleton<IUriGeneratorService, UriGeneratorService>();

        return services;
    }
}
