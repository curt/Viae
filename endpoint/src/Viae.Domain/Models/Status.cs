// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Models;

/// <summary>
/// Represents the publication state of a <c>Folium</c>, <c>Vestigium</c>,
/// or other publishable entity within the <i>Viae</i> ecosystem.
/// </summary>
/// <remarks>
/// <para>
/// This enumeration abstracts the logical state of an entity that may
/// otherwise be inferred from timestamp columns such as
/// <c>PublishedAt</c> and <c>DeletedAt</c>.
/// </para>
/// <para>
/// <list type="bullet">
/// <item><description>
/// <see cref="Latens"/> — The entity exists as a draft and has not been published.
/// </description></item>
/// <item><description>
/// <see cref="Manifestus"/> — The entity has been made public and is visible to others.
/// </description></item>
/// <item><description>
/// <see cref="Deletus"/> — The entity has been withdrawn or tombstoned and is no longer visible.
/// </description></item>
/// </list>
/// </para>
/// </remarks>
public enum Status
{
    /// <summary>
    /// Hidden or unpublished state — a draft awaiting publication.
    /// </summary>
    /// <remarks>
    /// Latin <i>latens</i>, meaning “hidden” or “concealed.”
    /// </remarks>
    Latens,

    /// <summary>
    /// Visible and publicly accessible state — the entity has been published.
    /// </summary>
    /// <remarks>
    /// Latin <i>manifestus</i>, meaning “made visible” or “evident.”
    /// </remarks>
    Manifestus,

    /// <summary>
    /// Tombstoned or withdrawn state — the entity has been removed from visibility.
    /// </summary>
    /// <remarks>
    /// Latin <i>deletus</i>, meaning “erased” or “destroyed.”
    /// </remarks>
    Deletus,
}
