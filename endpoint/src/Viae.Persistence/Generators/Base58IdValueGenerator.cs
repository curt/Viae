// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace Viae.Persistence.Generators;

public sealed class Base58IdValueGenerator : ValueGenerator<string>
{
    public override bool GeneratesTemporaryValues => false;

    public override string Next(EntityEntry entry) => Base58IdGenerator.Generate();
}
