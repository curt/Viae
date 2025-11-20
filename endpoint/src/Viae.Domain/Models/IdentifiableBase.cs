// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

public abstract class IdentifiableBase : IIdentifiable
{
    public string Id { get; set; } = default!;
}
