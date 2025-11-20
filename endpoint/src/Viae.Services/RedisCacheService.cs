// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using System.Text.Json;
using StackExchange.Redis;
using Viae.Domain.Services;

namespace Viae.Services;

/// <summary>
/// Redis-based implementation of caching service with JSON serialization.
/// </summary>
public sealed class RedisCacheService : IObjectCacheService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly JsonSerializerOptions _jsonOptions;

    public RedisCacheService(
        IConnectionMultiplexer redis,
        JsonSerializerOptions? jsonOptions = null
    )
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _jsonOptions =
            jsonOptions
            ?? new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = false,
            };
    }

    public async Task<T?> GetAsync<T>(string key, CancellationToken ct = default)
        where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var db = _redis.GetDatabase();
        var value = await db.StringGetAsync(key);

        return !value.HasValue
            ? null
            : JsonSerializer.Deserialize<T>(value.ToString(), _jsonOptions);
    }

    public async Task SetAsync<T>(
        string key,
        T value,
        TimeSpan expiration,
        CancellationToken ct = default
    )
        where T : class
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);
        ArgumentNullException.ThrowIfNull(value);

        if (expiration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expiration),
                "Expiration must be greater than zero."
            );
        }

        var db = _redis.GetDatabase();
        var json = JsonSerializer.Serialize(value, _jsonOptions);

        await db.StringSetAsync(key, json, expiration);
    }

    public async Task<bool> InvalidateAsync(string key, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var db = _redis.GetDatabase();
        return await db.KeyDeleteAsync(key);
    }

    public async Task<bool> ExistsAsync(string key, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(key);

        var db = _redis.GetDatabase();
        return await db.KeyExistsAsync(key);
    }
}
