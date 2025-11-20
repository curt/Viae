// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Represents an ActivityPub OrderedCollection object.
/// An OrderedCollection is a subtype of Collection where the items are explicitly ordered.
/// </summary>
[SuppressMessage(
    "Naming",
    "CA1711:Identifiers should not have incorrect suffix",
    Justification = "OrderedCollection is the name defined by the ActivityStreams specification"
)]
public class OrderedCollection : Collection
{
    /// <summary>
    /// Gets or sets the ordered items in this collection.
    /// Can be a single value or an array, and each can be a URI or inline object.
    /// Note: This overrides the base Collection.Items property with explicit ordering semantics.
    /// </summary>
    [JsonPropertyName("orderedItems")]
    public OneOrMany<LinkOr<ActivityObject>>? OrderedItems { get; set; }
}
