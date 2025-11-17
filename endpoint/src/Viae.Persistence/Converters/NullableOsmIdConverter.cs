// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Viae.Domain.Models;

namespace Viae.Persistence.Converters;

public class NullableOsmIdConverter : ValueConverter<OsmId?, string?>
{
    public NullableOsmIdConverter()
        : base(
            osmId => osmId.HasValue ? osmId.Value.ToString() : null,
            str => string.IsNullOrWhiteSpace(str) ? null : ParseOsmId(str)
        ) { }

    private static OsmId ParseOsmId(string str)
    {
        return OsmId.TryParse(str, out var result)
            ? result
            : throw new InvalidOperationException($"Unable to parse '{str}' as OsmId.");
    }
}
