// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Represents an ActivityPub Pong activity sent in response to a Ping.
/// The Pong's "object" field references the URI of the original Ping activity.
/// </summary>
public sealed class PongActivity : PingPongActivityBase
{
    /// <summary>
    /// Gets the URI of the original Ping activity that this Pong is responding to.
    /// This is typically a URI reference in the "object" field.
    /// </summary>
    [JsonIgnore]
    public Uri? PingUri
    {
        get
        {
            var obj = Objekt;
            return obj.HasValue && obj.Value.IsLink ? obj.Value.Link?.Href : null;
        }
    }
}
