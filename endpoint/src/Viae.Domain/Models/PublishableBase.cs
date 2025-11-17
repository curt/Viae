// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

public abstract class PublishableBase : UpdatableBase, IPublishable
{
    public DateTime? PublishedAt { get; set; }

    public virtual Status Status => PublishedAt != null ? Status.Manifestus : Status.Latens;
}
