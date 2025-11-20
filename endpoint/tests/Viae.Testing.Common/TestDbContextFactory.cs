// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using Microsoft.EntityFrameworkCore;
using Viae.Persistence;

namespace Viae.Testing.Common;

public class TestDbContextFactory(DbContextOptions<ViaeDbContext> options)
    : IDbContextFactory<ViaeDbContext>
{
    public ViaeDbContext CreateDbContext()
    {
        return new TestViaeDbContext(options);
    }
}
