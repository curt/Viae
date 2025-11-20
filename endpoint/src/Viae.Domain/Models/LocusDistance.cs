// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// Represents a Locus and its distance to a reference Locus.
/// </summary>
public sealed class LocusDistance
{
    public required Locus Locus { get; set; }
    public required Locus ReferenceLocus { get; set; }

    /// <summary>
    /// Distance in meters.
    /// </summary>
    public double Distance { get; set; }
    public bool IsReferenceLocus => Locus.Id == ReferenceLocus.Id;
}
