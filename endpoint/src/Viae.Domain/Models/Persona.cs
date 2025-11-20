// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// Represents a user of the system.
/// </summary>
public class Persona : IdentifiableBase
{
    /// <summary>
    /// Gets or sets the origin of this Persona.
    /// </summary>
    public Origo Origo { get; set; }

    /// <summary>
    /// Gets or sets the username for this Persona (unique, used for login and URLs).
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the display name shown in the UI for this Persona.
    /// </summary>
    public string DisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of this Persona.
    /// </summary>
    public string? Email { get; set; }

    /// <summary>
    /// Gets or sets the AWS Cognito user identifier (sub claim from JWT).
    /// This is the unique, immutable identifier for the Persona in Cognito.
    /// </summary>
    public Guid? CognitoUserId { get; set; }

    /// <summary>
    /// Gets or sets the biographical information about this Persona.
    /// </summary>
    public string? Bio { get; set; }

    /// <summary>
    /// Gets or sets the URL of the avatar image of this Persona.
    /// </summary>
    public string? AvatarUrl { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this Persona was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when this Persona was last updated. Null if never updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// The ActivityPub URI for this Persona.
    /// </summary>
    public Uri? Uri { get; set; }

    /// <summary>
    /// The ActivityPub URL for this Persona.
    /// </summary>
    public Uri? Url { get; set; }

    /// <summary>
    /// Public key for HTTP signatures (ActivityPub) for this Persona.
    /// </summary>
    public string? PublicKey { get; set; }

    /// <summary>
    /// Gets or sets the private key for HTTP signatures (ActivityPub) for this Persona.
    /// Null when Origo equals Externus.
    /// </summary>
    public string? PrivateKey { get; set; }

    /// <summary>
    /// Gets or sets the navigation property for Folios authored by this Persona.
    /// </summary>
    public ICollection<Folium> Folios { get; set; } = [];
}
