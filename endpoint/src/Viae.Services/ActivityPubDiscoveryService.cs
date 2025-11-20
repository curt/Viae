// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Viae.Services;

/// <summary>
/// Service for discovering ActivityPub actor information from remote instances.
/// </summary>
public partial class ActivityPubDiscoveryService(
    IHttpClientFactory httpClientFactory,
    ILogger<ActivityPubDiscoveryService> logger
) : IActivityPubDiscoveryService
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

    #region Logging

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Discovering inbox for actor: {ActorUri}"
    )]
    private partial void LogDiscoveringInbox(Uri actorUri);

    [LoggerMessage(
        Level = LogLevel.Information,
        Message = "Successfully discovered inbox: {InboxUri} for actor: {ActorUri}"
    )]
    private partial void LogInboxDiscovered(Uri inboxUri, Uri actorUri);

    [LoggerMessage(
        Level = LogLevel.Warning,
        Message = "Failed to discover inbox for actor: {ActorUri}. Status: {StatusCode}"
    )]
    private partial void LogInboxDiscoveryFailed(Uri actorUri, int statusCode);

    [LoggerMessage(
        Level = LogLevel.Error,
        Message = "Error discovering inbox for actor: {ActorUri}"
    )]
    private partial void LogInboxDiscoveryError(Exception ex, Uri actorUri);

    #endregion

    /// <inheritdoc />
    public async Task<Uri?> DiscoverInboxAsync(Uri actorUri)
    {
        LogDiscoveringInbox(actorUri);

        try
        {
            var request = new HttpRequestMessage(HttpMethod.Get, actorUri);
            request.Headers.Add("Accept", "application/activity+json");

            var response = await _httpClient.SendAsync(request);

            if (!response.IsSuccessStatusCode)
            {
                LogInboxDiscoveryFailed(actorUri, (int)response.StatusCode);
                return null;
            }

            var content = await response.Content.ReadAsStringAsync();
            var actorObject = JsonDocument.Parse(content);

            // Try to get the inbox from the actor object
            if (
                actorObject.RootElement.TryGetProperty("inbox", out var inboxElement)
                && inboxElement.ValueKind == JsonValueKind.String
            )
            {
                var inboxUriString = inboxElement.GetString();
                if (
                    !string.IsNullOrEmpty(inboxUriString)
                    && Uri.TryCreate(inboxUriString, UriKind.Absolute, out var inboxUri)
                    && (
                        inboxUri.Scheme == Uri.UriSchemeHttp
                        || inboxUri.Scheme == Uri.UriSchemeHttps
                    )
                )
                {
                    LogInboxDiscovered(inboxUri, actorUri);
                    return inboxUri;
                }
            }

            logger.LogWarning(
                "Actor object at {ActorUri} does not contain a valid 'inbox' property",
                actorUri
            );
            return null;
        }
        catch (Exception ex)
        {
            LogInboxDiscoveryError(ex, actorUri);
            return null;
        }
    }
}
