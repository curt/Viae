// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json.Serialization;
using Fluid;
using Fluid.MvcViewEngine;
using Hangfire;
using Hangfire.PostgreSql;
using Markdig;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Viae.ActivityPub;
using Viae.Domain.Models;
using Viae.Fluid.Markdown.Mvc;
using Viae.Persistence;
using Viae.Persistence.Extensions;
using Viae.Presentation.Converters;
using Viae.Services.Extensions;

var builder = WebApplication.CreateBuilder(args);
var env = builder.Environment;

// Configure and validate PostgreSQL settings
var postgresConfig =
    builder.Configuration.GetSection(PostgresConfiguration.SectionName).Get<PostgresConfiguration>()
    ?? new PostgresConfiguration();

var validator = new PostgresConfigurationValidator();
var validationResult = validator.Validate(postgresConfig);
if (!validationResult.IsValid)
{
    var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
    throw new InvalidOperationException($"PostgreSQL configuration is invalid: {errors}");
}

var connectionString = postgresConfig.BuildConnectionString();

// Add database context
builder.Services.AddDbContextFactory<ViaeDbContext>(options =>
    options.UseNpgsql(
        connectionString,
        npgsqlOptions =>
        {
            npgsqlOptions.UseNetTopologySuite();
            npgsqlOptions.MapEnum<Origo>();
        }
    )
);

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy
            .WithOrigins(
                builder.Configuration.GetValue<string>("Cors:AllowedOrigins")
                    ?? "http://localhost:5173"
            )
            .AllowCredentials() // Required for cookies
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder
    .Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        // Register the Point JSON converter
        options.JsonSerializerOptions.Converters.Add(new PointJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new OsmTypeJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new OsmIdJsonConverter());
        options.JsonSerializerOptions.Converters.Add(new ActivityObjectJsonConverter());

        options.JsonSerializerOptions.NumberHandling =
            JsonNumberHandling.AllowNamedFloatingPointLiterals;

        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    })
    .AddFluid();

// The configure step comes after adding Fluid.
builder.Services.Configure<FluidMvcViewOptions>(options =>
{
    var fileProvider = new PhysicalFileProvider(Path.Combine(env.ContentRootPath, "Mvc", "Views"));
    options.TemplateOptions.FileProvider = fileProvider;
    options.ViewsFileProvider = fileProvider;
    options.PartialsFileProvider = fileProvider;

    var strategy = new UnsafeMemberAccessStrategy();
    options.TemplateOptions.MemberAccessStrategy = strategy;

    // Add Markdown extensions
    options.AddFluidMarkdownFilters(cfg =>
        cfg.ConfigurePipeline = b => b.UseAdvancedExtensions().UsePipeTables()
    );
});

// Add persistence services
builder.Services.AddViaePersistence(builder.Configuration);

// Add other services
builder.Services.AddViaeServices();

// Add Hangfire services
builder.Services.AddHangfire(configuration =>
    configuration
        .SetDataCompatibilityLevel(CompatibilityLevel.Version_180)
        .UseSimpleAssemblyNameTypeSerializer()
        .UseRecommendedSerializerSettings()
        .UsePostgreSqlStorage(options => options.UseNpgsqlConnection(connectionString))
);

// Add the processing server as IHostedService
var defaultQueues = new[] { "critical", "default", "low" };
builder.Services.AddHangfireServer(options =>
{
    options.WorkerCount = builder.Configuration.GetValue("Hangfire:WorkerCount", 5);
    options.Queues =
        builder.Configuration.GetSection("Hangfire:Queues").Get<string[]>() ?? defaultQueues;
});

var app = builder.Build();

// Enable Hangfire Dashboard
var dashboardPath = builder.Configuration.GetValue("Hangfire:DashboardPath", "/hangfire");
app.UseHangfireDashboard(
    dashboardPath,
    new DashboardOptions
    {
        // In development, allow anonymous access. In production, add authorization.
        Authorization = app.Environment.IsDevelopment()
            ? Array.Empty<Hangfire.Dashboard.IDashboardAuthorizationFilter>()
            : [new Hangfire.Dashboard.LocalRequestsOnlyAuthorizationFilter()],
    }
);

app.UseDefaultFiles();

app.UseStaticFiles(
    new StaticFileOptions()
    {
        FileProvider = new PhysicalFileProvider(
            Path.Combine(env.ContentRootPath, "Mvc", "wwwroot")
        ),
    }
);

// Enable CORS (must be before authentication/authorization)
app.UseCors();

// Add authentication and authorization middleware
app.UseAuthentication();
app.UseAuthorization();

// Map controllers
app.MapControllers();

// Apply database migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContextFactory = scope.ServiceProvider.GetRequiredService<
        IDbContextFactory<ViaeDbContext>
    >();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    MigrationLog.ApplyingMigrations(logger);

    try
    {
        using var dbContext = dbContextFactory.CreateDbContext();
        dbContext.Database.Migrate();
        MigrationLog.MigrationsAppliedSuccessfully(logger);
    }
    catch (Exception ex)
    {
        MigrationLog.MigrationError(logger, ex);
        throw; // Fail fast if migrations fail
    }
}

app.Run();

// High-performance logging using LoggerMessage source generator
internal static partial class MigrationLog
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Applying database migrations...")]
    internal static partial void ApplyingMigrations(ILogger logger);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Database migrations applied successfully"
    )]
    internal static partial void MigrationsAppliedSuccessfully(ILogger logger);

    [LoggerMessage(Level = LogLevel.Error, Message = "Error applying database migrations")]
    internal static partial void MigrationError(ILogger logger, Exception ex);
}
