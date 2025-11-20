// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Viae.Domain.Models;

namespace Viae.Services.Tests.TestBuilders;

/// <summary>
/// Fluent builder for creating Persona test data.
/// </summary>
public class PersonaBuilder
{
    private string _id = Guid.NewGuid().ToString("N")[..12]; // Short test ID
    private Origo _origo = Origo.Domesticus;
    private string _username = "testuser";
    private string _displayName = "Test User";
    private string? _email = "test@example.com";
    private Guid? _cognitoUserId = Guid.NewGuid();
    private string? _bio;
    private string? _avatarUrl;
    private DateTime _createdAt = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private DateTime? _updatedAt;
    private Uri? _uri;
    private Uri? _url;
    private string? _publicKey;
    private string? _privateKey;

    /// <summary>
    /// Creates a new PersonaBuilder with default values.
    /// </summary>
    public static PersonaBuilder Default() => new();

    /// <summary>
    /// Sets the ID of the persona.
    /// </summary>
    public PersonaBuilder WithId(string id)
    {
        _id = id;
        return this;
    }

    /// <summary>
    /// Sets the origin of the persona.
    /// </summary>
    public PersonaBuilder WithOrigo(Origo origo)
    {
        _origo = origo;
        return this;
    }

    /// <summary>
    /// Sets the username of the persona.
    /// </summary>
    public PersonaBuilder WithUsername(string username)
    {
        _username = username;
        return this;
    }

    /// <summary>
    /// Sets the display name of the persona.
    /// </summary>
    public PersonaBuilder WithDisplayName(string displayName)
    {
        _displayName = displayName;
        return this;
    }

    /// <summary>
    /// Sets the email of the persona.
    /// </summary>
    public PersonaBuilder WithEmail(string? email)
    {
        _email = email;
        return this;
    }

    /// <summary>
    /// Sets the Cognito user ID of the persona.
    /// </summary>
    public PersonaBuilder WithCognitoUserId(Guid? cognitoUserId)
    {
        _cognitoUserId = cognitoUserId;
        return this;
    }

    /// <summary>
    /// Sets the bio of the persona.
    /// </summary>
    public PersonaBuilder WithBio(string bio)
    {
        _bio = bio;
        return this;
    }

    /// <summary>
    /// Sets the avatar URL of the persona.
    /// </summary>
    public PersonaBuilder WithAvatarUrl(string avatarUrl)
    {
        _avatarUrl = avatarUrl;
        return this;
    }

    /// <summary>
    /// Sets the created date of the persona.
    /// </summary>
    public PersonaBuilder WithCreatedAt(DateTime createdAt)
    {
        _createdAt = createdAt;
        return this;
    }

    /// <summary>
    /// Sets the updated date of the persona.
    /// </summary>
    public PersonaBuilder WithUpdatedAt(DateTime? updatedAt)
    {
        _updatedAt = updatedAt;
        return this;
    }

    /// <summary>
    /// Sets the ActivityPub URI of the persona.
    /// </summary>
    public PersonaBuilder WithUri(Uri uri)
    {
        _uri = uri;
        return this;
    }

    /// <summary>
    /// Sets the ActivityPub URL of the persona.
    /// </summary>
    public PersonaBuilder WithUrl(Uri url)
    {
        _url = url;
        return this;
    }

    /// <summary>
    /// Sets the public key of the persona.
    /// </summary>
    public PersonaBuilder WithPublicKey(string publicKey)
    {
        _publicKey = publicKey;
        return this;
    }

    /// <summary>
    /// Sets the private key of the persona.
    /// </summary>
    public PersonaBuilder WithPrivateKey(string privateKey)
    {
        _privateKey = privateKey;
        return this;
    }

    /// <summary>
    /// Configures the persona as a remote (Externus) persona.
    /// </summary>
    public PersonaBuilder AsRemote()
    {
        _origo = Origo.Externus;
        _cognitoUserId = null;
        _privateKey = null;
        return this;
    }

    /// <summary>
    /// Configures the persona as a local (Domesticus) persona.
    /// </summary>
    public PersonaBuilder AsLocal()
    {
        _origo = Origo.Domesticus;
        _cognitoUserId ??= Guid.NewGuid();
        return this;
    }

    /// <summary>
    /// Builds the Persona with the configured values.
    /// </summary>
    public Persona Build()
    {
        return new Persona
        {
            Id = _id,
            Origo = _origo,
            Username = _username,
            DisplayName = _displayName,
            Email = _email,
            CognitoUserId = _cognitoUserId,
            Bio = _bio,
            AvatarUrl = _avatarUrl,
            CreatedAt = _createdAt,
            UpdatedAt = _updatedAt,
            Uri = _uri,
            Url = _url,
            PublicKey = _publicKey,
            PrivateKey = _privateKey,
        };
    }

    #region Factory Methods

    /// <summary>
    /// Creates a simple persona for basic tests.
    /// </summary>
    public static PersonaBuilder Simple(string username = "testuser") =>
        Default().WithUsername(username).WithDisplayName(username);

    /// <summary>
    /// Creates a local persona with full configuration.
    /// </summary>
    public static Persona LocalPersona(string username = "localuser") =>
        Simple(username)
            .AsLocal()
            .WithUri(new Uri($"https://example.com/users/{username}"))
            .WithUrl(new Uri($"https://example.com/@{username}"))
            .Build();

    /// <summary>
    /// Creates a remote persona with full configuration.
    /// </summary>
    public static Persona RemotePersona(string username = "remoteuser") =>
        Simple(username)
            .AsRemote()
            .WithUri(new Uri($"https://remote.example.com/users/{username}"))
            .WithUrl(new Uri($"https://remote.example.com/@{username}"))
            .Build();

    #endregion
}
