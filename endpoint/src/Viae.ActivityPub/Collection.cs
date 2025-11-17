// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Represents an ActivityPub Collection object.
/// A Collection is a subtype of ActivityPub Object that represents ordered or unordered sets of Object or Link instances.
/// </summary>
[SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "Collection is the name defined by the ActivityStreams specification"
)]
public class Collection : ActivityObject
{
    /// <summary>
    /// Gets or sets the total number of items in the collection.
    /// This is not necessarily the number of items in the current response.
    /// </summary>
    [JsonPropertyName("totalItems")]
    public int? TotalItems { get; set; }

    /// <summary>
    /// Gets or sets the current page of items in this collection.
    /// Can be a single value or an array, and each can be a URI or inline object.
    /// </summary>
    [JsonPropertyName("current")]
    public LinkOr<CollectionPage>? Current { get; set; }

    /// <summary>
    /// Gets or sets the first page of items in this collection.
    /// Can be a single value or an array, and each can be a URI or inline object.
    /// </summary>
    [JsonPropertyName("first")]
    public LinkOr<CollectionPage>? First { get; set; }

    /// <summary>
    /// Gets or sets the last page of items in this collection.
    /// Can be a single value or an array, and each can be a URI or inline object.
    /// </summary>
    [JsonPropertyName("last")]
    public LinkOr<CollectionPage>? Last { get; set; }

    /// <summary>
    /// Gets or sets the items in this collection.
    /// Can be a single value or an array, and each can be a URI or inline object.
    /// Note: This is typically only used when the collection is not paginated.
    /// </summary>
    [JsonPropertyName("items")]
    public OneOrMany<LinkOr<ActivityObject>>? Items { get; set; }
}
