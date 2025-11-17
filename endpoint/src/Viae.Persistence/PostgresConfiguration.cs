// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Globalization;

namespace Viae.Persistence;

/// <summary>
/// Configuration options for PostgreSQL database connection.
/// </summary>
public sealed class PostgresConfiguration
{
    /// <summary>
    /// The configuration section name.
    /// </summary>
    public const string SectionName = "Postgres";

    /// <summary>
    /// Gets or sets the database host.
    /// </summary>
    public string Host { get; set; } = "localhost";

    /// <summary>
    /// Gets or sets the database port.
    /// </summary>
    public int Port { get; set; } = 5432;

    /// <summary>
    /// Gets or sets the database name.
    /// </summary>
    public string Database { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the database username.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the database password.
    /// </summary>
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the minimum pool size.
    /// </summary>
    public int MinPoolSize { get; set; }

    /// <summary>
    /// Gets or sets the maximum pool size.
    /// </summary>
    public int MaxPoolSize { get; set; } = 100;

    /// <summary>
    /// Gets or sets the connection timeout in seconds.
    /// </summary>
    public int ConnectionTimeout { get; set; } = 15;

    /// <summary>
    /// Gets or sets whether to include error details in exceptions.
    /// </summary>
    public bool IncludeErrorDetail { get; set; }

    /// <summary>
    /// Builds a Npgsql connection string from the configuration values.
    /// </summary>
    /// <returns>A PostgreSQL connection string.</returns>
    public string BuildConnectionString()
    {
        var builder = new System.Text.StringBuilder();

        builder.Append(CultureInfo.InvariantCulture, $"Host={Host}");
        builder.Append(CultureInfo.InvariantCulture, $";Port={Port}");
        builder.Append(CultureInfo.InvariantCulture, $";Database={Database}");
        builder.Append(CultureInfo.InvariantCulture, $";Username={Username}");
        builder.Append(CultureInfo.InvariantCulture, $";Password={Password}");
        builder.Append(CultureInfo.InvariantCulture, $";Minimum Pool Size={MinPoolSize}");
        builder.Append(CultureInfo.InvariantCulture, $";Maximum Pool Size={MaxPoolSize}");
        builder.Append(CultureInfo.InvariantCulture, $";Timeout={ConnectionTimeout}");
        builder.Append(CultureInfo.InvariantCulture, $";Include Error Detail={IncludeErrorDetail}");

        return builder.ToString();
    }
}
