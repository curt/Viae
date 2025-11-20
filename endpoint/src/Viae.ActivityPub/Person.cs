// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Represents an ActivityPub Person (actor).
/// </summary>
public sealed class Person : ActivityObject
{
    /// <summary>
    /// Gets or sets the display name of this person.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the preferred username (handle) of this person.
    /// </summary>
    [JsonPropertyName("preferredUsername")]
    public string? PreferredUsername { get; set; }

    /// <summary>
    /// Gets or sets the summary/bio of this person.
    /// </summary>
    [JsonPropertyName("summary")]
    public string? Summary { get; set; }

    /// <summary>
    /// Gets or sets the inbox URI for this person.
    /// Can be a URI or an inline object (though typically a URI).
    /// </summary>
    [JsonPropertyName("inbox")]
    public LinkOr<ActivityObject>? Inbox { get; set; }

    /// <summary>
    /// Gets or sets the outbox URI for this person.
    /// </summary>
    [JsonPropertyName("outbox")]
    public LinkOr<ActivityObject>? Outbox { get; set; }

    /// <summary>
    /// Gets or sets the followers collection URI for this person.
    /// </summary>
    [JsonPropertyName("followers")]
    public LinkOr<ActivityObject>? Followers { get; set; }

    /// <summary>
    /// Gets or sets the following collection URI for this person.
    /// </summary>
    [JsonPropertyName("following")]
    public LinkOr<ActivityObject>? Following { get; set; }

    /// <summary>
    /// Gets or sets the public key information for HTTP signatures.
    /// </summary>
    [JsonPropertyName("publicKey")]
    public PublicKeyInfo? PublicKey { get; set; }
}

/// <summary>
/// Represents public key information for HTTP signatures.
/// </summary>
public sealed class PublicKeyInfo
{
    /// <summary>
    /// Gets or sets the ID of this public key.
    /// </summary>
    [JsonPropertyName("id")]
    public Uri? Id { get; set; }

    /// <summary>
    /// Gets or sets the owner of this public key (typically the actor's ID).
    /// </summary>
    [JsonPropertyName("owner")]
    public Uri? Owner { get; set; }

    /// <summary>
    /// Gets or sets the PEM-encoded public key.
    /// </summary>
    [JsonPropertyName("publicKeyPem")]
    public string? PublicKeyPem { get; set; }
}
