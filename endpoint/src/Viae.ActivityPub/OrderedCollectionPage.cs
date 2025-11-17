// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Represents an ActivityPub OrderedCollectionPage object.
/// An OrderedCollectionPage is a subtype of CollectionPage and OrderedCollection where the items are explicitly ordered.
/// </summary>
public class OrderedCollectionPage : CollectionPage
{
    /// <summary>
    /// Gets or sets the ordered items in this collection page.
    /// Can be a single value or an array, and each can be a URI or inline object.
    /// Note: This overrides the base Collection.Items property with explicit ordering semantics.
    /// </summary>
    [JsonPropertyName("orderedItems")]
    public OneOrMany<LinkOr<ActivityObject>>? OrderedItems { get; set; }

    /// <summary>
    /// Gets or sets the index of the first item in this page within the overall ordered collection.
    /// This is useful for implementations that need to know the absolute position of items.
    /// </summary>
    [JsonPropertyName("startIndex")]
    public int? StartIndex { get; set; }
}
