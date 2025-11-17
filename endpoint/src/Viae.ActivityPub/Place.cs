// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json.Serialization;

namespace Viae.ActivityPub;

public sealed class Place : ActivityObject
{
    /// <summary>
    /// Gets or sets the display name of this place.
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the latitude of this place.
    /// </summary>
    [JsonPropertyName("latitude")]
    public double? Latitude { get; set; }

    /// <summary>
    /// Gets or sets the longitude of this place.
    /// </summary>
    [JsonPropertyName("longitude")]
    public double? Longitude { get; set; }
}
