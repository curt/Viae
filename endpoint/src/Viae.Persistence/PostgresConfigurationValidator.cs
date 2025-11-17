// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using FluentValidation;

namespace Viae.Persistence;

/// <summary>
/// Validator for PostgresConfiguration.
/// </summary>
public sealed class PostgresConfigurationValidator : AbstractValidator<PostgresConfiguration>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PostgresConfigurationValidator"/> class.
    /// </summary>
    public PostgresConfigurationValidator()
    {
        RuleFor(x => x.Host).NotEmpty().WithMessage("Postgres:Host is required.");

        RuleFor(x => x.Port)
            .InclusiveBetween(1, 65535)
            .WithMessage("Postgres:Port must be between 1 and 65535.");

        RuleFor(x => x.Database).NotEmpty().WithMessage("Postgres:Database is required.");

        RuleFor(x => x.Username).NotEmpty().WithMessage("Postgres:Username is required.");

        RuleFor(x => x.Password).NotEmpty().WithMessage("Postgres:Password is required.");
    }
}
