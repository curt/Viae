// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// The origin or provenance of an object.
/// </summary>
public enum Origo
{
    /// <summary>
    /// A local, internal object.
    /// </summary>
    Domesticus,

    /// <summary>
    /// A remote, external object.
    /// </summary>
    Externus,
}
