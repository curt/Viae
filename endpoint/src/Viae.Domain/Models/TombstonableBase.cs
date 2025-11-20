// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

public abstract class TombstonableBase : PublishableBase, ITombstonable
{
    public DateTime? TombstonedAt { get; set; }

    public override Status Status =>
        TombstonedAt != null ? Status.Deletus
        : PublishedAt != null ? Status.Manifestus
        : Status.Latens;
}
