// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Represents a base ActivityPub Ping or Pong activity for testing federation connectivity.
/// </summary>
public class PingPongActivityBase : Activity
{
    /// <summary>
    /// Gets the actor URI as a string, if the actor is a link reference.
    /// </summary>
    [JsonIgnore]
    public Uri? ActorUri => Actor?.Link?.Href;

    /// <summary>
    /// Gets the inline actor object, if provided.
    /// </summary>
    [JsonIgnore]
    public ActivityObject? ActorObject => Actor?.Value;

    /// <summary>
    /// Gets the "to" recipient as a URI, if there's a single recipient that's a link reference.
    /// </summary>
    [JsonIgnore]
    public Uri? ToUri
    {
        get
        {
            var first = To?.FirstOrDefault();
            return first.HasValue && first.Value.IsLink ? first.Value.Link?.Href : null;
        }
    }

    /// <summary>
    /// Gets all "to" recipients as URIs.
    /// </summary>
    [JsonIgnore]
    public IEnumerable<Uri> ToUris =>
        To?.Where(t => t.IsLink).Select(t => t.Link!.Value.Href) ?? [];
}
