// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// Enumerates the OpenStreetMap element types supported by the Overpass service.
/// </summary>
public enum OsmType : byte
{
    /// <summary>
    /// A single OpenStreetMap node element.
    /// </summary>
    Node = 0,

    /// <summary>
    /// An OpenStreetMap way element composed of multiple nodes.
    /// </summary>
    Way = 1,

    /// <summary>
    /// An OpenStreetMap relation element describing associations between nodes and ways.
    /// </summary>
    Relation = 2,
}
