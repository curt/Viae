// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

/// <summary>
/// Represents an ActivityPub CollectionPage object.
/// A CollectionPage is a subtype of Collection that represents a single page within a larger collection.
/// </summary>
public class CollectionPage : Collection
{
    /// <summary>
    /// Gets or sets a reference to the collection this page is part of.
    /// Can be either a URI reference or an inline object.
    /// </summary>
    [JsonPropertyName("partOf")]
    public LinkOr<Collection>? PartOf { get; set; }

    /// <summary>
    /// Gets or sets the next page in the collection.
    /// Can be either a URI reference or an inline CollectionPage object.
    /// </summary>
    [JsonPropertyName("next")]
    public LinkOr<CollectionPage>? Next { get; set; }

    /// <summary>
    /// Gets or sets the previous page in the collection.
    /// Can be either a URI reference or an inline CollectionPage object.
    /// </summary>
    [JsonPropertyName("prev")]
    public LinkOr<CollectionPage>? Prev { get; set; }
}
