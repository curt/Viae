// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

namespace Viae.ActivityPub;

/// <summary>
/// Represents an ActivityPub Ping activity for testing federation connectivity.
/// Ping activities are sent to test HTTP signatures and federation connectivity.
/// </summary>
public sealed class PingActivity : PingPongActivityBase { }
