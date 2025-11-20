// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using NetTopologySuite.Geometries;

namespace Viae.Services.Tests.TestData;

/// <summary>
/// Provides predefined coordinate points for testing.
/// All coordinates use SRID 4326 (WGS 84).
/// </summary>
public static class TestCoordinates
{
    /// <summary>
    /// San Francisco city center coordinates.
    /// </summary>
    public static Point SanFranciscoCenter { get; } = Create(37.7749, -122.4194);

    /// <summary>
    /// Golden Gate Bridge coordinates.
    /// </summary>
    public static Point GoldenGateBridge { get; } = Create(37.8199, -122.4783);

    /// <summary>
    /// Golden Gate Park coordinates.
    /// </summary>
    public static Point GoldenGatePark { get; } = Create(37.7694, -122.4862);

    /// <summary>
    /// Alcatraz Island coordinates.
    /// </summary>
    public static Point AlcatrazIsland { get; } = Create(37.8267, -122.4230);

    /// <summary>
    /// A location very close to San Francisco center (within 100m).
    /// </summary>
    public static Point VeryNear { get; } = Create(37.7750, -122.4195);

    /// <summary>
    /// A location moderately close to San Francisco center (within 5km).
    /// </summary>
    public static Point ModeratelyClose { get; } = Create(37.8000, -122.4500);

    /// <summary>
    /// A location far from San Francisco center (over 50km).
    /// </summary>
    public static Point VeryFar { get; } = Create(38.0000, -122.0000);

    /// <summary>
    /// New York City center coordinates (very far from San Francisco).
    /// </summary>
    public static Point NewYorkCenter { get; } = Create(40.7128, -74.0060);

    /// <summary>
    /// Origin point (0, 0) for simple tests.
    /// </summary>
    public static Point Origin { get; } = Create(0.0, 0.0);

    /// <summary>
    /// Simple test location A.
    /// </summary>
    public static Point LocationA { get; } = Create(37.5, -122.5);

    /// <summary>
    /// Simple test location B.
    /// </summary>
    public static Point LocationB { get; } = Create(37.6, -122.6);

    /// <summary>
    /// Simple test location C.
    /// </summary>
    public static Point LocationC { get; } = Create(37.7, -122.7);

    /// <summary>
    /// Creates a new Point with the specified latitude and longitude.
    /// </summary>
    /// <param name="latitude">The latitude (Y coordinate).</param>
    /// <param name="longitude">The longitude (X coordinate).</param>
    /// <returns>A Point with SRID 4326.</returns>
    public static Point Create(double latitude, double longitude)
    {
        return new Point(longitude, latitude) { SRID = 4326 };
    }
}
