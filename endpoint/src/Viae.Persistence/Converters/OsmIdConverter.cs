// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Viae.Domain.Models;

namespace Viae.Persistence.Converters;

public class OsmIdConverter : ValueConverter<OsmId, string>
{
    public OsmIdConverter()
        : base(osmId => osmId.ToString(), str => ParseOsmId(str)) { }

    private static OsmId ParseOsmId(string str)
    {
        return OsmId.TryParse(str, out var result)
            ? result
            : throw new InvalidOperationException($"Unable to parse '{str}' as OsmId.");
    }
}
