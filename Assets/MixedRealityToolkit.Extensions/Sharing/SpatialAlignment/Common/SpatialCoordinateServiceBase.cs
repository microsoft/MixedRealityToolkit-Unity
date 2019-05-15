// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using Microsoft.MixedReality.Toolkit.Utilities;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.MixedReality.SpatialAlignment.Common
{
    /// <summary>
    /// Helper base class for <see cref="ISpatialCoordinateService"/> implementations.
    /// </summary>
    /// <typeparam name="TKey">They key for the <see cref="ISpatialCoordinate"/>.</typeparam>
    public abstract class SpatialCoordinateServiceBase<TKey> : DisposableBase, ISpatialCoordinateService
    {
        /// <inheritdoc />
        public event Action<ISpatialCoordinate> CoordinatedDiscovered;

        private readonly object discoveryLockObject = new object();

        private CancellationTokenSource trackingCTS = null;
        private ConcurrentBag<ISpatialCoordinate> discoveredCoordinates = null;

        private bool isTracking;

        /// <inheritdoc />
        public bool IsTracking
        {
            get
            {
                ThrowIfDisposed();
                return isTracking;
            }
        }

        protected readonly ConcurrentDictionary<TKey, ISpatialCoordinate> knownCoordinates = new ConcurrentDictionary<TKey, ISpatialCoordinate>();

        /// <inheritdoc />
        public IEnumerable<ISpatialCoordinate> KnownCoordinates
        {
            get
            {
                ThrowIfDisposed();

                return knownCoordinates.Values.Cast<ISpatialCoordinate>();
            }
        }

        protected override void OnManagedDispose()
        {
            base.OnManagedDispose();

            knownCoordinates.Clear();
            StopAllTracking();
        }

        /// <summary>
        /// Adds a coordinate to be tracked by this service.
        /// </summary>
        private void OnNewCoordinate(TKey id, ISpatialCoordinate spatialCoordinate)
        {
            ThrowIfDisposed();

            if (knownCoordinates.TryAdd(id, spatialCoordinate))
            {
                discoveredCoordinates.Add(spatialCoordinate);
                CoordinatedDiscovered?.Invoke(spatialCoordinate);
            }
            else
            {
                throw new InvalidOperationException($"Coordinate with id '{id}' already discovered.");
            }
        }

        /// <inheritdoc />
        public virtual Task<ISpatialCoordinate> TryCreateCoordinateAsync(Vector3 worldPosition, Quaternion worldRotation, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            return Task.FromResult<ISpatialCoordinate>(null);
        }

        /// <inheritdoc />
        public virtual Task<bool> TryDeleteCoordinateAsync(string id, CancellationToken cancellationToken)
        {
            ThrowIfDisposed();

            return Task.FromResult(false);
        }

        /// <inheritdoc />
        public void StopAllTracking()
        {
            lock (discoveryLockObject)
            {
                trackingCTS?.Cancel();
                trackingCTS?.Dispose();
                trackingCTS = null;

                isTracking = false;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ISpatialCoordinate>> DiscoverCoordinatesAsync(CancellationToken cancellationToken)
        {
            CancellationToken trackingToken;
            lock (discoveryLockObject)
            {
                ThrowIfDisposed();
                if (isTracking)
                {
                    throw new InvalidOperationException("Discovery or Location is already running, please cancel that operation first.");
                }

                isTracking = true;

                discoveredCoordinates = new ConcurrentBag<ISpatialCoordinate>();
                trackingCTS = new CancellationTokenSource();
                trackingToken = trackingCTS.Token;
            }

            try
            {
                using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(trackingToken, cancellationToken))
                {
                    await OnDiscoverCoordinatesAsync(OnNewCoordinate, cts.Token).IgnoreCancellation();
                }

                return discoveredCoordinates;
            }
            finally
            {
                isTracking = false;
                discoveredCoordinates = null;
            }
        }

        /// <inheritdoc />
        public async Task<IEnumerable<ISpatialCoordinate>> LocateCoordinatesAsync(string[] coordinateIds, CancellationToken cancellationToken)
        {
            CancellationToken trackingToken;
            lock (discoveryLockObject)
            {
                ThrowIfDisposed();
                if (isTracking)
                {
                    throw new InvalidOperationException("Discovery or Location is already running, please cancel that operation first.");
                }

                isTracking = true;

                discoveredCoordinates = new ConcurrentBag<ISpatialCoordinate>();
                trackingCTS = new CancellationTokenSource();
                trackingToken = trackingCTS.Token;
            }

            try
            {
                using (CancellationTokenSource cts = CancellationTokenSource.CreateLinkedTokenSource(trackingToken, cancellationToken))
                {
                    await OnLocateCoordinatesAsync(OnNewCoordinate, coordinateIds, cts.Token).IgnoreCancellation();
                }

                return discoveredCoordinates;
            }
            finally
            {
                isTracking = false;
                discoveredCoordinates = null;
            }
        }

        /// <summary>
        /// Implement this method for the logic begin and end tracking (when <see cref="CancellationToken"/> is cancelled).
        /// </summary>
        protected abstract Task OnDiscoverCoordinatesAsync(Action<TKey, ISpatialCoordinate> onNewCoordinate, CancellationToken cancellationToken);

        /// <summary>
        /// Implement this method for the logic begin and end tracking (when <see cref="CancellationToken"/> is cancelled).
        /// </summary>
        protected abstract Task OnLocateCoordinatesAsync(Action<TKey, ISpatialCoordinate> onNewCoordinate, string[] coordinateIds, CancellationToken cancellationToken);
    }
}
