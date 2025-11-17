// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.Domain.Services;

/// <summary>
/// Interface for object caching operations with expiration support.
/// </summary>
public interface IObjectCacheService
{
    /// <summary>
    /// Retrieves a cached value by key.
    /// </summary>
    Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        where T : class;

    /// <summary>
    /// Stores a value in cache with an expiration time.
    /// </summary>
    Task SetAsync<T>(string key, T value, TimeSpan expiration, CancellationToken ct = default)
        where T : class;

    /// <summary>
    /// Removes a cached value by key.
    /// </summary>
    Task<bool> InvalidateAsync(string key, CancellationToken ct = default);

    /// <summary>
    /// Checks if a key exists in the cache.
    /// </summary>
    Task<bool> ExistsAsync(string key, CancellationToken ct = default);
}
