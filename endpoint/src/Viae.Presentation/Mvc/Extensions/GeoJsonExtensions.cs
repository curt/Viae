// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace Viae.Presentation.Mvc.Extensions;

/// <summary>
/// Extension methods for creating GeoJSON representations from domain models.
/// </summary>
public static class GeoJsonExtensions
{
    /// <summary>
    /// Creates a GeoJSON Feature from a model with coordinates and attributes.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="model">The model to convert.</param>
    /// <param name="geometry">Function to extract the geometry from the model.</param>
    /// <param name="attributes">Function to create the attributes table from the model.</param>
    /// <returns>A GeoJSON Feature.</returns>
    public static Feature ToGeoJsonFeature<T>(
        this T model,
        Func<T, Geometry> geometry,
        Func<T, AttributesTable> attributes
    )
    {
        return new Feature(geometry(model), attributes(model));
    }

    /// <summary>
    /// Creates a GeoJSON FeatureCollection from a collection of models.
    /// </summary>
    /// <typeparam name="T">The model type.</typeparam>
    /// <param name="models">The models to convert.</param>
    /// <param name="geometry">Function to extract the geometry from each model.</param>
    /// <param name="attributes">Function to create the attributes table from each model.</param>
    /// <returns>A GeoJSON FeatureCollection.</returns>
    public static FeatureCollection ToGeoJsonFeatureCollection<T>(
        this IEnumerable<T> models,
        Func<T, Geometry> geometry,
        Func<T, AttributesTable> attributes
    )
    {
        var featureCollection = new FeatureCollection();

        foreach (var model in models)
        {
            var feature = model.ToGeoJsonFeature(geometry, attributes);
            featureCollection.Add(feature);
        }

        return featureCollection;
    }
}
