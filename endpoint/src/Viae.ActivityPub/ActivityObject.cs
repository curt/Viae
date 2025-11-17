// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Base class for all ActivityPub objects and activities.
/// Uses custom JSON converter to handle polymorphic deserialization regardless of property order.
/// </summary>
/// <remarks>
/// Supported ActivityPub types (must be kept in sync with ActivityObjectJsonConverter):
/// <list type="bullet">
/// <item><description>Ping - PingActivity</description></item>
/// <item><description>Pong - PongActivity</description></item>
/// <item><description>Person - Person</description></item>
/// <item><description>Note - Note</description></item>
/// <item><description>Create - CreateActivity</description></item>
/// <item><description>Follow - FollowActivity</description></item>
/// <item><description>Like - LikeActivity</description></item>
/// <item><description>Collection - Collection</description></item>
/// <item><description>OrderedCollection - OrderedCollection</description></item>
/// <item><description>CollectionPage - CollectionPage</description></item>
/// <item><description>OrderedCollectionPage - OrderedCollectionPage</description></item>
/// </list>
/// </remarks>
[JsonConverter(typeof(ActivityObjectJsonConverter))]
public abstract class ActivityObject
{
    /// <summary>
    /// Gets or sets the unique identifier for this object.
    /// </summary>
    [JsonPropertyName("id")]
    public Uri? Id { get; set; }

    /// <summary>
    /// Gets or sets the browseable URL for this object.
    /// </summary>
    [JsonPropertyName("url")]
    public LinkOr<Uri>? Url { get; set; }

    /// <summary>
    /// Gets or sets the @context for this object.
    /// Typically "https://www.w3.org/ns/activitystreams" or a more complex array.
    /// </summary>
    [JsonPropertyName("@context")]
    public object? Context { get; set; } = "https://www.w3.org/ns/activitystreams";

    /// <summary>
    /// Gets or sets extra properties that aren't explicitly defined in the model.
    /// Allows for extensibility without defining every possible ActivityStreams property.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? Extra { get; set; }
}
