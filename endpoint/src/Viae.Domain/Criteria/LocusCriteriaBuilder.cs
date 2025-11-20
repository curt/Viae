// Copyright © 2025 Curt Gilman
// SPDX-License-Identifier: AGPL-3.0-only
// Viae: A geo-centric, journey-focused, federated blog platform

using NetTopologySuite.Geometries;

namespace Viae.Domain.Criteria;

public class LocusCriteriaBuilder
{
    private readonly LocusCriteria _criteria = new();

    public LocusCriteriaBuilder Near(Point point, double maxDistanceMeters)
    {
        _criteria.NearPoint = point;
        _criteria.MaxDistanceMeters = maxDistanceMeters;
        return this;
    }

    public LocusCriteriaBuilder WithMinThemata(int count)
    {
        _criteria.MinThemataCount = count;
        return this;
    }

    public LocusCriteriaBuilder HasThema(string themaId)
    {
        _criteria.HasThemaId = themaId;
        return this;
    }

    public LocusCriteriaBuilder SortByDistance()
    {
        _criteria.SortBy = LocusSortField.Distance;
        return this;
    }

    public LocusCriteriaBuilder SortByName()
    {
        _criteria.SortBy = LocusSortField.Name;
        return this;
    }

    public LocusCriteriaBuilder Descending()
    {
        _criteria.SortDirection = SortDirection.Descending;
        return this;
    }

    public LocusCriteriaBuilder Page(int pageNumber, int pageSize)
    {
        _criteria.PageNumber = pageNumber;
        _criteria.PageSize = pageSize;
        return this;
    }

    public LocusCriteriaBuilder IncludeVestigia(VestigiumCriteria criteria)
    {
        _criteria.IncludesVestigia = criteria;
        return this;
    }

    public LocusCriteria Build() => _criteria;
}
