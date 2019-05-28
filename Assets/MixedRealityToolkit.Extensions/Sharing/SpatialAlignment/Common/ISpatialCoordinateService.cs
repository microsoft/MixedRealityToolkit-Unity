// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.Experimental.SpatialAlignment.Common
{
    /// <summary>
    /// This service is used to discover, track and create coordinates.
    /// </summary>
    public interface ISpatialCoordinateService : IDisposable
    {
        /// <summary>
        /// Triggered when a new coordinate is discovered or created with this service.
        /// </summary>
        /// <remarks>It may not be in the IsLocated state if created locally.</remarks>
        event Action<ISpatialCoordinate> CoordinatedDiscovered;

        /// <summary>
        /// Triggered when a new coordinate is removed from this service.
        /// </summary>
        event Action<ISpatialCoordinate> CoordinateRemoved;

        /// <summary>
        /// Gets or sets whether this coordinate service should be discovering/tracking coordinates.
        /// </summary>
        bool IsTracking { get; }

        /// <summary>
        /// Gets a set of all known coordinates to this service.
        /// </summary>
        IEnumerable<ISpatialCoordinate> KnownCoordinates { get; }

        /// <summary>
        /// A key based lookup for a known coordinate.
        /// </summary>
        /// <param name="id">The identifier of the coordinate to look up.</param>
        /// <param name="spatialCoordinate">The out parameter that will be filled with found coordinate, or null otherwise.</param>
        /// <returns>Returns true if coordinate was found (known locally), false otherwise.</returns>
        bool TryGetKnownCoordinate(string id, out ISpatialCoordinate spatialCoordinate);

        /// <summary>
        /// Begins search for coordinates, optionally priortizing a set of ids.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to be used for cancellation (stopping) of the discovery task.</param>
        /// <param name="coordinateIds">The optional set to prioritize discovery of ids to.</param>
        /// <returns>The set of coordinates discovered during this session.</returns>
        Task<bool> TryDiscoverCoordinatesAsync(CancellationToken cancellationToken, params string[] idsToLocate);

        /// <summary>
        /// Attempts to create a new coordinate with this service.
        /// </summary>
        /// <param name="id">Attempts to set id on the spatial coordinate, if possible.</param>
        /// <param name="localPosition">Position at which the coordinate should be created.</param>
        /// <param name="localRotation">Orientation the coordinate should be created with.</param>
        /// <returns>The coordinate if the coordinate was succesfully created, otherwise null.</returns>
        Task<ISpatialCoordinate> TryCreateCoordinateAsync(Vector3 worldPosition, Quaternion worldRotation, CancellationToken cancellationToken);

        /// <summary>
        /// Attempts to asynchronously delete a coordinate with this space given an Id.
        /// </summary>
        /// <param name="id">The id representing the coordinate.</param>
        /// <returns>True if the coordinate was succesfully deleted.</returns>
        Task<bool> TryDeleteCoordinateAsync(string id, CancellationToken cancellationToken);
    }
}
