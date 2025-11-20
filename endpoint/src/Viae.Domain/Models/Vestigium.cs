// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// Represents a check-in.
/// </summary>
public sealed class Vestigium : TombstonableBase
{
    public required string PersonaId { get; set; }

    public Persona Persona { get; set; } = null!;

    public required string LocusId { get; set; }

    public Locus Locus { get; set; } = null!;

    public required DateTime HappenedAt { get; set; }

    public required Origo Origo { get; set; }

    public required string Content { get; set; }

    public required Uri Uri { get; set; }
}
