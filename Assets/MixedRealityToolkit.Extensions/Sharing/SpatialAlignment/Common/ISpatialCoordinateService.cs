// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System;
using System.Collections.Generic;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.SpatialAlignment.Common
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
        /// Gets or sets whether this coordiante service should be discovering/tracking coordinates.
        /// </summary>
        bool IsTracking { get; }

        /// <summary>
        /// Gets a set of all known coordinates to this service.
        /// </summary>
        IEnumerable<ISpatialCoordinate> KnownCoordinates { get; }

        /// <summary>
        /// Begins search for all coordinates.
        /// </summary>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to be used for cancellation (stopping) of the discovery task.</param>
        /// <returns>The set of coordinates discovered during this session.</returns>
        Task<IEnumerable<ISpatialCoordinate>> DiscoverCoordinatesAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Begins locating specific coordinates (provided by Ids).
        /// </summary>
        /// <param name="coordinateIds">The coordinate ids to discover.</param>
        /// <param name="cancellationToken">The <see cref="CancellationToken"/> to be used for cancellation (stopping) of the locating task.</param>
        /// <returns>The set of coordinates discovered during this session.</returns>
        Task<IEnumerable<ISpatialCoordinate>> LocateCoordinatesAsync(string[] coordinateIds, CancellationToken cancellationToken);

        /// <summary>
        /// Stops all currently discovery or location operations.
        /// </summary>
        void StopAllTracking();

        /// <summary>
        /// Attempts to create a new coordinate with this service.
        /// </summary>
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
